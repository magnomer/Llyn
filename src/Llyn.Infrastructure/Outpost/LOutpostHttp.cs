using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LOutpostHttp : LOutpost
{
    private readonly HttpClient _lOutpostClient;

    private const string LOutpostBanner = "JoplinClipperServer";

    private const int LOutpostFirst = 41184;

    private const int LOutpostLast = 41194;

    private const int LOutpostPage = 100;

    private static readonly TimeSpan LOutpostPatience = TimeSpan.FromSeconds(1);

    public LOutpostHttp(HttpClient client)
    {
        ArgumentNullException.ThrowIfNull(client);
        _lOutpostClient = client;
    }

    public async Task<int?> LOutpostFind(int port, CancellationToken cancellation)
    {
        List<int> ports = port > IPEndPoint.MinPort && port <= IPEndPoint.MaxPort ? [port] : [];
        for (int candidate = LOutpostFirst; candidate <= LOutpostLast; candidate++)
        {
            if (candidate != port)
            {
                ports.Add(candidate);
            }
        }

        foreach (int candidate in ports)
        {
            if (await LOutpostHttpCheck(candidate, cancellation).ConfigureAwait(false))
            {
                return candidate;
            }
        }

        return null;
    }

    public async Task<string> LOutpostWarrantStart(int port, CancellationToken cancellation)
    {
        using HttpResponseMessage response = await LOutpostHttpSend(
            HttpMethod.Post, LOutpostHttpFormat(port, "auth", null), null, false, false, cancellation)
            .ConfigureAwait(false);
        JsonNode? answer = await LOutpostHttpParse(response, cancellation).ConfigureAwait(false);
        string? ticket = LOutpostHttpParse(answer, "auth_token");
        if (string.IsNullOrEmpty(ticket))
        {
            throw new HttpRequestException("Joplin answered the token request without a ticket.");
        }

        return ticket;
    }

    public async Task<LWarrantAnswer> LOutpostWarrantCheck(int port, string ticket, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ticket);
        LWarrantAnswer rejected = new(LWarrantState.LWarrantStateRejected, null);
        string path = "auth/check?auth_token=" + Uri.EscapeDataString(ticket);
        try
        {
            using HttpResponseMessage response = await LOutpostHttpSend(
                HttpMethod.Get, LOutpostHttpFormat(port, path, null), null, false, false, cancellation)
                .ConfigureAwait(false);
            JsonNode? answer = await LOutpostHttpParse(response, cancellation).ConfigureAwait(false);
            string? token = LOutpostHttpParse(answer, "token");
            return LOutpostHttpParse(answer, "status") switch
            {
                "waiting" => new LWarrantAnswer(LWarrantState.LWarrantStateWaiting, null),
                "accepted" when !string.IsNullOrEmpty(token) =>
                    new LWarrantAnswer(LWarrantState.LWarrantStateAccepted, token),
                _ => rejected,
            };
        }
        catch (HttpRequestException exception)
            when (exception.StatusCode is not null || exception.InnerException is JsonException)
        {
            return rejected;
        }
    }

    public async Task LOutpostFolderSave(
        int port, string token, string id, string title, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(title);

        using HttpResponseMessage put = await LOutpostHttpSend(
            HttpMethod.Put,
            LOutpostHttpFormat(port, "folders/" + Uri.EscapeDataString(id), token),
            LOutpostHttpFormat(new JsonObject { ["title"] = title, ["deleted_time"] = 0 }),
            true,
            true,
            cancellation).ConfigureAwait(false);
        if (put.StatusCode != HttpStatusCode.NotFound)
        {
            return;
        }

        using HttpResponseMessage post = await LOutpostHttpSend(
            HttpMethod.Post,
            LOutpostHttpFormat(port, "folders", token),
            LOutpostHttpFormat(new JsonObject { ["id"] = id, ["title"] = title }),
            true,
            false,
            cancellation).ConfigureAwait(false);
    }

    public async Task LOutpostNoteSave(int port, string token, LOutpostNote note, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        ArgumentNullException.ThrowIfNull(note);
        ArgumentException.ThrowIfNullOrWhiteSpace(note.LOutpostNoteId);

        using HttpResponseMessage put = await LOutpostHttpSend(
            HttpMethod.Put,
            LOutpostHttpFormat(port, "notes/" + Uri.EscapeDataString(note.LOutpostNoteId), token),
            LOutpostHttpFormat(new JsonObject
            {
                ["title"] = note.LOutpostNoteTitle,
                ["body"] = note.LOutpostNoteBody,
                ["parent_id"] = note.LOutpostNoteFolder,
                ["deleted_time"] = 0,
            }),
            true,
            true,
            cancellation).ConfigureAwait(false);
        if (put.StatusCode != HttpStatusCode.NotFound)
        {
            return;
        }

        using HttpResponseMessage post = await LOutpostHttpSend(
            HttpMethod.Post,
            LOutpostHttpFormat(port, "notes", token),
            LOutpostHttpFormat(new JsonObject
            {
                ["id"] = note.LOutpostNoteId,
                ["title"] = note.LOutpostNoteTitle,
                ["body"] = note.LOutpostNoteBody,
                ["parent_id"] = note.LOutpostNoteFolder,
            }),
            true,
            false,
            cancellation).ConfigureAwait(false);
    }

    public async Task LOutpostNoteRemove(int port, string token, string id, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using HttpResponseMessage response = await LOutpostHttpSend(
            HttpMethod.Delete,
            LOutpostHttpFormat(port, "notes/" + Uri.EscapeDataString(id), token),
            null,
            true,
            true,
            cancellation).ConfigureAwait(false);
    }

    public async Task LOutpostTagSave(
        int port, string token, string id, IReadOnlyList<string> tags, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(tags);

        string note = Uri.EscapeDataString(id);
        HashSet<string> missing = new(StringComparer.Ordinal);
        foreach (string tag in tags)
        {
            if (!string.IsNullOrWhiteSpace(tag))
            {
                missing.Add(tag.Trim().ToLowerInvariant());
            }
        }

        List<JsonObject> current = await LOutpostHttpRead(port, token, "notes/" + note + "/tags", cancellation)
            .ConfigureAwait(false);
        foreach (JsonObject tag in current)
        {
            string? tagId = LOutpostHttpParse(tag, "id");
            string? title = LOutpostHttpParse(tag, "title");
            if (string.IsNullOrEmpty(tagId) || (title is not null && missing.Remove(title)))
            {
                continue;
            }

            using HttpResponseMessage detach = await LOutpostHttpSend(
                HttpMethod.Delete,
                LOutpostHttpFormat(port, "tags/" + Uri.EscapeDataString(tagId) + "/notes/" + note, token),
                null,
                true,
                true,
                cancellation).ConfigureAwait(false);
        }

        if (missing.Count == 0)
        {
            return;
        }

        List<JsonObject> known = await LOutpostHttpRead(port, token, "tags", cancellation).ConfigureAwait(false);
        foreach (string title in missing)
        {
            string? tagId = LOutpostHttpFind(known, title);
            if (tagId is null)
            {
                using HttpResponseMessage create = await LOutpostHttpSend(
                    HttpMethod.Post,
                    LOutpostHttpFormat(port, "tags", token),
                    LOutpostHttpFormat(new JsonObject { ["title"] = title }),
                    true,
                    false,
                    cancellation).ConfigureAwait(false);
                JsonNode? created = await LOutpostHttpParse(create, cancellation).ConfigureAwait(false);
                tagId = LOutpostHttpParse(created, "id");
                if (string.IsNullOrEmpty(tagId))
                {
                    throw new HttpRequestException("Joplin created a tag without answering its id.");
                }
            }

            using HttpResponseMessage attach = await LOutpostHttpSend(
                HttpMethod.Post,
                LOutpostHttpFormat(port, "tags/" + Uri.EscapeDataString(tagId) + "/notes", token),
                LOutpostHttpFormat(new JsonObject { ["id"] = id }),
                true,
                false,
                cancellation).ConfigureAwait(false);
        }
    }

    public async Task LOutpostParcelSave(int port, string token, LParcel parcel, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        ArgumentNullException.ThrowIfNull(parcel);
        ArgumentException.ThrowIfNullOrWhiteSpace(parcel.LParcelId);
        ArgumentNullException.ThrowIfNull(parcel.LParcelBytes);

        using HttpResponseMessage known = await LOutpostHttpSend(
            HttpMethod.Get,
            LOutpostHttpFormat(port, "resources/" + Uri.EscapeDataString(parcel.LParcelId), token),
            null,
            true,
            true,
            cancellation).ConfigureAwait(false);
        if (known.StatusCode != HttpStatusCode.NotFound)
        {
            return;
        }

        ByteArrayContent data = new(parcel.LParcelBytes);
        data.Headers.ContentType = MediaTypeHeaderValue.TryParse(parcel.LParcelMime, out MediaTypeHeaderValue? mime)
            ? mime
            : new MediaTypeHeaderValue("application/octet-stream");
        JsonObject props = new()
        {
            ["id"] = parcel.LParcelId,
            ["title"] = parcel.LParcelTitle,
            ["mime"] = data.Headers.ContentType.MediaType,
        };
        string name = parcel.LParcelTitle.Replace("\"", string.Empty, StringComparison.Ordinal).Trim();
        MultipartFormDataContent form = new()
        {
            { data, "data", name.Length == 0 ? parcel.LParcelId : name },
            { new StringContent(props.ToJsonString(), Encoding.UTF8), "props" },
        };

        using HttpResponseMessage post = await LOutpostHttpSend(
            HttpMethod.Post, LOutpostHttpFormat(port, "resources", token), form, true, false, cancellation)
            .ConfigureAwait(false);
    }

    private async Task<bool> LOutpostHttpCheck(int port, CancellationToken cancellation)
    {
        using CancellationTokenSource limit = CancellationTokenSource.CreateLinkedTokenSource(cancellation);
        limit.CancelAfter(LOutpostPatience);
        try
        {
            using HttpRequestMessage request = new(HttpMethod.Get, LOutpostHttpFormat(port, "ping", null));
            using HttpResponseMessage response = await _lOutpostClient.SendAsync(request, limit.Token)
                .ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            string body = await response.Content.ReadAsStringAsync(limit.Token).ConfigureAwait(false);
            return string.Equals(body, LOutpostBanner, StringComparison.Ordinal);
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

    private async Task<HttpResponseMessage> LOutpostHttpSend(
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
            response = await _lOutpostClient.SendAsync(request, cancellation).ConfigureAwait(false);
        }
        catch (OperationCanceledException exception) when (!cancellation.IsCancellationRequested)
        {
            throw new HttpRequestException("Joplin did not answer in time.", exception);
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

    private async Task<List<JsonObject>> LOutpostHttpRead(
        int port, string token, string path, CancellationToken cancellation)
    {
        List<JsonObject> items = [];
        for (int page = 1; ; page++)
        {
            string paged = string.Create(CultureInfo.InvariantCulture, $"{path}?page={page}&limit={LOutpostPage}");
            using HttpResponseMessage response = await LOutpostHttpSend(
                HttpMethod.Get, LOutpostHttpFormat(port, paged, token), null, true, false, cancellation)
                .ConfigureAwait(false);
            JsonNode? answer = await LOutpostHttpParse(response, cancellation).ConfigureAwait(false);
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

    private static string? LOutpostHttpFind(IReadOnlyList<JsonObject> tags, string title)
    {
        foreach (JsonObject tag in tags)
        {
            if (string.Equals(LOutpostHttpParse(tag, "title"), title, StringComparison.Ordinal))
            {
                return LOutpostHttpParse(tag, "id");
            }
        }

        return null;
    }

    private static string LOutpostHttpFormat(int port, string path, string? token)
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

    private static StringContent LOutpostHttpFormat(JsonObject body)
    {
        return new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json");
    }

    private static async Task<JsonNode?> LOutpostHttpParse(
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

    private static string? LOutpostHttpParse(JsonNode? node, string name)
    {
        return node is JsonObject entry && entry[name] is JsonValue value && value.TryGetValue(out string? text)
            ? text
            : null;
    }
}
