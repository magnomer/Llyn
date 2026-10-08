# LEntryFacade.cs
Hash: `94a8f704f109d84e`

## `public sealed class LEntryFacade : LEntryPort, LGraspPort`

The engine's facade for entry.
Every clerk call takes the engine's gate and hands the work to the clerk holding the rules.
The lifecycle goes to `LEntryClerk`, searches to `LEntryQueryClerk` and the grasp to `LGraspClerk`.
The glyph resolve and the grasp, establishment and usage reads sit here, each one call on an entry.
It implements the entry and grasp ports itself, so Host hands it to Conduct with no outlet between.

## `public LEntryFacade(LEngine engine)`

Keeps the engine and its gate so entry calls share the engine's state and lock.

## `public LEntry? LEngineEntryRead(long id)`

Reads the entry for `id`, or `null` when no entry has that id.

## `public (bool, string, string) LEngineStampRead(long entryId)`

Whether the entry is stored, with its creation and update times already worded, since the draft carries no clock.
An id no entry has answers false with empty times, so the reading view hides its stamp row.

## `internal static string LEngineStampFormat(string? utc)`

Turns a stored ISO 8601 UTC stamp into local time in the short general format of the current culture.
A missing or unreadable stamp answers empty.

## `public IReadOnlyList<LEntry> LEngineEntryFind(string query, LCatalogOrder order, LCatalogFilter filter)`

The query clerk's search under the gate.
Every other `LEngineEntryFind` overload is the same relay for the query clerk's overload of the same shape.

## `public LEntryDraft? LEngineEntryLoad(long id)`

The entry as a draft, or null when it no longer stands.
The entry clerk resolves its recordings to absolute paths.

## `internal LRevision LEngineEntryDelete(long id)`

The clerk's delete under the gate, then the entry bulletin raised outside it.
Returns the recorded revision.

## `public IReadOnlyList<LReflexDraft> LEngineReflexRead(LEntryDraft draft)`

The reflex rows of the draft that carry written text, in draft order.
A row the reflex fetch left blank stays out of a reading view.

## `public LEntry LEngineGlyphResolve(string character, string language)`

The entry `character` stands for in `language`, made when none exists yet.
Only an exact headword in that language counts, so a Mandarin entry of the same character is never taken.
The candidates come from the query clerk's search on the headword.
Creation goes through `LEngineTranslationCreate`, so the revision is recorded and the frequency fetch starts.

## `public int LEngineGraspStep`

The last grasp step, handed out so the shells draw the stars without naming the Core constant.

## `public string LEngineGraspFormat(int step)`

The localized label of one grasp step, through the grasp clerk.

## `public int LEngineGraspRead(long entryId)`

Reads the half-step grasp stored on the entry.
A missing entry reads zero, the same as an entry never rated.

## `public void LEngineGraspSave(long entryId, int grasp)`

The grasp clerk writes the grasp under the gate, then a Grasp bulletin is raised for the entry.

## `public LEstablishment LEngineEstablishmentRead()`

Counts the held drafts that differ from their origin, the stored entries and the database bytes.
Only drafts this engine holds are counted.
Each held draft is measured as the leave dialog measures it, so the bar and the dialog cannot disagree.
The entry count comes from the query clerk and the bytes from the workspace clerk.

## `public IReadOnlyDictionary<long, int> LEngineUsageRead(LOwner owner)`

The usage clerk's counts under the gate.

## `public string LEngineTallyRead(long? reference)`

The usage clerk's citation line for one Source under the gate.

## `public string LEngineTallyRead(long? id, LOwner owner)`

The usage clerk's citation line for one Source, Example or Situation under the gate.

## `public IReadOnlyList<LUsage> LEngineUsageRead(long id, LOwner owner)`

The usage clerk's itemized rows under the gate, with the epithet the settings ask for.

## `public string LEngineUnitFormat(LUnit unit)`

The localization key that names a lexical unit, empty when none is chosen.
The unit clerk owns the wording.

## `public (LOwner, int)? LEngineCardFind(LEntryDraft draft, long id)`

Which card list of `draft` holds the card `id`, and at which place, or null when neither does.
The card clerk scans the draft, so the facade only answers the port.
