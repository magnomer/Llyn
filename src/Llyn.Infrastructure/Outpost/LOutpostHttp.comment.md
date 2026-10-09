# LOutpostHttp.cs
Hash: `6c88bc9ff86aedfd`

## `public sealed class LOutpostHttp : LOutpost`

The adapter behind the `LOutpost` port, which sends every request through `LOutpostWire`.
It speaks Joplin's local Data API on the loopback address only.
Joplin listens nowhere else, so no other host is ever contacted.
Every token rides in the query string, the one place the Data API reads it from.
Every note, folder, resource and tag id passes `LOutpostSeal.LOutpostSealCheck` before any URL is built.
So no id can bend a path toward an item Llyn never addressed.

## `private const int LOutpostFirst = 41184;`

Joplin's default port, the first it tries when it starts its server.

## `private const int LOutpostLast = 41194;`

The last port Joplin moves to when the ones before it are taken.

## `private readonly LOutpostWire _lOutpostHttpWire = new();`

The transport that owns the client, the ping and the JSON reading.
This type keeps only what Joplin's notes, folders, tags and resources mean.

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
A stalled or unreachable Joplin still throws `TimeoutException`, since Joplin may simply have closed.

## `public async Task LOutpostFolderSave(int port, string token, string id, string parent, string title, CancellationToken cancellation)`

Updates the notebook in place and creates it with the fixed id only when Joplin knows no such id.
An `id` that is not 32 lowercase hex characters throws `ArgumentException` first.
Trying the update first keeps a repeated push to one call.
Both the update and the creation send `parent` as the notebook's parent.
`parent` travels only in the body, never the URL, so it needs no id check.
The update sends a zero deletion time so a trashed notebook comes back.
That a zero restores the notebook is unverified against Joplin and needs a live check.

## `public async Task LOutpostNoteSave(int port, string token, LOutpostNote note, CancellationToken cancellation)`

Overwrites the note in place and creates it with the fixed id only when Joplin knows no such id.
The note id and its folder id must both be 32 lowercase hex characters, or it throws `ArgumentException`.
A new note is created with an empty body and then filled by the same update.
Joplin downloads every picture address in a created body into resources it owns.
Such copies of remote pictures, like video thumbnails, would be orphaned by the next push.
An update leaves the body as sent, so no such copy is ever made.
The update sends a zero deletion time so a trashed note comes back.
That a zero restores the note is unverified against Joplin and needs a live check.

## `public async Task<string?> LOutpostNoteRead(int port, string token, string id, string folder, string title, CancellationToken cancellation)`

Asks only for the title, body, notebook and deletion time, so the check stays one small call.
An `id` or `folder` that is not 32 lowercase hex characters throws `ArgumentException` first.
A not-found answer reads as null, since the caller then simply creates the note.
A note outside `folder`, or with any nonzero or unreadable deletion time, reads as null too.
A note whose title differs from `title` ordinally reads as null as well.
Such a note was moved, trashed or renamed in Joplin, and the push puts it back.
A malformed answer still throws, so the entry fails rather than resending blindly.

## `public async Task<bool> LOutpostNoteRemove(int port, string token, string id, IReadOnlySet<string> folders, string mark, CancellationToken cancellation)`

An `id` that is not 32 lowercase hex characters throws `ArgumentException` first.
`folders` are only compared, never placed in a URL, so they need no id check.
It reads the note's notebook, body and deletion time before anything is deleted.
It trashes only a note that sits untrashed in one of `folders` with a body starting with `mark`.
Only such a note is proven to be one Llyn wrote, so no other Joplin item is ever touched.
The delete goes without the permanent flag, so Joplin moves the note to its trash.
It answers true only when it trashed the note.
A missing, moved, trashed or foreign note answers false and sends no delete.

## `public async Task LOutpostTagSave(int port, string token, string id, IReadOnlyList<string> tags, CancellationToken cancellation)`

Reads the note's current tags, keeps the asked ones and detaches every other.
The note id and every tag id Joplin answers must be 32 lowercase hex characters, or it throws `ArgumentException`.
Asked tags are trimmed and normalized to NFC, and blanks are dropped.
Case is kept in the titles Llyn sends.
Titles compare with `StringComparison.OrdinalIgnoreCase` after the same normalization, which also drops duplicates.
The tag list is read only when a tag is still missing from the note.
A missing tag is reused when one with that title exists and created otherwise, then attached.
A create that fails with an HTTP status re-reads the tag list once.
A tag that now matches is used, and otherwise the failure is rethrown.

## `public async Task LOutpostParcelSave(int port, string token, LParcel parcel, CancellationToken cancellation)`

Asks for the resource by id and uploads it only when Joplin answers not found.
A resource id that is not 32 lowercase hex characters throws `ArgumentException` first.
The upload is a multipart form, the only shape Joplin accepts for resource bytes.
An unreadable media type falls back to plain bytes rather than failing the push.
The props also carry the media type, or Joplin would record every image as plain bytes.
The file name is the title without quotes, or the id when that is blank, since the form rejects both.

## `private static string? LOutpostHttpFind(IReadOnlyList<JsonObject> tags, string title)`

The id of the tag whose normalized title matches `title` ignoring case, or null when none does.

## `private static bool LOutpostNoteMatch(JsonNode? answer, IReadOnlySet<string> folders)`

Whether a note answer shows a zero deletion time and sits in one of `folders`.
The read passes its one folder as a set of one.
A missing or unreadable deletion time fails the check, so doubt never reads as present.
The read and the trash share it, so both judge a note alike.

## `private static HttpContent LOutpostNoteBuild(LOutpostNote note)`

The update payload, built once so the plain overwrite and the fill after creation never differ.
