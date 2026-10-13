# Outpost

This folder is the Joplin bridge's transport.
It sends Llyn's notes over Joplin's local Data API and keeps the manifest of pushed digests.
The wire and HTTP files carry the requests.
The seal checks every id before it enters a URL.

## `LManifestFile`

The adapter behind the manifest port.
It keeps the Joplin digests as JSON under the workspace root, through the keep, under the name `joplin`.
The shape holds a string `realm` and an object `notes` mapping each Joplin note id to its digest.
The two conversions are public so a test can run them on text.

### `LManifestRead`

A missing file reads as an empty manifest, and so does broken JSON or a wrong shape.
Broken JSON reads as empty on purpose, so a damaged file never blocks pushing.
The cost is that notes of entries deleted earlier are no longer trashed.
A file that exists but cannot be read raises `LVaultFault` around the file system's own exception.

### `LManifestSave`

It replaces the whole manifest kept under the name.
A keep that cannot be written raises `LVaultFault` around the file system's own exception.

### `LManifestFileParse`

An entry whose digest is not a string is skipped, so one bad entry costs one extra push only.
An entry whose id is not 32 lowercase hex characters is skipped too, so it can never be trashed.
A missing or non-string `realm` reads as empty, the mark of a manifest written before realms.
Text that is blank or not JSON reads as empty.

### `LManifestFileFormat`

Note ids are written in a fixed order, so an unchanged manifest writes the same bytes.
The realm is written beside them, so a manifest read in another workspace is known as foreign.
A rebuilt database or a foreign file must never drive trashing.

## `LOutpostSeal`

It is the one id check every id-bearing Joplin call goes through before it builds a URL.
It lives apart so the HTTP adapter stays short.
An id must be exactly 32 lowercase hex characters, the only form Joplin hands out or Llyn derives.
A null id is not a match.
Any other text could add a path segment or query and reach an item Llyn never created.
`LOutpostSealCheck` throws `ArgumentException` for a non-match.

## `LOutpostWire`

It is the HTTP transport under `LOutpostHttp`, owning the one `HttpClient` for Joplin.
It speaks to the loopback address only, since Joplin listens nowhere else.
It knows requests, answers and JSON, never notes, folders or tags.

### `LOutpostWireClient`

It never follows a redirect and never uses a proxy.
The token rides in the URL, so it must not reach any other address.
Its timeout is 100 seconds, so a large upload is not cut short.
The one-second ping bound per port is separate and stays.

### Constants

The banner is the exact text Joplin answers a ping with.
Another program on the same port answers something else and is skipped.
The page size of 100 is the largest the Data API hands out.
So a list takes the fewest round trips.
The one-second patience treats a slower port as silent, since a loopback answer is near instant.

### `LOutpostWireCheck`

Its own timeout is linked to the caller's, so either one ends the wait.
Only a cancellation the caller asked for is thrown, and every other failure reads as silent.

### `LOutpostWireSend`

It hands back the answer when the request succeeded.
A not-found answer is handed back too when lenient, for the calls that branch on it.
A forbidden answer to a tokened call throws the warrant refusal, so the caller asks for a new token.
Any other failed status throws `HttpRequestException` carrying that status.
A stalled or unreachable Joplin throws `TimeoutException`, including a refused connection.
So a caller can tell a silent Joplin from one that answered with an error.
The caller's own cancellation is thrown through unchanged.

### `LOutpostWireRead`

It reads every page of a list until Joplin says there are no more.
An answer that is not a list object ends the read with what was gathered.

### Format and parse helpers

`LOutpostWireFormat` builds the loopback address with the escaped token appended when there is one.
A port outside the valid range throws before any request is built.
The body overload sends JSON as UTF-8.
`LOutpostWireParse` on a response returns null when the answer is empty.
A malformed answer throws `HttpRequestException` around the `JsonException`.
`LOutpostHttp.LOutpostWarrantCheck` reads that failure as rejected, since a poll must end.
The field overload returns null when the node, field or type differs.
So a well-formed answer missing the field never throws mid-push.

## `LOutpostHttp`

It is the adapter behind the `LOutpost` port, sending every request through `LOutpostWire`.
It speaks Joplin's local Data API on the loopback address only.
Every token rides in the query string, the one place the Data API reads it from.
Every note, folder, resource and tag id placed in a URL passes `LOutpostSeal.LOutpostSealCheck` first.
So no id can bend a path toward an item Llyn never addressed.
It keeps only what Joplin's notes, folders, tags and resources mean.

### `LOutpostFind`

