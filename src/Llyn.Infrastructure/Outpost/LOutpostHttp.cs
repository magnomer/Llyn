using System;
using System.Collections.Generic;
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
    private const int LOutpostFirst = 41184;

    private const int LOutpostLast = 41194;

    private readonly LOutpostWire _lOutpostHttpWire = new();

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
            if (await _lOutpostHttpWire.LOutpostWireCheck(candidate, cancellation).ConfigureAwait(false))
            {
                return candidate;
            }
        }

        return null;
    }

    public async Task<string> LOutpostWarrantStart(int port, CancellationToken cancellation)
    {
        using HttpResponseMessage response = await _lOutpostHttpWire.LOutpostWireSend(
            HttpMethod.Post, LOutpostWire.LOutpostWireFormat(port, "auth", null), null, false, false, cancellation)
            .ConfigureAwait(false);
        JsonNode? answer = await LOutpostWire.LOutpostWireParse(response, cancellation).ConfigureAwait(false);
        string? ticket = LOutpostWire.LOutpostWireParse(answer, "auth_token");
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
            using HttpResponseMessage response = await _lOutpostHttpWire.LOutpostWireSend(
                HttpMethod.Get, LOutpostWire.LOutpostWireFormat(port, path, null), null, false, false, cancellation)
                .ConfigureAwait(false);
            JsonNode? answer = await LOutpostWire.LOutpostWireParse(response, cancellation).ConfigureAwait(false);
            string? token = LOutpostWire.LOutpostWireParse(answer, "token");
            return LOutpostWire.LOutpostWireParse(answer, "status") switch
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
        int port, string token, string id, string parent, string title, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        LOutpostSeal.LOutpostSealCheck(id);
        ArgumentNullException.ThrowIfNull(parent);
        ArgumentNullException.ThrowIfNull(title);

        string address = LOutpostWire.LOutpostWireFormat(port, "folders/" + Uri.EscapeDataString(id), token);
        StringContent folder = LOutpostWire.LOutpostWireFormat(
            new JsonObject { ["title"] = title, ["parent_id"] = parent, ["deleted_time"] = 0 });
        using HttpResponseMessage put = await _lOutpostHttpWire.LOutpostWireSend(
            HttpMethod.Put, address, folder, true, true, cancellation).ConfigureAwait(false);
        if (put.StatusCode != HttpStatusCode.NotFound)
        {
            return;
        }

        StringContent created = LOutpostWire.LOutpostWireFormat(
            new JsonObject { ["id"] = id, ["title"] = title, ["parent_id"] = parent });
        using HttpResponseMessage post = await _lOutpostHttpWire.LOutpostWireSend(
            HttpMethod.Post, LOutpostWire.LOutpostWireFormat(port, "folders", token), created, true, false,
            cancellation).ConfigureAwait(false);
    }

    public async Task LOutpostNoteSave(int port, string token, LOutpostNote note, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        ArgumentNullException.ThrowIfNull(note);
        LOutpostSeal.LOutpostSealCheck(note.LOutpostNoteId);
        LOutpostSeal.LOutpostSealCheck(note.LOutpostNoteFolder);

        string address = LOutpostWire.LOutpostWireFormat(
            port, "notes/" + Uri.EscapeDataString(note.LOutpostNoteId), token);
        using HttpResponseMessage put = await _lOutpostHttpWire.LOutpostWireSend(
            HttpMethod.Put, address, LOutpostNoteBuild(note), true, true, cancellation).ConfigureAwait(false);
        if (put.StatusCode != HttpStatusCode.NotFound)
        {
            return;
        }

        StringContent blank = LOutpostWire.LOutpostWireFormat(new JsonObject
        {
            ["id"] = note.LOutpostNoteId,
            ["title"] = note.LOutpostNoteTitle,
            ["body"] = string.Empty,
            ["parent_id"] = note.LOutpostNoteFolder,
        });
        using HttpResponseMessage post = await _lOutpostHttpWire.LOutpostWireSend(
            HttpMethod.Post, LOutpostWire.LOutpostWireFormat(port, "notes", token), blank, true, false, cancellation)
            .ConfigureAwait(false);

        using HttpResponseMessage fill = await _lOutpostHttpWire.LOutpostWireSend(
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
        using HttpResponseMessage response = await _lOutpostHttpWire.LOutpostWireSend(
            HttpMethod.Get, LOutpostWire.LOutpostWireFormat(port, path, token), null, true, true, cancellation)
            .ConfigureAwait(false);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        JsonNode? answer = await LOutpostWire.LOutpostWireParse(response, cancellation).ConfigureAwait(false);
        return LOutpostNoteMatch(answer, new HashSet<string>(StringComparer.Ordinal) { folder })
            && string.Equals(LOutpostWire.LOutpostWireParse(answer, "title"), title, StringComparison.Ordinal)
            ? LOutpostWire.LOutpostWireParse(answer, "body")
            : null;
    }

    public async Task<bool> LOutpostNoteRemove(
        int port, string token, string id, IReadOnlySet<string> folders, string mark, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        LOutpostSeal.LOutpostSealCheck(id);
        ArgumentNullException.ThrowIfNull(folders);
        ArgumentException.ThrowIfNullOrEmpty(mark);

        string path = "notes/" + Uri.EscapeDataString(id);
        string fields = path + "?fields=parent_id,body,deleted_time";
        using HttpResponseMessage read = await _lOutpostHttpWire.LOutpostWireSend(
            HttpMethod.Get, LOutpostWire.LOutpostWireFormat(port, fields, token), null, true, true, cancellation)
            .ConfigureAwait(false);
        if (read.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        JsonNode? answer = await LOutpostWire.LOutpostWireParse(read, cancellation).ConfigureAwait(false);
        if (!LOutpostNoteMatch(answer, folders)
            || LOutpostWire.LOutpostWireParse(answer, "body")?.StartsWith(mark, StringComparison.Ordinal) != true)
        {
            return false;
        }

        using HttpResponseMessage response = await _lOutpostHttpWire.LOutpostWireSend(
            HttpMethod.Delete, LOutpostWire.LOutpostWireFormat(port, path, token), null, true, true, cancellation)
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

        List<JsonObject> current = await _lOutpostHttpWire.LOutpostWireRead(
            port, token, "notes/" + note + "/tags", cancellation).ConfigureAwait(false);
        foreach (JsonObject tag in current)
        {
            string? tagId = LOutpostWire.LOutpostWireParse(tag, "id");
            string? title = LOutpostWire.LOutpostWireParse(tag, "title");
            string? kept = title?.Trim().Normalize(NormalizationForm.FormC);
            if (string.IsNullOrEmpty(tagId) || (kept is not null && missing.Remove(kept)))
            {
                continue;
            }

            LOutpostSeal.LOutpostSealCheck(tagId);
            string detach = "tags/" + Uri.EscapeDataString(tagId) + "/notes/" + note;
            using HttpResponseMessage gone = await _lOutpostHttpWire.LOutpostWireSend(
                HttpMethod.Delete, LOutpostWire.LOutpostWireFormat(port, detach, token), null, true, true,
                cancellation).ConfigureAwait(false);
        }

        if (missing.Count == 0)
        {
            return;
        }

        List<JsonObject> known = await _lOutpostHttpWire.LOutpostWireRead(port, token, "tags", cancellation)
            .ConfigureAwait(false);
        foreach (string title in missing)
        {
            string? tagId = LOutpostHttpFind(known, title);
            if (tagId is null)
            {
                try
                {
                    StringContent named = LOutpostWire.LOutpostWireFormat(new JsonObject { ["title"] = title });
                    using HttpResponseMessage create = await _lOutpostHttpWire.LOutpostWireSend(
                        HttpMethod.Post, LOutpostWire.LOutpostWireFormat(port, "tags", token), named, true, false,
                        cancellation).ConfigureAwait(false);
                    JsonNode? created = await LOutpostWire.LOutpostWireParse(create, cancellation)
                        .ConfigureAwait(false);
                    tagId = LOutpostWire.LOutpostWireParse(created, "id");
                }
                catch (HttpRequestException exception) when (exception.StatusCode is not null)
                {
                    known = await _lOutpostHttpWire.LOutpostWireRead(port, token, "tags", cancellation)
                        .ConfigureAwait(false);
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
            using HttpResponseMessage joined = await _lOutpostHttpWire.LOutpostWireSend(HttpMethod.Post,
                LOutpostWire.LOutpostWireFormat(port, attach, token),
                LOutpostWire.LOutpostWireFormat(new JsonObject { ["id"] = id }),
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
        using HttpResponseMessage known = await _lOutpostHttpWire.LOutpostWireSend(
            HttpMethod.Get, LOutpostWire.LOutpostWireFormat(port, resource, token), null, true, true, cancellation)
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

        using HttpResponseMessage post = await _lOutpostHttpWire.LOutpostWireSend(
            HttpMethod.Post, LOutpostWire.LOutpostWireFormat(port, "resources", token), form, true, false,
            cancellation).ConfigureAwait(false);
    }

    private static string? LOutpostHttpFind(IReadOnlyList<JsonObject> tags, string title)
    {
        foreach (JsonObject tag in tags)
        {
            string? found = LOutpostWire.LOutpostWireParse(tag, "title")?.Trim().Normalize(NormalizationForm.FormC);
            if (string.Equals(found, title, StringComparison.OrdinalIgnoreCase))
            {
                return LOutpostWire.LOutpostWireParse(tag, "id");
            }
        }

        return null;
    }

    private static bool LOutpostNoteMatch(JsonNode? answer, IReadOnlySet<string> folders)
    {
        return answer is JsonObject note && note["deleted_time"] is JsonValue time
            && time.TryGetValue(out long deleted) && deleted == 0
            && LOutpostWire.LOutpostWireParse(answer, "parent_id") is string folder && folders.Contains(folder);
    }

    private static HttpContent LOutpostNoteBuild(LOutpostNote note)
    {
        return LOutpostWire.LOutpostWireFormat(new JsonObject
        {
            ["title"] = note.LOutpostNoteTitle,
            ["body"] = note.LOutpostNoteBody,
            ["parent_id"] = note.LOutpostNoteFolder,
            ["deleted_time"] = 0,
        });
    }
}
