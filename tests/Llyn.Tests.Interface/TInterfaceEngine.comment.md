# TInterfaceEngine.cs
Hash: `433b19c83a83ab40`

## `internal static partial class TInterface`

The relays for the engine operations over an entry as a whole.
That is saving, loading, updating, and deleting one, and the drafts and revisions around it.
The court relays and the chronicle undo and redo with their two checks are relayed here too.
The author draft start is relayed here beside the entry draft start.
The draft save writes a held draft's content through the claim clerk, as an editor's edit does.
Grasp, favorite, card fold, reflex opening and box opening relays also live here.
Mention lookup is relayed here too.
The leftover read and sweep and the request apply are relayed here too.
The engine start over a hand-built rig sits here as well.
Most relays are transparent and carry no test logic of their own.
A few rebuild a read or a write the engine no longer offers from the clerks it keeps.

## `private static LEntry TEngineEntryCommit(this LEngine engine, long? id, LEntryDraft draft)`

Writes a whole draft the way the editor does.
The draft is started, its content normalized and saved, and then committed.
The commit starts the fetches a real save starts.
A test that must not fetch turns the setting off first.

## `internal static IReadOnlyList<LDraft> TEngineLeftoverRead(this LEngine engine)`

The drafts a leftover sweep leaves for the user.
Each is saveable, held by no one here, and claimed by no other process.
The relay filters claim-clerk drafts by held state, saveability and foreign ownership.

## `internal static (string, IReadOnlyList<LMentionPiece>, string) TEngineLineRead(LSentenceDraft sentence, LSentenceOrder order, string mark, IReadOnlyDictionary<long, string> citations)`

Relays the example line read.
It answers the frame, the sentence pieces and the Source line.

## `internal static LEngine TEngineCreate(LRig rig)`

Starts an engine over `rig` that answers the same rig for any workspace and keeps no workspace pointer.
A fault fact thus runs the engine over a rig whose vaults it has replaced.

## `internal static LCourt? TEngineCourtFind(this LEngine engine, long ownerId, long targetId)`

Existing court lookup stays behind the request facade.

## `internal static LCourt TEngineCourtStart(this LEngine engine, long ownerId, string origin, string headword, string language)`

Court creation uses the request facade rather than exposing a clerk to callers.

## `internal static LDraft? TEngineChronicleUndo(this LEngine engine, long id)`

Undo returns the request facade's resulting draft.

## `internal static LDraft? TEngineChronicleRedo(this LEngine engine, long id)`

Redo returns the request facade's resulting draft.

## `internal static bool TEngineUndoCheck(this LEngine engine, long id)`

Undo availability comes from the request facade.

## `internal static bool TEngineRedoCheck(this LEngine engine, long id)`

Redo availability comes from the request facade.

## `internal static LCourt TEngineCourtSave(this LEngine engine, long ownerId, long targetId, string headword, string language)`

Court persistence uses the request facade.

## `internal static void TEngineDraftCancel(this LEngine engine, long id)`

Cancellation stays behind the draft facade.

## `internal static bool TEngineDraftCheck(this LEngine engine, long id)`

Saveability comes from the draft facade, including refusal wording when requested.

## `internal static bool TEngineDraftCheck(this LEngine engine, long id, out string? refusal)`

Saveability comes from the draft facade, including refusal wording when requested.

## `internal static LEntry TEngineDraftCommit(this LEngine engine, long id)`

This relay exposes only the committed entry, not the full outcome.

## `internal static LOutcome TEngineOutcomeCommit(this LEngine engine, long id)`

This relay retains the full draft-commit outcome.

## `internal static void TEngineDraftSweep(this LEngine engine, long id)`

Draft sweeping stays behind the draft facade.

## `internal static void TEngineDraftDelete(this LEngine engine, long id)`

Draft deletion stays behind the draft facade.

## `internal static LDraft? TEngineDraftRead(this LEngine engine, long id)`

Draft lookup stays behind the draft facade.

## `internal static IReadOnlyList<LDraft> TEngineDraftScan(this LEngine engine)`

The scan reaches the held staff's claim clerk rather than inventing another draft inventory.

## `internal static LDraft TEngineDraftStart(this LEngine engine, string origin, long? entryId)`

The supplied origin and optional entry identify the draft start.

