# LCourtArchive.cs

## `public sealed class LCourtArchive : LCourtVault`

Keeps the court as plain files in `drafts/court`, one file per tentative link named after its id.
A translation may name an entry the user has not written yet, and that link cannot be a database row.
Writing a real row and deleting it on discard leaks rows whenever the program dies first.
A link waits here instead until the record it points at becomes real or is dropped.
A link to an entry that already exists is never written here, because an ordinary id needs no register.
Nothing here reaches SQLite and nothing here edits a draft file.
Two copies of the program may run against one workspace.
Every operation touches a single named file and holds nothing open.

## `public LCourtArchive(string root)`

Binds the archive to the workspace `root` whose draft folder it keeps the files in.

## `public void LCourtSave(LCourt link)`

Writes `link` to `drafts/court/<LCourtId>.json`, replacing whatever was there.
The text goes to a `.json.tmp` file first and is then moved over the target.
A move is atomic, so a reader never sees a half-written link.
A crash mid-write leaves the previous file intact.

## `public IReadOnlyList<LCourt> LCourtScan()`

Every link the court holds.
A file that fails to parse or cannot be opened is skipped rather than thrown.
Recovery lists leftovers after a crash, which is exactly when a truncated file is likely.
One bad file must not hide the rest.

## `public void LCourtDelete(long id)`

Removes the file for `id`, which is how one link leaves the court on its own.
A file already gone, or held open by the other copy of the program, is not an error.

## `public IReadOnlyList<LCourt> LCourtSettle(long draftId)`

Settles every link pointing at `draftId`, whether that draft became a real entry or was abandoned.
The link files disappear and the links themselves are returned.
Each returned link names the draft that owns it, so the caller can rewrite that draft's stored content.
Rewriting is not done here, because the engine owns the draft file.
Resolution and cancellation were two names for this one deletion.
The real id one of them demanded was never stored anywhere.
The caller already knows what to put in place of the tentative id.

## `public void LCourtSweep()`

Deletes every half-written `.json.tmp` in the court that is older than an hour.
It also deletes every link file of another version, or of no readable shape, which a read already skips.
A save writes its pending file and then moves it over the target.
A kill between the two leaves a file the listing rightly ignores and nothing collects.
The hour keeps a file another copy of the program is writing out of reach.
A missing folder is not an error, and neither is a file that vanishes mid-sweep.

## Inline notes

### the extension check inside the listing

A Windows search pattern can match a file whose extension merely starts with the one asked for.
The half-written `.json.tmp` file must never be read as a link.

### the age check inside the sweep

A pending file younger than the hour may belong to a save still in flight.
Taking it would break a write the other copy of the program is in the middle of.
