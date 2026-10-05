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
    private static readonly HttpClient LOutpostClient = new(
        new HttpClientHandler { AllowAutoRedirect = false, UseProxy = false })
    {
        Timeout = TimeSpan.FromSeconds(100),
    };

    private const string LOutpostBanner = "JoplinClipperServer";

    private const int LOutpostFirst = 41184;

    private const int LOutpostLast = 41194;

    private const int LOutpostPage = 100;

    private static readonly TimeSpan LOutpostPatience = TimeSpan.FromSeconds(1);

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
        LOutpostSeal.LOutpostSealCheck(id);
        ArgumentNullException.ThrowIfNull(title);

        string address = LOutpostHttpFormat(port, "folders/" + Uri.EscapeDataString(id), token);
        StringContent folder = LOutpostHttpFormat(new JsonObject { ["title"] = title, ["deleted_time"] = 0 });
        using HttpResponseMessage put = await LOutpostHttpSend(
            HttpMethod.Put, address, folder, true, true, cancellation).ConfigureAwait(false);
        if (put.StatusCode != HttpStatusCode.NotFound)
        {
            return;
        }

        StringContent created = LOutpostHttpFormat(new JsonObject { ["id"] = id, ["title"] = title });
        using HttpResponseMessage post = await LOutpostHttpSend(
            HttpMethod.Post, LOutpostHttpFormat(port, "folders", token), created, true, false, cancellation)
            .ConfigureAwait(false);
    }

    public async Task LOutpostNoteSave(int port, string token, LOutpostNote note, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        ArgumentNullException.ThrowIfNull(note);
        LOutpostSeal.LOutpostSealCheck(note.LOutpostNoteId);
        LOutpostSeal.LOutpostSealCheck(note.LOutpostNoteFolder);

        string address = LOutpostHttpFormat(port, "notes/" + Uri.EscapeDataString(note.LOutpostNoteId), token);
        using HttpResponseMessage put = await LOutpostHttpSend(
            HttpMethod.Put, address, LOutpostNoteBuild(note), true, true, cancellation).ConfigureAwait(false);
        if (put.StatusCode != HttpStatusCode.NotFound)
        {
            return;
        }

        StringContent blank = LOutpostHttpFormat(new JsonObject
        {
            ["id"] = note.LOutpostNoteId,
            ["title"] = note.LOutpostNoteTitle,
            ["body"] = string.Empty,
            ["parent_id"] = note.LOutpostNoteFolder,
        });
        using HttpResponseMessage post = await LOutpostHttpSend(
            HttpMethod.Post, LOutpostHttpFormat(port, "notes", token), blank, true, false, cancellation)
            .ConfigureAwait(false);

        using HttpResponseMessage fill = await LOutpostHttpSend(
            HttpMethod.Put, address, LOutpostNoteBuild(note), true, false, cancellation).ConfigureAwait(false);
    }

    public async Task<string?> LOutpostNoteRead(
        int port, string token, string id, string folder, string title, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        LOutpostSeal.LOutpostSealCheck(id);
        LOutpostSeal.LOutpostSealCheck(folder);
        ArgumentNullException.ThrowIfNull(title);

        string path = "notes/" + Uri.EscapeDataString(id) + "?fields=title,body,parent_id,deleted_time";
        using HttpResponseMessage response = await LOutpostHttpSend(
            HttpMethod.Get, LOutpostHttpFormat(port, path, token), null, true, true, cancellation)
            .ConfigureAwait(false);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        JsonNode? answer = await LOutpostHttpParse(response, cancellation).ConfigureAwait(false);
        return LOutpostNoteMatch(answer, folder)
            && string.Equals(LOutpostHttpParse(answer, "title"), title, StringComparison.Ordinal)
            ? LOutpostHttpParse(answer, "body")
            : null;
    }

    public async Task<bool> LOutpostNoteRemove(
        int port, string token, string id, string folder, string mark, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        LOutpostSeal.LOutpostSealCheck(id);
        LOutpostSeal.LOutpostSealCheck(folder);
        ArgumentException.ThrowIfNullOrEmpty(mark);

        string path = "notes/" + Uri.EscapeDataString(id);
        string fields = path + "?fields=parent_id,body,deleted_time";
        using HttpResponseMessage read = await LOutpostHttpSend(
            HttpMethod.Get, LOutpostHttpFormat(port, fields, token), null, true, true, cancellation)
            .ConfigureAwait(false);
        if (read.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        JsonNode? answer = await LOutpostHttpParse(read, cancellation).ConfigureAwait(false);
        if (!LOutpostNoteMatch(answer, folder)
            || LOutpostHttpParse(answer, "body")?.StartsWith(mark, StringComparison.Ordinal) != true)
        {
            return false;
        }

        using HttpResponseMessage response = await LOutpostHttpSend(
            HttpMethod.Delete, LOutpostHttpFormat(port, path, token), null, true, true, cancellation)
            .ConfigureAwait(false);
        return response.StatusCode != HttpStatusCode.NotFound;
    }

    public async Task LOutpostTagSave(
        int port, string token, string id, IReadOnlyList<string> tags, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        LOutpostSeal.LOutpostSealCheck(id);
        ArgumentNullException.ThrowIfNull(tags);

        string note = Uri.EscapeDataString(id);
        HashSet<string> missing = new(StringComparer.OrdinalIgnoreCase);
        foreach (string tag in tags)
        {
            if (!string.IsNullOrWhiteSpace(tag))
            {
                missing.Add(tag.Trim().Normalize(NormalizationForm.FormC));
            }
        }

        List<JsonObject> current = await LOutpostHttpRead(port, token, "notes/" + note + "/tags", cancellation)
            .ConfigureAwait(false);
        foreach (JsonObject tag in current)
        {
            string? tagId = LOutpostHttpParse(tag, "id");
            string? title = LOutpostHttpParse(tag, "title");
            string? kept = title?.Trim().Normalize(NormalizationForm.FormC);
            if (string.IsNullOrEmpty(tagId) || (kept is not null && missing.Remove(kept)))
            {
                continue;
            }

            LOutpostSeal.LOutpostSealCheck(tagId);
            string detach = "tags/" + Uri.EscapeDataString(tagId) + "/notes/" + note;
            using HttpResponseMessage gone = await LOutpostHttpSend(
                HttpMethod.Delete, LOutpostHttpFormat(port, detach, token), null, true, true, cancellation)
                .ConfigureAwait(false);
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
                try
                {
                    StringContent named = LOutpostHttpFormat(new JsonObject { ["title"] = title });
                    using HttpResponseMessage create = await LOutpostHttpSend(
                        HttpMethod.Post, LOutpostHttpFormat(port, "tags", token), named, true, false, cancellation)
                        .ConfigureAwait(false);
                    JsonNode? created = await LOutpostHttpParse(create, cancellation).ConfigureAwait(false);
                    tagId = LOutpostHttpParse(created, "id");
                }
                catch (HttpRequestException exception) when (exception.StatusCode is not null)
                {
                    known = await LOutpostHttpRead(port, token, "tags", cancellation).ConfigureAwait(false);
                    tagId = LOutpostHttpFind(known, title);
                    if (tagId is null)
                    {
                        throw;
                    }
                }

                if (string.IsNullOrEmpty(tagId))
                {
                    throw new HttpRequestException("Joplin created a tag without answering its id.");
                }
            }

            LOutpostSeal.LOutpostSealCheck(tagId);
            string attach = "tags/" + Uri.EscapeDataString(tagId) + "/notes";
            using HttpResponseMessage joined = await LOutpostHttpSend(HttpMethod.Post,
                LOutpostHttpFormat(port, attach, token), LOutpostHttpFormat(new JsonObject { ["id"] = id }),
                true, false, cancellation).ConfigureAwait(false);
        }
    }

    public async Task LOutpostParcelSave(int port, string token, LParcel parcel, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        ArgumentNullException.ThrowIfNull(parcel);
        LOutpostSeal.LOutpostSealCheck(parcel.LParcelId);
        ArgumentNullException.ThrowIfNull(parcel.LParcelBytes);

        string resource = "resources/" + Uri.EscapeDataString(parcel.LParcelId);
        using HttpResponseMessage known = await LOutpostHttpSend(
            HttpMethod.Get, LOutpostHttpFormat(port, resource, token), null, true, true, cancellation)
            .ConfigureAwait(false);
        if (known.StatusCode != HttpStatusCode.NotFound)
        {
            return;
        }

        ByteArrayContent data = new(parcel.LParcelBytes);
        data.Headers.ContentType =
            MediaTypeHeaderValue.TryParse(parcel.LParcelMime, out MediaTypeHeaderValue? mime)
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
            using HttpResponseMessage response = await LOutpostClient.SendAsync(request, limit.Token)
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
            response = await LOutpostClient.SendAsync(request, cancellation).ConfigureAwait(false);
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
            string? found = LOutpostHttpParse(tag, "title")?.Trim().Normalize(NormalizationForm.FormC);
            if (string.Equals(found, title, StringComparison.OrdinalIgnoreCase))
            {
                return LOutpostHttpParse(tag, "id");
            }
        }

        return null;
    }

    private static bool LOutpostNoteMatch(JsonNode? answer, string folder)
    {
        return answer is JsonObject note && note["deleted_time"] is JsonValue time
            && time.TryGetValue(out long deleted) && deleted == 0
            && string.Equals(LOutpostHttpParse(answer, "parent_id"), folder, StringComparison.Ordinal);
    }

    private static HttpContent LOutpostNoteBuild(LOutpostNote note)
    {
        return LOutpostHttpFormat(new JsonObject
        {
            ["title"] = note.LOutpostNoteTitle,
            ["body"] = note.LOutpostNoteBody,
            ["parent_id"] = note.LOutpostNoteFolder,
            ["deleted_time"] = 0,
        });
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
