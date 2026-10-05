# LOutpostHttp.cs
Hash: `e28113e358b8e277`

## `public sealed class LOutpostHttp : LOutpost`

The adapter behind the `LOutpost` port, bound to the shared `HttpClient` the rig builds it over.
It speaks Joplin's local Data API on the loopback address only.
Joplin listens nowhere else, so no other host is ever contacted.
Every token rides in the query string, the one place the Data API reads it from.

## `private const string LOutpostBanner = "JoplinClipperServer";`

The exact text Joplin answers a ping with.
Another program on the same port answers something else and is skipped.

## `private const int LOutpostFirst = 41184;`

Joplin's default port, the first it tries when it starts its server.

## `private const int LOutpostLast = 41194;`

The last port Joplin moves to when the ones before it are taken.

## `private const int LOutpostPage = 100;`

The largest page the Data API hands out, so a list takes the fewest round trips.

## `private static readonly TimeSpan LOutpostPatience = TimeSpan.FromSeconds(1);`

How long one port may take to answer a ping.
A loopback answer is near instant, so a slower port is treated as silent.
A search tries at most twelve ports, one second each.

## `public LOutpostHttp(HttpClient client)`

Binds the adapter to the `client` every call goes through.

## `public async Task<int?> LOutpostFind(int port, CancellationToken cancellation)`

Pings `port` first and then Joplin's own range, answering the first port that names Joplin.
A refused connection or a timeout on one port moves to the next.
An invalid stored `port` is skipped, so a bad setting still scans the range and never throws.
The caller's own cancellation is thrown through, since the search is being abandoned.

## `public async Task<string> LOutpostWarrantStart(int port, CancellationToken cancellation)`

Posts a token request and answers the ticket Joplin returns.
An answer without a ticket throws, since there would be nothing to poll.

## `public async Task<LWarrantAnswer> LOutpostWarrantCheck(int port, string ticket, CancellationToken cancellation)`

Reads the ticket's status and maps it to a warrant state.
An accepted answer without a token, any unknown status, an error status or malformed JSON reads as rejected.
Joplin forgets tickets it no longer knows, so a poll must end rather than wait forever.
A transport failure still throws, since Joplin may simply have closed.

## `public async Task LOutpostFolderSave(int port, string token, string id, string title, CancellationToken cancellation)`

Updates the notebook in place and creates it with the fixed id only when Joplin knows no such id.
Trying the update first keeps a repeated push to one call.
The update sends a zero deletion time so a trashed notebook comes back.
That a zero restores the notebook is unverified against Joplin and needs a live check.

## `public async Task LOutpostNoteSave(int port, string token, LOutpostNote note, CancellationToken cancellation)`

Overwrites the note in place and creates it with the fixed id only when Joplin knows no such id.
The update sends a zero deletion time so a trashed note comes back.
That a zero restores the note is unverified against Joplin and needs a live check.

## `public async Task LOutpostNoteRemove(int port, string token, string id, CancellationToken cancellation)`

Deletes without the permanent flag, so Joplin moves the note to its trash.
A note Joplin does not know is already gone, so a not-found answer is success.

## `public async Task LOutpostTagSave(int port, string token, string id, IReadOnlyList<string> tags, CancellationToken cancellation)`

Reads the note's current tags, keeps the asked ones and detaches every other.
Asked tags are trimmed and lowercased, blanks and duplicates dropped, since Joplin stores titles that way.
Titles then compare ordinally against that stored form, and new tags are created with it.
The tag list is read only when a tag is still missing from the note.
A missing tag is reused when one with that title exists and created otherwise, then attached.

## `public async Task LOutpostParcelSave(int port, string token, LParcel parcel, CancellationToken cancellation)`

Asks for the resource by id and uploads it only when Joplin answers not found.
The upload is a multipart form, the only shape Joplin accepts for resource bytes.
An unreadable media type falls back to plain bytes rather than failing the push.
The props also carry the media type, or Joplin would record every image as plain bytes.
The file name is the title without quotes, or the id when that is blank, since the form rejects both.

## `private async Task<bool> LOutpostHttpCheck(int port, CancellationToken cancellation)`

Whether one port answers a ping with Joplin's banner within the patience.
Its own timeout is linked to the caller's, so either one ends the wait.
Only a cancellation the caller asked for is thrown, every other failure reads as silent.

## `private async Task<HttpResponseMessage> LOutpostHttpSend(HttpMethod method, string address, HttpContent? content, bool warranted, bool lenient, CancellationToken cancellation)`

Sends one request and hands back the answer when it succeeded.
A not-found answer is handed back too when `lenient`, for the calls that branch on it.
A forbidden answer to a tokened call throws the warrant refusal, so the caller asks for a new token.
Any other failed status throws `HttpRequestException` carrying that status.
The client's own timeout throws `HttpRequestException` too, so a stalled Joplin fails like an unreachable one.
The caller's own cancellation is thrown through unchanged.

## `private async Task<List<JsonObject>> LOutpostHttpRead(int port, string token, string path, CancellationToken cancellation)`

Reads every page of a list until Joplin says there are no more.
An answer that is not a list object ends the read with what was gathered.

## `private static string? LOutpostHttpFind(IReadOnlyList<JsonObject> tags, string title)`

The id of the tag whose title matches the normalized title ordinally, or null when none does.

## `private static string LOutpostHttpFormat(int port, string path, string? token)`

The loopback address for `path`, with the escaped token appended when there is one.
A port outside the valid range throws before any request is built.

## `private static StringContent LOutpostHttpFormat(JsonObject body)`

The JSON body of one request, sent as UTF-8.

## `private static async Task<JsonNode?> LOutpostHttpParse(HttpResponseMessage response, CancellationToken cancellation)`

The answer read as JSON, or null when it is empty.
A malformed answer throws `HttpRequestException` around the `JsonException`, except in `LOutpostWarrantCheck`.
There it reads as rejected, since a poll must end.

## `private static string? LOutpostHttpParse(JsonNode? node, string name)`

The string field `name` of an object, or null when the node, the field or its type differs.
A well-formed answer missing the field therefore reads as null instead of throwing mid-push.
