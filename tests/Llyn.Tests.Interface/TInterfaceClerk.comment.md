# TInterfaceClerk.cs
Hash: `cc703108e09c2bf4`

## `internal static partial class TInterface`

The relays for the clerks of the application ring, built over a rig of fakes.
Each relay is transparent and carries no test logic of its own.

## `internal static IReadOnlyList<(long LMeaningId, string LMeaningName, int LMeaningDepth)> TMeaningClerkSort(IReadOnlyList<LMeaning> meanings, string unknown)`

Relays the meaning clerk's reading order, so the rule is tested over handed-in Meanings.

## `internal static string TDraftListParse(string? text, bool settled, Func<string, bool> accept)`

Relays the typed-list parse, so a test reads the rule where it is owned.

## `internal static IReadOnlyList<LSpeechDraft> TSpeechParse(IReadOnlyList<LSpeechDraft> held, string? typed)`

Relays the speech clerk's parse of a typed list against the held chips.

## `internal static IReadOnlyList<LSpeechDraft>? TSpeechAdd(IReadOnlyList<LSpeechDraft> held, string? name, Func<string, LSpeechValue?> declare)`

Relays the speech clerk's add of one named chip, declaring unknown names through `declare`.

## `internal static IReadOnlyList<LSpeechDraft> TSpeechRemove(IReadOnlyList<LSpeechDraft> held, string? name)`

Relays the speech clerk's removal of one named chip.

## `internal static (IReadOnlyList<LSpeechDraft> LSpeechHeld, string LSpeechTyped) TSpeechSettle(IReadOnlyList<LSpeechDraft> shown, IReadOnlyList<LSpeechDraft> held, string typed)`

Relays the speech clerk's settling of the shown chips and the typed text into held chips and leftover text.

## `internal static IReadOnlyList<LSpeechDraft> TSpeechChipRead(IReadOnlyList<LSpeechDraft> shown, string? pending)`

Relays the speech clerk's reading of the chips the shown list and the pending text make.

## `internal static string? TSpeechPendingRead(IReadOnlyList<LSpeechDraft> held, string? typed)`

Relays the speech clerk's reading of the typed text still pending.

## `internal static LSpeechOffer TSpeechFind(IReadOnlyList<LSpeechValue> values, IReadOnlyList<LSpeechDraft> held, string? typed)`

Relays the speech clerk's offer of the values that fit the typed text.

## `internal static bool TGlossEmptyCheck(LDraft draft, long cardId, long sentenceId)`

Relays the gloss clerk's check that a sentence of a card holds no gloss.

## `internal static int TCardEndRead(LEntryDraft content, LCardKind kind)`

Relays the card clerk's reading of the end place of one card kind.

## `internal static bool TCardLoneCheck(LEntryDraft content, long id)`

Relays the card clerk's check that a card is alone in its kind.

## `internal static int? TCardShiftRead(LEntryDraft content, long id, int place)`

Relays the card clerk's judgement of a dragged place, so its unchanged and stray places are tested.

## `internal static LSpeechDraft TSpeechCreate(long value, string name)`

Builds a chip for a declared value when `value` is positive, else a chip for a new name.

## `internal static LSpeechValue TSpeechValueCreate(long value, string name)`

Builds an English speech value with `value` as its id and `name` as its name.

## `internal static LRig TRigClerkCreate(TVaultFake entries)`

A fake rig over `entries` as both entry ports, with in-memory workspace, revision, tombstone, pronunciation, transcription, reflex and etymology stores.
That is enough for an entry clerk save to run without SQLite.

## `internal static LRig TRigClaimCreate()`

The clerk rig with the draft, claim and court ports seated by in-memory fakes, posing as process one.

## `internal static LRig TRigProcessSet(this LRig rig, int process)`

The same rig posing as another process, its claim fake told to name that process from now on.

## `internal static void TDraftStaleSet(this LRig rig, long id)`

Marks one draft of the rig's draft fake as what the next sweep drops.

## `internal static LTombstone? TTombstoneRead(this LRig rig, long entryId)`

The tombstone the rig's tombstone fake holds for that entry, or `null`.

## `internal static IReadOnlyList<LRevisionDelta> TRevisionChangeRead(this LRig rig, long revisionId)`

The deltas the rig's revision fake recorded for one revision.

## `internal static IReadOnlyList<LPronunciation> TPronunciationRead(this LRig rig, long entryId)`

The readings the rig's pronunciation store holds for one entry.

## `internal static IReadOnlyList<LReflex> TReflexRead(this LRig rig, long entryId)`

The reflexes the rig's reflex store holds for one entry, as stored.

## `internal static long? TRevisionRead(this LRig rig)`

The revision the rig's workspace row points at, or `null` before any was recorded.

