# TInterfaceClerk.cs

## `internal static partial class TInterface`

The relays for the clerks of the application ring, built over a rig of fakes.
Each relay is transparent and carries no test logic of its own.

## `internal static LRig TRigClerkCreate(LEntryVault entries)`

A fake rig over `entries` with in-memory workspace, revision, tombstone, pronunciation, transcription and reflex stores.
That is enough for an entry clerk save to run without SQLite.

## `internal static LRig TRigClaimCreate()`

The clerk rig with the draft, claim and court ports seated by in-memory fakes, posing as process one.

## `internal static LRig TRigProcessSet(this LRig rig, int process)`

The same rig posing as another process, its claim fake told to name that process from now on.

## `internal static void TDraftStaleSet(this LRig rig, long id)`

Marks one draft of the rig's draft fake as what the next sweep drops.

## `internal static LTombstone? TTombstoneRead(this LRig rig, long entryId)`

The tombstone the rig's tombstone fake holds for that entry, or `null`.

## `internal static long? TRevisionRead(this LRig rig)`

The revision the rig's workspace row points at, or `null` before any was recorded.

## `internal static LClaimClerk TClaimClerkCreate(LRig rig)`

The claim clerk over `rig`, with its issuer, chronicle and court built over the same rig.

## `internal static LCourtClerk TCourtClerkCreate(LRig rig)`

The court clerk over `rig`, with its own issuer and chronicle over the same rig.

## `internal static LDraft TClaimClerkStart(this LClaimClerk clerk, string origin, long entryId)`

Starts a blank entry draft through the clerk and holds it.

## `internal static LCourt TCourtClerkSave(this LCourtClerk clerk, long ownerId, long targetId, string headword)`

Writes one English court row through the clerk.

## `internal static LPortraitPage TExamplePageRead(LExample example, LReference? cited, int count, LPortraitLegend legend)`

The page the example clerk composes for one Example.

## `internal static LPortraitPage TReferencePageRead(LReference reference, IReadOnlyList<LAuthor> credits, int count, LPortraitLegend legend)`

The page the reference clerk composes for one Source.

## `internal static LPortraitPage TSituationPageRead(LSituation situation, int count, LPortraitLegend legend)`

The page the situation clerk composes for one Situation.

## `internal static LDraftClerk TDraftClerkCreate(LRig rig)`

The draft clerk over `rig`, with its issuer and its pack cache built over the same rig.

## `internal static LRequest TRequestStrayCreate(long draftId)`

A request of a kind no switch knows, so a test can prove the default arm throws.

## `private sealed record TRequestStray(long LRequestDraftId) : LRequest(LRequestDraftId);`

The one request kind the production code never declares.

## `internal static LEntryClerk TEntryClerkCreate(LRig rig)`

An entry clerk over `rig` with every clerk it composes, the transcription, reflex and recording clerks included.
The reflex clerk gets a throwaway gate and a bulletin that goes nowhere.

## `internal static LRecordingClerk TRecordingClerkCreate(LRig rig)`

A recording clerk over `rig` with a fresh language cache, trail clerk and claim clerk.

## `internal static LMarkupClerkIntake TMarkupIntakeCreate(LRig rig)`

The markup intake with the clerk graph an import needs, built over `rig`.

## `internal static LPortraitClerk TPortraitClerkCreate(LRig rig)`

The portrait clerk with every clerk a page composes from, built over `rig`.

## `private static LSettings TSettingsRead()`

The default settings a clerk built here reads at fetch time.

## `internal static Task<IReadOnlyList<LRecording>> TRecordingClerkFind(this LRecordingClerk clerk, string word, string language, string variety, LListener listener, CancellationToken cancellation)`

Relays the harvest of the recording clerk.

## `internal static void TRecordingClerkSweep(this LRecordingClerk clerk)`

Relays the sweep of the recording clerk.

## `internal static void TRecordingClerkPlay(this LRecordingClerk clerk, string? file)`

Relays the playback of the recording clerk.

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

## `internal static LRefusal TRefusalCreate(string reason)`

One refusal with `reason`, for a test that wraps it.

## `internal static LMarkupClerk TMarkupClerkCreate(LRig rig)`

A markup clerk over `rig`.

## `internal static LMarkupCargo TMarkupClerkRead(this LMarkupClerk clerk, string path)`

Relays the cargo read of the markup clerk.

## `internal static LMarkupEntry? TMarkupClerkLoad(this LMarkupClerk clerk, long id)`

Relays the per-entry load of the markup clerk.

## `internal static LMarkupOutcome TMarkupClerkImport(this LMarkupClerkIntake clerk, LMarkupCargo cargo, IReadOnlyList<LMarkupIntake> intakes)`

Relays the import of the markup intake.

## `internal static LPortraitPage TPortraitClerkRead(this LPortraitClerk clerk, long entryId, LPortraitLabel label)`

Relays the entry page of the portrait clerk.

## `internal static Task TPortraitClerkPrint(this LPortraitClerk clerk, LPortraitPage page, LPressTicket ticket)`

Relays the print of the portrait clerk.

## `internal static LEntry TEntryClerkAdd(this LRig rig, LEntry entry)`

Creates a bare entry with the headword and language of `entry`, as a translation link does.