Ports 41184 to 41194 are Joplin's range, from its default port to the last it moves to.
The stored port is pinged first, then the range, and the first port that names Joplin wins.
A refused connection or a timeout on one port moves to the next.
An invalid stored port is skipped, so a bad setting still scans the range and never throws.
A search tries at most twelve ports.
The caller's own cancellation is thrown through, since the search is being abandoned.

### Warrant calls

`LOutpostWarrantStart` posts a token request and answers the ticket.
An answer without a ticket throws, since there would be nothing to poll.
`LOutpostWarrantCheck` maps the ticket's status to a warrant state.
An accepted answer without a token, an unknown status, an error status or malformed JSON reads as rejected.
Joplin forgets tickets it no longer knows, so a poll must end rather than wait forever.
A stalled or unreachable Joplin still throws `TimeoutException`, since Joplin may simply have closed.

### `LOutpostFolderSave`

It updates the notebook in place and creates it with the fixed id only when Joplin knows no such id.
Trying the update first keeps a repeated push to one call.
An id that is not 32 lowercase hex characters throws `ArgumentException` first.
Both calls send the parent in the body, never the URL, so it needs no id check.
The update sends a zero deletion time so a trashed notebook comes back.
That a zero restores the notebook is unverified against Joplin and needs a live check.

### `LOutpostNoteSave`

It overwrites the note in place and creates it with the fixed id only when Joplin knows no such id.
The note id and its folder id must both be valid, or it throws `ArgumentException`.
A new note is created with an empty body and then filled by the same update.
Joplin downloads every picture address in a created body into resources it owns.
Such copies of remote pictures, like video thumbnails, would be orphaned by the next push.
An update leaves the body as sent, so no such copy is ever made.
The update sends a zero deletion time so a trashed note comes back.
That a zero restores the note is unverified against Joplin and needs a live check.

### `LOutpostNoteRead`

It asks only for the title, body, notebook and deletion time, so the check stays one small call.
An invalid id or folder throws `ArgumentException` first.
A not-found answer reads as null, since the caller then simply creates the note.
A note outside the folder, or with a nonzero or unreadable deletion time, reads as null too.
So does a note with a different title.
Such a note was moved, trashed or renamed in Joplin, and the push puts it back.
The title compares ordinally.
A malformed answer still throws, so the entry fails rather than resending blindly.

### `LOutpostNoteRemove`

An invalid id throws `ArgumentException` first.
The folders are only compared, never placed in a URL, so they need no id check.
It reads the note's notebook, body and deletion time before anything is deleted.
It trashes only a note that sits untrashed in one of the folders with a body starting with the mark.
Only such a note is proven to be one Llyn wrote, so no other Joplin item is ever touched.
The delete goes without the permanent flag, so Joplin moves the note to its trash.
It answers true only when it trashed the note.
A missing, moved, trashed or foreign note answers false and sends no delete.

### `LOutpostTagSave`

It reads the note's current tags, keeps the asked ones and detaches every other.
The note id and every tag id it detaches or attaches must be valid, or it throws `ArgumentException`.
A current tag without an id is skipped.
Asked tags are trimmed and normalized to NFC, and blanks are dropped.
Case is kept in the titles Llyn sends.
Titles compare ignoring case after the same normalization, which also drops duplicates.
The tag list is read only when a tag is still missing from the note.
A missing tag is reused when one with that title exists and created otherwise, then attached.
A create that fails with an HTTP status re-reads the tag list once.
A tag that now matches is used, and otherwise the failure is rethrown.

### `LOutpostParcelSave`

It asks for the resource by id and uploads it only when Joplin answers not found.
A resource id that is not valid throws `ArgumentException` first.
The upload is a multipart form, the only shape Joplin accepts for resource bytes.
An unreadable media type falls back to plain bytes rather than failing the push.
The props also carry the media type, or Joplin would record every image as plain bytes.
The file name is the title without quotes, or the id when that is blank, since the form rejects both.

### Helpers

`LOutpostNoteMatch` shows a zero deletion time and a folder in the set.
A missing or unreadable deletion time fails the check, so doubt never reads as present.
The read and the trash share it, so both judge a note alike.
`LOutpostNoteBuild` builds the update payload once, so the overwrite and the fill after creation never differ.

## Stamp

Commit: `c2f335c331fe`
- `LManifestFile.cs` `ffcd72a145df49ae`
- `LOutpostHttp.cs` `6c88bc9ff86aedfd`
- `LOutpostSeal.cs` `29af3e9a4ce34b26`
- `LOutpostWire.cs` `e6738c4739bec66c`
