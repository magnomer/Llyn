# LDraftArchive.cs

## `public static class LDraftArchive`

Keeps tentative records as plain files in the workspace `drafts` folder, one file per draft named after its id.
Nothing here reaches SQLite.
A record the user has not committed must never appear in the database.
Files are the cheapest store that survives a forced shutdown, and a folder listing is all recovery needs.
Two copies of the program may run against one workspace.
Every operation touches a single named file and holds nothing open.
The folder is never locked and a file this process did not write is never removed.

## `public static void LDraftArchiveSave(string root, LDraft draft)`

Writes `draft` to `drafts/<LDraftId>.json`, replacing whatever was there.
The text goes to a `.json.tmp` file first and is then moved over the target.
A move is atomic, so a reader never sees a half-written draft.
A crash mid-write leaves the previous file intact.

The file is stamped with the archive's version number before it is written.
The draft handed in is not changed, so a caller keeps working with the value it holds.

The save refuses a draft in which any item still carries id zero.
Positive means a stored row and negative means an id the engine minted, so zero is never a saved state.
Only the engine mints, and the guard here means no caller can slip past it.
A credited author is an item too, so a credit with id zero is refused the same way.
The entry id of the draft itself may be zero, because that names a new record rather than an item.

## `public const int LDraftArchiveVersion = 3;`

Shape of the draft file the current build writes.
It moves whenever a draft-shaped record changes, so an older file is skipped rather than misread.
Version two added the credited authors a source draft carries.
Version three made the pronunciation a list and added the transcriptions beside it.

## `public static LDraft? LDraftArchiveRead(string root, long id)`

The draft stored under `id`, or `null` when no readable file holds it.
A missing file and an unreadable one are the same answer to the caller.
So is a file of another version, because a draft the current build did not write is not a draft.

## `public static IReadOnlyList<LDraft> LDraftArchiveScan(string root)`

Every draft the folder holds.
A file that fails to parse or cannot be opened is skipped rather than thrown.
Recovery lists leftovers after a crash, which is exactly when a truncated file is likely.
One bad file must not hide the rest.

## `public static void LDraftArchiveDelete(string root, long id)`

Removes the file for `id`, which is how a draft ends once its record is saved or abandoned.
A file already gone, or held open by the other copy of the program, is not an error.

## `public static IReadOnlyList<long> LDraftArchiveSweep(string root)`

Deletes every half-written `.json.tmp` in the drafts folder that is older than an hour.
It also deletes every draft file of another version and returns the ids they carried.
The engine drops the claim and the court rows of each returned id, which only it can reach.
A file that does not read as a draft at all is of no version and goes the same way.
Such a file is deleted and not returned, since nothing else can name it.
A save writes its pending file and then moves it over the target.
A kill between the two leaves a file the listing rightly ignores and nothing collects.
The hour keeps a file another copy of the program is writing out of reach.
A missing folder is not an error, and neither is a file that vanishes mid-sweep.

## Inline notes

### the extension check inside the listing

A Windows search pattern can match a file whose extension merely starts with the one asked for.
The half-written `.json.tmp` file must never be read as a draft.

### the age check inside the sweep

A pending file younger than the hour may belong to a save still in flight.
Taking it would break a write the other copy of the program is making.
