# LOutpost.cs
Hash: `6d6633226cbdb55d`

## `public interface LOutpost`

The port for Joplin's local Data API, which receives Llyn's entries one way.
Joplin is an outside program reached over HTTP, so the engine names neither the client nor the address.
Every call takes the port first, because Joplin may answer on a port other than its default.
Every call carrying a token throws `LRefusal` with `LRefusalWarrant` when Joplin refuses the token.
So the caller asks for a new token instead of failing each note.
Every call takes a cancellation last, so a slow or stalled Joplin never holds a push open.
Every call but `LOutpostFind` throws `TimeoutException` for a stalled or unreachable Joplin.
So the caller can stop instead of retrying each note.

## `Task<int?> LOutpostFind(int port, CancellationToken cancellation);`

Finds the port Joplin answers on, trying `port` before any other.
It answers null when Joplin is not running, so the caller can say so instead of failing.

## `Task<string> LOutpostWarrantStart(int port, CancellationToken cancellation);`

Asks Joplin for a token and answers with the ticket of the pending request.
The user must accept that request inside Joplin before any token exists.

## `Task<LWarrantAnswer> LOutpostWarrantCheck(int port, string ticket, CancellationToken cancellation);`

Polls the ticket from `LOutpostWarrantStart` for the user's decision.
The token arrives only in an accepted answer.

## `Task LOutpostFolderSave(int port, string token, string id, string title, CancellationToken cancellation);`

Creates or updates the notebook with that fixed id.
A fixed id lets every push land in the same notebook without searching by title.

## `Task LOutpostNoteSave(int port, string token, LOutpostNote note, CancellationToken cancellation);`

Creates or overwrites the note with that fixed id.
Llyn owns the note, so any edit made inside Joplin is replaced.
The save also brings a note back from Joplin's trash and back into its notebook.
The push is one way, so Llyn's copy wins.

## `Task<string?> LOutpostNoteRead(int port, string token, string id, string folder, string title, CancellationToken cancellation);`

The body Joplin holds for that note, but only while it sits untrashed in `folder` under `title`.
It answers null for a missing, moved, renamed or trashed note, so the caller sends it again.
That lets a push repair edits made inside Joplin, or a fresh Joplin profile.

## `Task<bool> LOutpostNoteRemove(int port, string token, string id, CancellationToken cancellation);`

Moves the note to Joplin's trash rather than deleting it.
It answers false when Joplin no longer knew the note, so the caller does not count it.
The user can still recover a note whose entry was removed by mistake.

## `Task LOutpostTagSave(int port, string token, string id, IReadOnlyList<string> tags, CancellationToken cancellation);`

Makes the note's tags exactly `tags`, adding missing ones and dropping the rest.

## `Task LOutpostParcelSave(int port, string token, LParcel parcel, CancellationToken cancellation);`

Uploads an attachment unless one with its id already exists.
Attachments rarely change, so skipping known ids keeps a repeated push cheap.
