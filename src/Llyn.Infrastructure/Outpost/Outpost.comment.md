# Outpost

This folder is the embassy module: Llyn's notes sent to Joplin over its local Data API.
Only `LEmbassyEntrance` is public.
The pairing, the batch, the shelf, the HTTP adapter, the wire, the seal and the manifest are internal.
The seal checks every id before it enters a URL and derives every id and digest Llyn sends.

## `LEmbassyEntrance`

The module's entrance, and the rig's `LOutpost`.
It implements the port explicitly, so its own public surface is the constructors.
One takes the keep and the engraver and uses the wire's shared client.
The other takes a client too, so a test can stand a stub handler in front of Joplin.
The two halves it builds are the pairing, which asks for a token, and the batch, which sends notes.
Both share one HTTP adapter over one wire.

## `LManifest`

The Joplin digests the last push kept, with the realm that wrote them.
The digests map each note id to the digest of what was sent.
The realm names the workspace that pushed them.

## `LOutpostPairing`

It attaches Llyn to a running Joplin and answers the token the user accepted.
It finds the port first and refuses with the outpost refusal when no Joplin answers.
A failure of the token request is reported to the fault callback and refused the same way.
A refusal or a cancellation passes through unchanged.
It then polls the ticket once a second, at most 120 times.
An accepted answer returns its token, and one without a token is the warrant refusal.
A rejected answer is the warrant refusal.
A poll that runs out is the pending refusal.
A poll that times out is reported and counted, and three in a row refuse with the outpost refusal.
Any other answer resets that count.

## `LOutpostBatch`

It sends one cargo of entries and writes the manifest of what Joplin now holds.
Every notebook, note and style id derives from a fixed seed through the seal.
So a repeated push addresses the same items.
A notebook per language sits under the root notebook, beside a system notebook that holds the style note.
The style note is sent first, and the other notes import it by the mark the engraver reads.
Each language is read through the cargo's sound callback.
A language that fails to read is reported and sets a flag that stops the trash pass.
So a gap never trashes notes.
Language shelves are sent before the entries.
So an entry can link to a stem or diwei note only when that note was answered.
Each entry is read through the cargo's page callback and rendered by the engraver.
It is sent with the tags the draft names.
A failed entry is reported and its headword joins the failed list.
Three timeouts in a row refuse with the outpost refusal.
Any other result resets that count.

A note is sent only when its digest changed or the body Joplin holds differs.
The manifest digest is compared first, then the stored body is read back.
So a note moved, trashed or edited in Joplin is put back.
A kept note still has its tags saved.
After the sends, every manifest id no send touched is trashed.
The trash pass is skipped when a language was unread.
A manifest written by another workspace realm is dropped first, so a foreign manifest never drives trashing.
The manifest is saved in a `finally`, so a refused or cancelled run still records what it pushed.
The manifest read and save hold one lock.

The receipt counts the notes saved, kept and trashed, and names the failures.
Tags are the language and each card tag, under the prefix `llyn/`, collected through the card tree in sorted order.

## `LOutpostShelf`

It sends the stem and diwei notes of one language beneath its notebook.
Four notebook ids derive from fixed seeds and the language name.
They are the stems notebook and the diwei notebook, which holds the tone notes.
The diwei notebook has two children, for initials and for rimes.
Only a notebook that will hold a note is saved.
A timeout while saving them refuses with the outpost refusal.
A note whose link id is empty is dropped, since nothing addressed it.
`LOutpostShelfBuild` derives each link id from the stamp, the language, the kind and the page key.
It answers an empty id for an unknown key.
Each note is sent through a callback of the batch, so its digest, parcels and tags follow the batch's rules.
A failed note is reported and its title joins the failed list.
Three timeouts in a row refuse with the outpost refusal.

## `LManifestFile`

The store of the Joplin manifest, internal to this folder.
It keeps the digests as JSON under the workspace root, through the keep, under the name `joplin`.
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
It also derives the ids and digests Llyn sends.
An id must be exactly 32 lowercase hex characters, the only form Joplin hands out or Llyn derives.
A null id is not a match.
Any other text could add a path segment or query and reach an item Llyn never created.
`LOutpostSealCheck` throws `ArgumentException` for a non-match.
`LOutpostSealFormat` of a seed is the first 32 lowercase hex digits of its SHA-256.
So a derived id passes the check.
The overload for a note is the digest of the title, folder, body, sorted tags and parcel ids.
So an unchanged note keeps its digest.

## `LOutpostWire`

It is the HTTP transport under `LOutpostHttp`, over one `HttpClient` handed to its constructor.
It speaks to the loopback address only, since Joplin listens nowhere else.
It knows requests, answers and JSON, never notes, folders or tags.

### `LOutpostWireClient`

The shared client is internal, so the entrance can hand it to a wire.
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

It is the Joplin adapter under the batch and the pairing.
It sends every request through the `LOutpostWire` handed to its constructor.
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

Commit: `1f3467bbe5c6`
- `LEmbassyEntrance.cs` `d59c5776cf171b26`
- `LManifest.cs` `b8500c03d8ce0900`
- `LManifestFile.cs` `92f7f4bd435aae17`
- `LOutpostBatch.cs` `002ecd7fd431f8d5`
- `LOutpostHttp.cs` `b19d69b41dd7366f`
- `LOutpostPairing.cs` `1d54871d02d64c6f`
- `LOutpostSeal.cs` `f54da07b3f29c1ae`
- `LOutpostShelf.cs` `41e82ee4693290a2`
- `LOutpostWire.cs` `a2e66b3a8245bb2a`
