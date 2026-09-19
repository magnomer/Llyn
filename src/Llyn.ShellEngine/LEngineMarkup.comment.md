# LEngineMarkup.cs

## `public sealed partial class LEngine`

The markup half of import.
A `.llx` file is read into `LMarkupEntry` records, matched against the workspace, and written as entries.
All three `LMarkupMode` values land here.
The Append merge stands in `LEngineMarkupAppend.cs` and the Replace detach in `LEngineMarkupReplace.cs`.
Every link by name, and the sense pass that follows the writes, stands in `LEngineMarkupLink.cs`.
The turn from a parsed entry into the draft the update path takes stands in `LEngineMarkupDraft.cs`.

## `public LMarkupCargo LEngineMarkupRead(string path)`

Reads the file at `path` through the markup port and parses it once, into the cargo the import later takes.
The port refuses a file past its ceiling before a byte is read, so an oversize file costs nothing.
The shell shows what a file holds before it commits to it, and commits to what it showed.
An entry language that could not name a pack folder is blanked and reported at the entry's line.
Every loader refuses such a name too, but the file should not carry it into the store at all.

## `public Task<LMarkupCargo> LEngineMarkupStart(string path)`

The first stage of an import on a worker thread: the read, so the window stays live while parsing.
The hop lives here because the shell starts no task of its own.

## `public IReadOnlyList<LEntry> LEngineMarkupFind(LMarkupEntry entry)`

Every stored entry whose headword and language equal the parsed ones, letter case folded.
Two answers mean the file cannot say which one it meant, and that is the caller's to settle.

## `public Task<LMarkupOutcome> LEngineMarkupStart(LMarkupCargo cargo, IReadOnlyList<LMarkupIntake> intakes)`

The second stage on a worker thread: the import under the intakes the customs window settled.
One verb names both stages, since together they are the one staged procedure the shell begins.

## `public LMarkupOutcome LEngineMarkupImport(LMarkupCargo cargo, IReadOnlyList<LMarkupIntake> intakes)`

Writes every entry of the cargo under the mode its intake names, inside one lock, one session and one revision.
The intake count must equal the entry count, each index named once, else `LRefusalItem`.
Any refusal along the way rolls the whole file back, so an import is whole or it never happened.
Every Append or Replace target is validated first, inside the lock, before any row is written.

Bare entries are prepared next, so a link between two entries of the same file lands on a real id.
Each entry is then resolved and filled in file order through the same update path a draft commit uses.
An Append or Replace entry with no language keeps the target's stored language.
A New entry records a `create` change and the other two modes record an `update` change.
A Replace draft carries no ids, so the update's sync drops every row the file lacks.
Examples the sync detaches stay as corpus rows, since detaching never deletes and nothing sweeps them.
A Merge draft is the stored entry with the file's content appended, ids intact.

A mention that names a sense is stored on its entry alone at first.
Once every entry of the file is stored, the sense pass points each one at the sense that now exists.
So a mention may cite a sense of a later entry.
It may also cite a sense the Replace of its own entry is about to rewrite.
The frequency fill starts for each entry after the commit, as a saved entry starts it.

## `private static void LEngineLineSet(List<LMarkupOmission> omissions, int noted, int line)`

Gives every omission added since `noted` the line of the entry being resolved, when it has none.
A record carries no line of its own, so the entry's line is the nearest place a reader can look.

## `private void LEngineMarkupValidate(IReadOnlyList<LMarkupIntake> intakes)`

Every Append or Replace target must be a stored entry, else `LRefusalEntry`.
Two intakes naming one target refuse with `LRefusalItem`, since the second write would erase the first.
A target this process holds an entry draft on refuses with `LRefusalStale`, since two writers would race.
Example, situation and reference drafts share the id field but not the id space, so they are passed over.

## `private IReadOnlyDictionary<int, long> LEngineMarkupPrepare(IReadOnlyList<LMarkupEntry> entries, IReadOnlyList<LMarkupIntake> intakes)`

A New intake creates a bare entry holding only headword and language, and its index maps to the new id.
A blank headword refuses here, before any row is written.
Other modes map their index to the target they name.