## `internal static void TEngineDraftSave(this LEngine engine, LDraft draft)`

The held claim clerk owns this draft-content write.

## `internal static LDraft TEngineAuthorStart(this LEngine engine, string origin, long? authorId)`

Author drafts start through the author facade.

## `internal static LRevision TEngineEntryDelete(this LEngine engine, long id)`

Entry deletion returns the entry facade's revision.

## `internal static IReadOnlyList<LEntry> TEngineEntryFind(this LEngine engine, LTag tag)`

Tag and register lookup use the entry facade.
Text queries instead reach the held staff's query clerk.

## `internal static IReadOnlyList<LEntry> TEngineEntryFind(this LEngine engine, LRegister register)`

Tag and register lookup use the entry facade.
Text queries instead reach the held staff's query clerk.

## `internal static LRegister TEngineRegisterCreate(this LEngine engine, string name)`

Register creation uses the catalog facade.

## `internal static IReadOnlyList<LEntry> TEngineEntryFind(this LEngine engine, string query)`

Tag and register lookup use the entry facade.
Text queries instead reach the held staff's query clerk.

## `internal static LEntryDraft? TEngineEntryLoad(this LEngine engine, long id)`

The entry facade supplies the stored draft.

## `internal static LEntry? TEngineEntryRead(this LEngine engine, long id)`

The entry facade supplies the stored entry summary.

## `internal static (bool, string, string) TEngineStampRead(this LEngine engine, long id)`

Stamp reads retain the entry facade's presence verdict and two strings.

## `internal static string TEngineStampFormat(string? utc)`

Stamp formatting uses the entry facade's shared formatter.

## `internal static LEntry TEngineEntrySave(this LEngine engine, LEntryDraft draft)`

A new entry follows the shared start, normalize, save and commit path.

## `internal static LEntry TEngineEntryUpdate(this LEngine engine, long id, LEntryDraft draft)`

An existing entry follows the same commit path with its id.

## `internal static bool TEngineFavoriteCheck(this LEngine engine, long entryId)`

Favorite reads use the catalog facade.

## `internal static void TEngineFavoriteDelete(this LEngine engine, long entryId)`

Unmarking uses the catalog facade.

## `internal static void TEngineFavoriteSave(this LEngine engine, long entryId)`

Marking uses the catalog facade.

## `internal static IReadOnlySet<long> TEngineFoldRead(this LEngine engine, long entryId)`

The card facade reads folded card ids for the supplied entry.

## `internal static void TEngineFoldSave(this LEngine engine, long entryId, long cardId)`

The card facade receives both entry and card ids for a fold write.

## `internal static void TEngineFoldDelete(this LEngine engine, long entryId, long cardId)`

The card facade receives both ids when removing a card fold.

## `internal static bool TEngineSpreadCheck(this LEngine engine, long entryId)`

The reflex facade reads the supplied entry's reflex opening.

## `internal static void TEngineReflexSpread(this LEngine engine, long entryId, bool opened)`

The reflex facade writes opening for the supplied entry only.

## `internal static bool TEngineBoxCheck(this LEngine engine, long entryId, LFoldBox box)`

Box kind and entry id jointly identify the opening read.

## `internal static void TEngineBoxSpread(this LEngine engine, long entryId, LFoldBox box, bool opened)`

Box kind and entry id jointly identify the opening write.

## `internal static int TEngineGraspRead(this LEngine engine, long entryId)`

Grasp reads use the entry facade.

## `internal static void TEngineGraspSave(this LEngine engine, long entryId, int grasp)`

Grasp writes use the entry facade.

## `internal static LMentionResult TEngineMentionFind(this LEngine engine, long exampleId, int offset)`

Stored example lookup uses the mention facade.
Explicit text and mention lists instead use the held staff's mention clerk.

## `internal static LMentionResult TEngineMentionFind(this LEngine engine, string text, string language, int offset, IReadOnlyList<LMention> mentions)`

Stored example lookup uses the mention facade.
Explicit text and mention lists instead use the held staff's mention clerk.

## `internal static void TEngineLeftoverSweep(this LEngine engine)`

Recovery sweeping uses the draft facade.

## `internal static LDraft TEngineRequestApply(this LEngine engine, LRequest request)`

Applying a request returns the request facade's draft.