## `internal static LClaimClerk TClaimClerkCreate(LRig rig)`

The claim clerk over `rig`, with its issuer, chronicle and court built over the same rig.

## `internal static LCourtClerk TCourtClerkCreate(LRig rig)`

The court clerk over `rig`, with its own issuer and chronicle over the same rig.

## `internal static LDraft TClaimClerkStart(this LClaimClerk clerk, string origin, long entryId)`

Starts a blank entry draft through the clerk and holds it.

## `internal static IReadOnlySet<long> TClaimHeldRead(this LClaimClerk clerk)`

The ids of the drafts the clerk holds.

## `internal static bool TClaimClerkCheck(this LClaimClerk clerk, long id)`

Relays the claim clerk's check of one draft id.

## `internal static bool TClaimForeignCheck(this LClaimClerk clerk, long id)`

Relays the claim clerk's check that another process holds the draft.

## `internal static LDraft? TClaimDraftRead(this LClaimClerk clerk, long id)`

Relays the claim clerk's read of one draft.

## `internal static IReadOnlyList<LDraft> TClaimDraftScan(this LClaimClerk clerk)`

Relays the claim clerk's scan of its drafts.

## `internal static void TClaimClerkFinish(this LClaimClerk clerk, long id)`

Relays the claim clerk's finish of one draft.

## `internal static void TClaimClerkCancel(this LClaimClerk clerk, long id)`

Relays the claim clerk's cancel of one draft.

## `internal static void TClaimClerkSweep(this LClaimClerk clerk)`

Relays the claim clerk's sweep.

## `internal static LCourt TCourtClerkSave(this LCourtClerk clerk, long ownerId, long targetId, string headword)`

Writes one English court row through the clerk.

## `internal static IReadOnlyList<LCourt> TCourtClerkScan(this LCourtClerk clerk)`

Relays the court clerk's scan of its court rows.

## `internal static LPortraitPage TExamplePageRead(LExample example, LReference? cited, int count, LPortraitLegend legend)`

The page the example clerk composes for one Example.

## `internal static LPortraitPage TReferencePageRead(LReference reference, IReadOnlyList<LAuthor> credits, int count, LPortraitLegend legend)`

The page the reference clerk composes for one Source.

## `internal static LPortraitPage TSituationPageRead(LSituation situation, int count, LPortraitLegend legend)`

The page the situation clerk composes for one Situation.

## `internal static bool TSchemeTakenCheck(IReadOnlyList<LTranscriptionDraft> drafts, string scheme, long ownId)`

Relays the reading clerk's check that another draft than `ownId` already uses the scheme.

## `internal static LTranscriptionSheet TTranscriptionSheetRead(IReadOnlyList<string> schemes, IReadOnlyList<LTranscriptionDraft> drafts, IReadOnlyList<LTranscriptionDraft> other)`

Relays the reading clerk's composing of the transcription sheet from schemes, drafts and the other drafts.

## `internal static int TTranscriptionPositionRead(IReadOnlyList<LTranscriptionDraft> spelled, long transcription)`

Relays the reading clerk's reading of one transcription's position among the spelled drafts.

## `internal static LDraftClerk TDraftClerkCreate(LRig rig)`

The draft clerk over `rig`, with its issuer and its language cache built over the same rig.

## `internal static LDraft TDraftClerkApply(this LDraftClerk clerk, LDraft draft, LRequest request)`

Relays the draft clerk's application of one request to a draft, answering the changed draft.

## `internal static LRequest TRequestStrayCreate(long draftId)`

A request of a kind no switch knows, so a test can prove the default arm throws.

## `private sealed record TRequestStray(long LRequestDraftId) : LRequest(LRequestDraftId)`

The one request kind the production code never declares.

## `internal static LEntryClerk TEntryClerkCreate(LRig rig)`

An entry clerk over `rig` with every clerk it composes, the transcription, reflex and recording clerks included.
Its translation and entry clerks each stamp through their own revision clerk over the same rig.
The reflex clerk gets a throwaway gate and a bulletin that goes nowhere.

## `internal static LRecordingClerk TRecordingClerkCreate(LRig rig)`

A recording clerk over `rig` with a fresh language cache, trail clerk and claim clerk.

## `internal static LMarkupClerkIntake TMarkupIntakeCreate(LRig rig)`

The markup intake with the clerk graph an import needs, built over `rig`.
The revision and query clerks it adds read the same rig, so the import stamps where the test reads.

## `internal static LPortraitClerk TPortraitClerkCreate(LRig rig)`

The portrait clerk with every clerk a page composes from, built over `rig`.

## `internal static LPortraitClerkPress TPortraitPressCreate(LRig rig)`

The press clerk over `rig`, which saves and prints a composed page.

## `private static LSettings TSettingsRead()`

