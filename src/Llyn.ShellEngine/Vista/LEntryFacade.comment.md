# LEntryFacade.cs
Hash: `d36db84c3a051927`

## `public sealed class LEntryFacade : LEntryPort, LGraspPort`

The engine's facade for entry.
Every clerk call takes the engine's gate and hands the work to the clerk holding the rules.
The lifecycle goes to `LEntryClerk`, searches to `LEntryQueryClerk` and the grasp to `LGraspClerk`.
The glyph resolve and the grasp, establishment and usage reads sit here, each one call on an entry.
It implements the entry and grasp ports itself, so Host hands it to Conduct with no outlet between.

## `internal LEntryFacade(LEngineHearth hearth, LCardFacade card, LDraftFacade draft)`

Stores the hearth, its gate and the sibling facades it calls, all built by `LEngine` before this one.
The gate, the staff and the shared state are read through the hearth.
It takes its siblings rather than the engine, so it names only the facades it uses.

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

## `public IReadOnlyList<LEntry> LEngineEntryFind(LSubject subject, long id)`

The entries the catalog record `id` of kind `subject` reaches, under the gate.
The record is probed by id alone, so only the id of each blank probe matters to the query clerk.
A subject that reaches no entries answers no rows.
The catalog vista's child list asks here, so the probe shapes stay beside the entry rows.

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
