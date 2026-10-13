# Outpost

This folder holds the ports and records for the one-way push of entries to Joplin.
Joplin is an outside program reached over its local Data API, so Core names neither the client nor the address.
Llyn's copy always wins, and the manifest lets the next push send only what changed.

## `LOutpost`

The port for Joplin's local Data API, which receives Llyn's entries one way.
Every call takes the port first, because Joplin may answer on a port other than its default.
Every id is 32 lowercase hex characters, and any other id throws `ArgumentException`.
Every call carrying a token throws `LRefusal` with `LRefusalWarrant` when Joplin refuses the token.
So the caller asks for a new token instead of failing each note.
Every call takes a cancellation last, so a slow or stalled Joplin never holds a push open.
Every call but `LOutpostFind` throws `TimeoutException` for a stalled or unreachable Joplin.
So the caller can stop instead of retrying each note.

## `LOutpostFind`

Finds the port Joplin answers on, trying the given port before any other.
It answers null when Joplin is not running, so the caller can say so instead of failing.

## `LOutpostWarrantStart`

Asks Joplin for a token and answers with the ticket of the pending request.
The user must accept that request inside Joplin before any token exists.

## `LOutpostWarrantCheck`

Polls the ticket for the user's decision.
The token arrives only in an accepted answer.

## `LOutpostFolderSave`

Creates or updates the notebook with that fixed id, placed under the notebook `parent`.
An empty `parent` places it at the top level.
A fixed id lets every push land in the same notebook without searching by title.
The save also moves the notebook back under `parent` if the user moved it in Joplin.

## `LOutpostNoteSave`

Creates or overwrites the note with that fixed id.
Llyn owns the note, so any edit made inside Joplin is replaced.
The save also brings a note back from Joplin's trash and back into its notebook.
The push is one way, so Llyn's copy wins.

## `LOutpostNoteRead`

Answers the body Joplin holds, but only while the note sits untrashed in `folder` under `title`.
It answers null for a missing, moved, renamed or trashed note, so the caller sends it again.
That lets a push repair edits made inside Joplin, or a fresh Joplin profile.

## `LOutpostNoteRemove`

Moves the note to Joplin's trash rather than deleting it.
It trashes only a note proven to be Llyn's, untrashed in one of `folders` with a body starting with `mark`.
Llyn must never act on a Joplin item it did not create.
It answers true only when it trashed, so the caller counts nothing else.
The user can still recover a note whose entry was removed by mistake.

## `LOutpostTagSave`

Makes the note's tags exactly the given tags, adding missing ones and dropping the rest.

## `LOutpostParcelSave`

Uploads an attachment unless one with its id already exists.
Attachments rarely change, so skipping known ids keeps a repeated push cheap.

## `LOutpostNote`

One Joplin note as Llyn pushes it.
A stable id lets a later push overwrite the same note instead of adding a copy.
The body is Markdown.

## `LParcel`

One attachment pushed to Joplin as a resource that notes can link to.
Its id derives from the file content, so a changed image gets a new id.
The note body that references the id changes too, so the note is pushed again.
That is why `LOutpostParcelSave` may skip an id that already exists.
The id is fixed, so a note body can name it before the upload happens.

## `LManifest`

What Llyn last pushed to Joplin, kept so the next push sends only what changed.
The digest covers everything pushed for one note: title, notebook, body and the sorted tag set.
A change to any of them must push the note again.
A note whose digest matches is skipped.
A note id present here but absent from the workspace is trashed.
The realm binds the manifest to the workspace that wrote it.
A rebuilt database or a foreign file must never drive trashing.
The realm is in `Guid` "N" format, or empty when unknown.

## `LManifestVault`

The port that keeps the push manifest under the workspace root.
The manifest travels with the workspace, so a moved workspace remembers what it pushed.
Saving replaces the stored manifest whole.

## `LReceipt`

The tally of one push to Joplin, shown to the user when it ends.
A failed note does not stop the push, so failures are listed rather than raised.
A reconstruction note whose title itself failed is listed under its language name.
The saved count never includes the style note.

## `LWarrant`

The port that protects the Joplin token at rest for the current user.
Protection depends on the operating system, so a platform twin implements it.
Core stays free of platform protection code.

## `LWarrantHide`

Returns a protected, printable form of the token that settings can store as text.
It refuses a blank token, since there is nothing worth protecting.
It can throw when the platform cannot protect data, for example when the user profile is not loaded.

## `LWarrantRestore`

Returns the token that the hidden form protects.
It answers null when the token cannot be recovered, for another user, another machine or damaged text.
On another machine a roaming Windows profile may still recover it there.
A null tells the caller to ask Joplin for a fresh token.

## `LWarrantAnswer`

Joplin's reply to one poll of a pending token request.
The token is present only when the request was accepted.

## `LWarrantState`

Where a token request stands inside Joplin.
Waiting means the user has not answered yet, so the caller polls again.
Accepted means polling stops and the token is read.
Rejected means polling stops.
An unknown or expired ticket is reported as rejected, so polling never runs forever.

## Stamp

Commit: `c2f335c331fe`
- `LManifest.cs` `43c8b7aa272af521`
- `LManifestVault.cs` `7c0d122f72ebe66b`
- `LOutpost.cs` `39a63f3599c3707c`
- `LOutpostNote.cs` `54e940cc97fb43e6`
- `LParcel.cs` `1ad3b2f7ad138c3e`
- `LReceipt.cs` `a2abbd39386ba268`
- `LWarrant.cs` `836b51c37d0f22a2`
- `LWarrantAnswer.cs` `62ad2b48ebd61fe0`
- `LWarrantState.cs` `615afc9002387e31`
