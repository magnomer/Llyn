# LDraftArchive.cs

## `public sealed class LDraftArchive : LDraftVault`

The adapter of `LDraftVault` for one workspace, whose root the constructor binds.
Keeps tentative records as plain files in the workspace `drafts` folder, one file per draft named after its id.
Nothing here reaches SQLite.
A record the user has not committed must never appear in the database.
Files are the cheapest store that survives a forced shutdown, and a folder listing is all recovery needs.
Two copies of the program may run against one workspace.
Every operation touches a single named file and holds nothing open.
The folder is never locked and a file this process did not write is never removed.

## `private static readonly JsonSerializerOptions LDraftArchiveIndent`

The one option set both directions share, indented on write.
Its resolver drops every property without a setter.
So a derived value such as the draft's IPA never lands in the file.
The records in Core carry no serializer attribute for that, since the serializer is this adapter's alone.

## `public LDraftArchive(string root)`

Binds the store to the workspace `root` whose `drafts` folder it keeps.

## `public void LDraftSave(LDraft draft)`

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
The image and video rows of a held Situation are items too, and are checked as a card's are.
The entry id of the draft itself may be zero, because that names a new record rather than an item.

## `public const int LDraftArchiveVersion = 5;`

Shape of the draft file the current build writes.
It moves whenever a draft-shaped record changes, so an older file is skipped rather than misread.
Version two added the credited authors a source draft carries.
Version three made the pronunciation a list and added the transcriptions beside it.
Version five added the etymology an entry draft carries, with its spans and its links.

## `public LDraft? LDraftRead(long id)`

The draft stored under `id`, or `null` when no readable file holds it.
A missing file and an unknown one are the same answer to the caller.
So is a file of another version, because a draft the current build did not write is not a draft.

## `public IReadOnlyList<LDraft> LDraftScan()`

Every draft the folder holds.
A file that fails to parse or cannot be opened is skipped rather than thrown.
Recovery lists leftovers after a crash, which is exactly when a truncated file is likely.
One bad file must not hide the rest.

## `public void LDraftDelete(long id)`

Removes the file for `id`, which is how a draft ends once its record is saved or abandoned.
A file already gone, or held open by the other copy of the program, is not an error.

## `public IReadOnlyList<long> LDraftSweep()`

Deletes every half-written `.json.tmp` in the drafts folder that is older than an hour.
It also sets aside every draft file of another version and returns the ids they carried.
The engine drops the claim and the court rows of each returned id, which only it can reach.
A file that does not read as a draft at all is of no version and goes the same way.
Such a file is set aside and not returned, since nothing else can name it.
Nothing is deleted: a set-aside file moves under `drafts/broken`, where the listing never looks.
A draft written by another build is still the user's work, and an upgrade must not wipe it.
A file that cannot be read this round is left where it is.
A passing lock is not a broken draft.
A save writes its pending file and then moves it over the target.
A kill between the two leaves a file the listing rightly ignores and nothing collects.
The hour keeps a file another copy of the program is writing out of reach.
A missing folder is not an error, and neither is a file that vanishes mid-sweep.

## `private static bool LDraftBrokenSave(string root, string file)`

Moves one unreadable or foreign-version draft file under `drafts/broken` and reports whether it moved.
A name already taken there gets the moment appended, so an earlier copy is never overwritten.
A file that cannot be moved stays and is reported as kept, so its id is not dropped.

## `private static void LDraftArchiveNormalize(JsonTypeInfo info)`

Removes the properties with no setter from an object's contract, so only stored fields are written.
The serializer's own read-only switch keeps get-only collections, which is why the contract is trimmed by hand.

## `private static LDraft? LDraftArchiveParse(string text, bool checking)`

Reads one draft out of its JSON text, or nothing when the text is not a draft.
`checking` refuses a draft of another version, as every reader but the sweep does.

## Inline notes

### the extension check inside the listing

A Windows search pattern can match a file whose extension merely starts with the one asked for.
The half-written `.json.tmp` file must never be read as a draft.

### the age check inside the sweep

A pending file younger than the hour may belong to a save still in flight.
Taking it would break a write the other copy of the program is making.
