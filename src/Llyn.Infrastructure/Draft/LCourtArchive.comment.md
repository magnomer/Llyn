# LCourtArchive.cs

## `public static class LCourtArchive`

Keeps the court as plain files in `drafts/court`, one file per tentative link named after its id.
A translation may name an entry the user has not written yet, and that link cannot be a database row.
Writing a real row and deleting it on discard leaks rows whenever the program dies first.
A link waits here instead until the record it points at becomes real or is dropped.
A link to an entry that already exists is never written here, because an ordinary id needs no register.
Nothing here reaches SQLite and nothing here edits a draft file.
Two copies of the program may run against one workspace, so every operation touches a single named file and holds nothing open.

## `public static void LCourtArchiveSave(string root, LCourtLink link)`

Writes `link` to `drafts/court/<LCourtLinkId>.json`, replacing whatever was there.
The text goes to a `.json.tmp` file first and is then moved over the target.
A move is atomic, so a reader never sees a half-written link and a crash mid-write leaves the previous file intact.

## `public static LCourtLink? LCourtArchiveRead(string root, string id)`

The link stored under `id`, or `null` when no readable file holds it.
A missing file and an unreadable one are the same answer to the caller.

## `public static IReadOnlyList<LCourtLink> LCourtArchiveScan(string root)`

Every link the court holds.
A file that fails to parse or cannot be opened is skipped rather than thrown.
Recovery lists leftovers after a crash, which is exactly when a truncated file is likely.
One bad file must not hide the rest.

## `public static void LCourtArchiveDelete(string root, string id)`

Removes the file for `id`, which is how one link leaves the court on its own.
A file already gone, or held open by the other copy of the program, is not an error.

## `public static IReadOnlyList<LCourtLink> LCourtArchiveResolve(string root, string draftId, string realId)`

Settles every link pointing at `draftId` now that the draft has been saved as the real entry `realId`.
The link files disappear and the links themselves are returned.
Each returned link names the draft that owns it, so the caller can rewrite that draft's stored content to point at `realId`.
Rewriting is not done here; job06 owns the draft file.

## `public static IReadOnlyList<LCourtLink> LCourtArchiveCancel(string root, string draftId)`

Settles every link pointing at `draftId` now that the draft has been abandoned.
The link files disappear and the links themselves are returned, so the caller can strip them from their owners' content.
It is the same settlement as resolution with nothing real to point at afterwards.

## Inline notes

### the extension check inside the listing

A Windows search pattern can match a file whose extension merely starts with the one asked for.
The half-written `.json.tmp` file must never be read as a link.

### `realId` being checked but not stored

The court file is deleted rather than rewritten, so the real id has nowhere to go here.
It is still demanded, because a caller resolving against a blank id has lost the entry it just saved and must fail loudly.
