using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TEmbassyFake : HttpMessageHandler
{
    private readonly Dictionary<string, string> _tEmbassyFakeTag = new(StringComparer.Ordinal);

    public Dictionary<string, (string TEmbassyFakeParent, string TEmbassyFakeTitle)> TEmbassyFakeFolder { get; }
        = [];

    public Dictionary<string, LOutpostNote> TEmbassyFakeNote { get; } = [];

    public List<string> TEmbassyFakeSaved { get; } = [];

    public List<string> TEmbassyFakeTrash { get; } = [];

    public HashSet<string> TEmbassyFakeRefusal { get; } = new(StringComparer.Ordinal);

    public HashSet<string> TEmbassyFakeSilence { get; } = new(StringComparer.Ordinal);

    public string TEmbassyFakeAnswer { get; init; } = "accepted";

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string[] path = request.RequestUri!.AbsolutePath.Trim('/').Split('/');
        JsonObject body = request.Content?.Headers.ContentType?.MediaType == "application/json"
            && JsonNode.Parse(await request.Content.ReadAsStringAsync(cancellationToken)) is JsonObject json
                ? json
                : new JsonObject();
        string id = path.Length > 1 ? path[1] : body["id"]?.GetValue<string>() ?? string.Empty;
        string title = body["title"]?.GetValue<string>() ?? string.Empty;
        string parent = body["parent_id"]?.GetValue<string>() ?? string.Empty;
        string text = body["body"]?.GetValue<string>() ?? string.Empty;
        HttpStatusCode missing = HttpStatusCode.NotFound;
        if (request.Method == HttpMethod.Put && path[0] == "notes" && TEmbassyFakeSilence.Contains(title))
        {
            throw new HttpRequestException("Joplin is unreachable.");
        }

        switch (request.Method.Method, path[0], path.Length)
        {
            case ("GET", "ping", 1):
                return TEmbassyFakeCreate(HttpStatusCode.OK, "JoplinClipperServer");
            case ("POST", "auth", 1):
                return TEmbassyFakeCreate(
                    HttpStatusCode.OK, new JsonObject { ["auth_token"] = "ticket" }.ToJsonString());
            case ("GET", "auth", 2):
                return TEmbassyFakeCreate(
                    HttpStatusCode.OK,
                    new JsonObject { ["status"] = TEmbassyFakeAnswer, ["token"] = "tok" }.ToJsonString());
            case ("PUT", "folders", 2) when !TEmbassyFakeFolder.ContainsKey(id):
                return TEmbassyFakeCreate(missing, "{}");
            case ("PUT", "folders", 2) or ("POST", "folders", 1):
                TEmbassyFakeFolder[id] = (parent, title);
                return TEmbassyFakeCreate(HttpStatusCode.OK, "{}");
            case ("PUT", "notes", 2) when TEmbassyFakeRefusal.Contains(title):
                return TEmbassyFakeCreate(HttpStatusCode.InternalServerError, "{}");
            case ("PUT", "notes", 2) when !TEmbassyFakeNote.ContainsKey(id):
                return TEmbassyFakeCreate(missing, "{}");
            case ("PUT", "notes", 2) or ("POST", "notes", 1):
                TEmbassyFakeNote[id] = new LOutpostNote(id, parent, title, text);
                if (request.Method == HttpMethod.Put)
                {
                    TEmbassyFakeSaved.Add(id);
                }

                return TEmbassyFakeCreate(HttpStatusCode.OK, "{}");
            case ("GET", "notes", 3) or ("GET", "tags", 1):
                JsonArray items = new();
                foreach ((string known, string tagged) in _tEmbassyFakeTag)
                {
                    if (path.Length == 1)
                    {
                        items.Add(new JsonObject { ["id"] = tagged, ["title"] = known });
                    }
                }

                return TEmbassyFakeCreate(
                    HttpStatusCode.OK, new JsonObject { ["items"] = items, ["has_more"] = false }.ToJsonString());
            case ("GET", "notes", 2):
                return TEmbassyFakeNote.TryGetValue(id, out LOutpostNote? note)
                    ? TEmbassyFakeCreate(HttpStatusCode.OK, new JsonObject
                    {
                        ["title"] = note.LOutpostNoteTitle,
                        ["body"] = note.LOutpostNoteBody,
                        ["parent_id"] = note.LOutpostNoteFolder,
                        ["deleted_time"] = 0,
                    }.ToJsonString())
                    : TEmbassyFakeCreate(missing, "{}");
            case ("DELETE", "notes", 2):
                TEmbassyFakeTrash.Add(id);
                return TEmbassyFakeCreate(TEmbassyFakeNote.Remove(id) ? HttpStatusCode.OK : missing, "{}");
            case ("POST", "tags", 1):
                string tagId = Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(title)))[..32];
                _tEmbassyFakeTag[title] = tagId;
                return TEmbassyFakeCreate(HttpStatusCode.OK, new JsonObject { ["id"] = tagId }.ToJsonString());
            case ("POST", "tags", 3) or ("DELETE", "tags", 4) or ("POST", "resources", 1):
                return TEmbassyFakeCreate(HttpStatusCode.OK, "{}");
            case ("GET", "resources", 2):
                return TEmbassyFakeCreate(missing, "{}");
            default:
                return TEmbassyFakeCreate(HttpStatusCode.BadRequest, "{}");
        }
    }

    private static HttpResponseMessage TEmbassyFakeCreate(HttpStatusCode status, string text)
    {
        return new HttpResponseMessage(status) { Content = new StringContent(text, Encoding.UTF8) };
    }
}