The default settings a clerk built here reads at fetch time.

## `internal static Task<IReadOnlyList<LRecording>> TRecordingClerkFind(this LRecordingClerk clerk, string word, string language, string variety, LListener listener, CancellationToken cancellation)`

Relays the harvest of the recording clerk.

## `internal static void TRecordingClerkSweep(this LRecordingClerk clerk)`

Relays the sweep of the recording clerk.

## `internal static int TRecordingClerkPlay(this LRecordingClerk clerk, string? file, double volume = 1)`

Relays the playback of the recording clerk at `volume`, full by default, answering the play's ticket.

## `internal static void TRecordingClerkAdjust(this LRecordingClerk clerk, double volume)`

Relays the live volume change of the recording clerk.

## `internal static void TRecordingClerkStop(this LRecordingClerk clerk, int ticket)`

Relays the stop of the recording clerk for `ticket`.

## `internal static string TRecordingFormat(LRig rig, string path)`

A recording path made workspace-relative through the trail of `rig`.

## `internal static LPronunciation TPronunciationSave(LRig rig, LPronunciation pronunciation)`

One pronunciation row written straight into the rig's pronunciation port.

## `internal static void TAudioSave(LRig rig, long pronunciationId, string file, string? source)`

One recording written straight into the rig's pronunciation port.

## `internal static LDoctorRescue TWorkspaceRescueCreate(LRig rig)`

Relays the workspace clerk's static rescue step.

## `internal static LSettings TWorkspaceSettingsRead(LRig rig, LSettings fallback, out bool settled)`

Relays the workspace clerk's static settings step.

## `internal static string? TWorkspaceNoticeRead(Exception exception)`

Relays the workspace clerk's notice read.

## `internal static string? TWorkspaceChosenRead(string chosen, string folder)`

Relays the workspace clerk's rule for a chosen folder.

## `internal static LRefusal TRefusalCreate(string reason)`

One refusal with `reason`, for a test that wraps it.

## `internal static LMarkupClerk TMarkupClerkCreate(LRig rig)`

A markup clerk over `rig`, exporting through the entry clerk `TMarkupExportCreate` builds.

## `internal static LMarkupClerkEntry TMarkupExportCreate(LRig rig)`

A markup entry clerk over `rig`, with a reflex clerk of its own.
So an export reads the pack's reflex order.
One example clerk serves both its card clerk and itself, as in the engine.

## `internal static LMarkupCargo TMarkupClerkRead(this LMarkupClerk clerk, string path)`

Relays the cargo read of the markup clerk.

## `internal static LMarkupEntry? TMarkupClerkLoad(this LMarkupClerkEntry clerk, long id)`

Relays the per-entry load of the markup entry clerk.

## `internal static LMarkupOutcome TMarkupClerkImport(this LMarkupClerkIntake clerk, LMarkupCargo cargo, IReadOnlyList<LMarkupIntake> intakes)`

Relays the import of the markup intake.

## `internal static LPortraitPage TPortraitClerkRead(this LPortraitClerk clerk, long entryId, LPortraitLabel label)`

Relays the entry page of the portrait clerk.

## `internal static Task TPortraitClerkPrint(this LPortraitClerkPress clerk, LPortraitPage page, LPressTicket ticket)`

Relays the print of the press clerk.

## `internal static LEntry TEntryClerkSave(this LEntryClerk clerk, LEntryDraft draft)`

Relays the entry clerk's save of a draft with no extra changes.

## `internal static LTranslationClerk TTranslationClerkCreate(LRig rig)`

The translation clerk over `rig`, stamping through a revision clerk over the same rig.

## `internal static IReadOnlyList<LEntry> TTranslationClerkFind(this LTranslationClerk clerk, string query, long? entryId)`

Relays the translation clerk's find of entries for a query, with an optional entry id.

## `internal static LEntry? TTranslationClerkResolve(this LTranslationClerk clerk, string word, long? entryId)`

Relays the translation clerk's resolve of a word to one entry, or `null`.

## `internal static string? TTranslationWordRead(string text)`

Relays the translation clerk's static reading of the word in a text.

## `internal static LEntry TEntryClerkAdd(this LRig rig, LEntry entry)`

Creates a bare entry with the headword and language of `entry`, as a translation link does.

## `internal static LEntry? TEntryClerkRead(this LEntryClerk clerk, long id)`

Relays the entry clerk's read of one entry, or `null`.

## `internal static IReadOnlyList<LEntry> TEntryQueryFind(this LRig rig, string query, LCatalogOrder order)`

Relays the query clerk's find of entries for a query in the given order, over a query clerk on `rig`.

## `internal static LRevision TEntryClerkDelete(this LEntryClerk clerk, long id)`

Relays the entry clerk's delete of one entry, answering the revision it recorded.
