using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LOutpostWire
{
    private static readonly HttpClient LOutpostWireClient = new(
        new HttpClientHandler { AllowAutoRedirect = false, UseProxy = false })
    {
        Timeout = TimeSpan.FromSeconds(100),
    };

    private const string LOutpostWireBanner = "JoplinClipperServer";

    private const int LOutpostWirePage = 100;

    private static readonly TimeSpan LOutpostWirePatience = TimeSpan.FromSeconds(1);

    public async Task<bool> LOutpostWireCheck(int port, CancellationToken cancellation)
    {
        using CancellationTokenSource limit = CancellationTokenSource.CreateLinkedTokenSource(cancellation);
        limit.CancelAfter(LOutpostWirePatience);
        try
        {
            using HttpRequestMessage request = new(HttpMethod.Get, LOutpostWireFormat(port, "ping", null));
            using HttpResponseMessage response = await LOutpostWireClient.SendAsync(request, limit.Token)
                .ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            string body = await response.Content.ReadAsStringAsync(limit.Token).ConfigureAwait(false);
            return string.Equals(body, LOutpostWireBanner, StringComparison.Ordinal);
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception) when (exception is HttpRequestException or OperationCanceledException)
        {
            return false;
        }
    }

    public async Task<HttpResponseMessage> LOutpostWireSend(
        HttpMethod method,
        string address,
        HttpContent? content,
        bool warranted,
        bool lenient,
        CancellationToken cancellation)
    {
        using HttpRequestMessage request = new(method, address) { Content = content };
        HttpResponseMessage response;
        try
        {
            response = await LOutpostWireClient.SendAsync(request, cancellation).ConfigureAwait(false);
        }
        catch (OperationCanceledException exception) when (!cancellation.IsCancellationRequested)
        {
            throw new TimeoutException("Joplin did not answer in time.", exception);
        }
        catch (HttpRequestException exception) when (exception.StatusCode is null)
        {
            throw new TimeoutException("Joplin could not be reached.", exception);
        }

        if (response.IsSuccessStatusCode || (lenient && response.StatusCode == HttpStatusCode.NotFound))
        {
            return response;
        }

        HttpStatusCode status = response.StatusCode;
        response.Dispose();
        if (warranted && status == HttpStatusCode.Forbidden)
        {
            throw new LRefusal(LRefusal.LRefusalWarrant);
        }

        throw new HttpRequestException(
            string.Create(CultureInfo.InvariantCulture, $"Joplin answered {(int)status} to {method}."),
            null,
            status);
    }

    public async Task<List<JsonObject>> LOutpostWireRead(
        int port, string token, string path, CancellationToken cancellation)
    {
        List<JsonObject> items = [];
        for (int page = 1; ; page++)
        {
            string paged = string.Create(
                CultureInfo.InvariantCulture, $"{path}?page={page}&limit={LOutpostWirePage}");
            using HttpResponseMessage response = await LOutpostWireSend(
                HttpMethod.Get, LOutpostWireFormat(port, paged, token), null, true, false, cancellation)
                .ConfigureAwait(false);
            JsonNode? answer = await LOutpostWireParse(response, cancellation).ConfigureAwait(false);
            if (answer is not JsonObject list)
            {
                return items;
            }

            if (list["items"] is JsonArray array)
            {
                foreach (JsonNode? item in array)
                {
                    if (item is JsonObject entry)
                    {
                        items.Add(entry);
                    }
                }
            }

            if (list["has_more"] is not JsonValue more || !more.TryGetValue(out bool next) || !next)
            {
                return items;
            }
        }
    }

    public static string LOutpostWireFormat(int port, string path, string? token)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(port, IPEndPoint.MinPort + 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(port, IPEndPoint.MaxPort);
        string address = string.Create(CultureInfo.InvariantCulture, $"http://127.0.0.1:{port}/{path}");
        if (token is null)
        {
            return address;
        }

        return address + (path.Contains('?', StringComparison.Ordinal) ? '&' : '?') + "token="
            + Uri.EscapeDataString(token);
    }

    public static StringContent LOutpostWireFormat(JsonObject body)
    {
        return new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json");
    }

    public static async Task<JsonNode?> LOutpostWireParse(
        HttpResponseMessage response, CancellationToken cancellation)
    {
        string text = await response.Content.ReadAsStringAsync(cancellation).ConfigureAwait(false);
        try
        {
            return string.IsNullOrWhiteSpace(text) ? null : JsonNode.Parse(text);
        }
        catch (JsonException exception)
        {
            throw new HttpRequestException("Joplin answered malformed JSON.", exception);
        }
    }

    public static string? LOutpostWireParse(JsonNode? node, string name)
    {
        return node is JsonObject entry && entry[name] is JsonValue value && value.TryGetValue(out string? text)
            ? text
            : null;
    }
}
