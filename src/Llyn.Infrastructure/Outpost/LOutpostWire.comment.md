# LOutpostWire.cs
Hash: `e6738c4739bec66c`

## `public sealed class LOutpostWire`

The HTTP transport under `LOutpostHttp`, which owns the one `HttpClient` for Joplin.
It speaks to the loopback address only, since Joplin listens nowhere else.
It knows requests, answers and JSON, never notes, folders or tags.

## `private static readonly HttpClient LOutpostWireClient`

The one client every call goes through, built once from an `HttpClientHandler`.
It never follows a redirect and never uses a proxy.
The token rides in the URL, so it must not reach any other address.
Its timeout is 100 seconds, so a large upload is not cut short.
The one-second ping bound per port is separate and stays.

## `private const string LOutpostWireBanner = "JoplinClipperServer";`

The exact text Joplin answers a ping with.
Another program on the same port answers something else and is skipped.

## `private const int LOutpostWirePage = 100;`

The largest page the Data API hands out, so a list takes the fewest round trips.

## `private static readonly TimeSpan LOutpostWirePatience = TimeSpan.FromSeconds(1);`

How long one port may take to answer a ping.
A loopback answer is near instant, so a slower port is treated as silent.
A search tries at most twelve ports, one second each.

## `public async Task<bool> LOutpostWireCheck(int port, CancellationToken cancellation)`

Whether one port answers a ping with Joplin's banner within the patience.
Its own timeout is linked to the caller's, so either one ends the wait.
Only a cancellation the caller asked for is thrown, every other failure reads as silent.

## `public async Task<HttpResponseMessage> LOutpostWireSend(HttpMethod method, string address, HttpContent? content, bool warranted, bool lenient, CancellationToken cancellation)`

Sends one request and hands back the answer when it succeeded.
A not-found answer is handed back too when `lenient`, for the calls that branch on it.
A forbidden answer to a tokened call throws the warrant refusal, so the caller asks for a new token.
Any other failed status throws `HttpRequestException` carrying that status.
A stalled or unreachable Joplin throws `TimeoutException` around the transport failure.
That covers the client's own timeout and any statusless `HttpRequestException`, such as a refused connection.
So a caller can tell a silent Joplin from one that answered with an error.
The caller's own cancellation is thrown through unchanged.

## `public async Task<List<JsonObject>> LOutpostWireRead(int port, string token, string path, CancellationToken cancellation)`

Reads every page of a list until Joplin says there are no more.
An answer that is not a list object ends the read with what was gathered.

## `public static string LOutpostWireFormat(int port, string path, string? token)`

The loopback address for `path`, with the escaped token appended when there is one.
A port outside the valid range throws before any request is built.

## `public static StringContent LOutpostWireFormat(JsonObject body)`

The JSON body of one request, sent as UTF-8.

## `public static async Task<JsonNode?> LOutpostWireParse(HttpResponseMessage response, CancellationToken cancellation)`

The answer read as JSON, or null when it is empty.
A malformed answer throws `HttpRequestException` around the `JsonException`.
`LOutpostHttp.LOutpostWarrantCheck` reads that failure as rejected, since a poll must end.

## `public static string? LOutpostWireParse(JsonNode? node, string name)`

The string field `name` of an object, or null when the node, the field or its type differs.
A well-formed answer missing the field therefore reads as null instead of throwing mid-push.
