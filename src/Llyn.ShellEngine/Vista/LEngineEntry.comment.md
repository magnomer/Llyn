# LEngineEntry.cs

## `public sealed partial class LEngine`

The entry facade of the engine.
Every call takes the gate and hands the work to `LEntryClerk`, which holds the rules.
The facade stays because the shell calls the engine, and the engine alone holds the gate and the observers.
The one thing done here beyond the clerk is resolving a recording to a full path on load.
The glyph, grasp, epithet, establishment and usage reads sit here too, since each is one call on an entry.

## `internal LEntry LEngineEntryCreate(LEntry entry, IReadOnlyList<LForm> forms, IReadOnlyList<LSpeech> speeches)`

Creates `entry` with `forms` and `speeches` as its ordered child rows, under the gate.

## `public LEntry? LEngineEntryRead(long id)`

Reads the entry for `id`, or `null` when no entry has that id.

## `public IReadOnlyList<LEntry> LEngineEntryFind(string query)`

The clerk's search under the gate.
Every other `LEngineEntryFind` overload is the same relay for the clerk's overload of the same shape.

## `public LEntryDraft? LEngineEntryLoad(long id)`

The entry as a draft, or null when it no longer stands.
The entry clerk resolves its recordings to absolute paths.

## `internal LRevision LEngineEntryDelete(long id)`

The clerk's delete under the gate, then the entry bulletin raised outside it.
Returns the recorded revision.

## `internal LTombstone? LEngineTombstoneRead(long entryId)`

Reads the tombstone left by deleting the Entry identified by `entryId`, or `null` when that Entry has never been deleted.

## `internal LRevision? LEngineRevisionRead()`

Reads the revision the workspace row points at, or `null` when the workspace has none yet.

## `internal IReadOnlyList<LRevisionChange> LEngineChangeRead(long revisionId)`

Reads the changes recorded under `revisionId`, in the order recorded.

## `public LGlyph? LEngineGlyphRead(string language)`

The glyph section the pack of `language` declares, or null when the language shows no glyph row.

## `public LEntry LEngineGlyphResolve(string character, string language)`

The entry `character` stands for in `language`, made when none exists yet.
Only an exact headword in that language counts, so a Mandarin entry of the same character is never taken.
Creation goes through `LEngineTranslationCreate`, so the revision is recorded and the frequency fetch starts.

## `public int LEngineGraspStep => LEntryClerk.LEntryGraspStep;`

The last grasp step, handed out so the shells draw the stars without naming the Core constant.

## `public string LEngineGraspFormat(int step)`

The localized label of one grasp step, through the entry clerk.

## `public int LEngineGraspRead(long entryId)`

Reads the half-step grasp stored on the entry.
A missing entry reads zero, the same as an entry never rated.

## `public void LEngineGraspSave(long entryId, int grasp)`

The clerk writes the grasp under the gate, then a Grasp bulletin is raised for the entry.

## `public string LEngineEpithetRead(long entryId)`

The epithet stored on the entry, or empty while the workspace hides epithets.

## `public LEstablishment LEngineEstablishmentRead()`

Counts the held drafts that differ from their origin, the stored entries and the database bytes.
Only drafts this engine holds are counted.
Each held draft is measured as the leave dialog measures it, so the bar and the dialog cannot disagree.
The counts come from the clerk, since only the held drafts are the engine's own.

## `public IReadOnlyDictionary<long, int> LEngineUsageRead(LOwner owner)`

The usage clerk's counts under the gate.

## `public IReadOnlyList<LUsage> LEngineUsageRead(long id, LOwner owner)`

The usage clerk's itemized rows under the gate, with the epithet the settings ask for.
