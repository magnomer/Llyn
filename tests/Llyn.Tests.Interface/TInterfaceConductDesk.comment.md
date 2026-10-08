# TInterfaceConductDesk.cs
Hash: `f574fb28339c25ff`

## `internal static class TInterfaceConductDesk`

The relays for Conduct's desk area, its session and its errand.
They build a desk or a session and reach the desk's starts, its tenure and the errand's maps.
They read the held draft's sentence ids and build the editor's marks over a grasp port a fact fakes.
Each relay is transparent and carries no test logic of its own.

## `internal static CDesk TDeskCreate(LEngine engine, string scope, CEnvoy envoy)`

Builds a desk over real draft and settings outlets on `engine`, as the Conduct areas that own one do.

## `internal static CDesk TDeskCreate(LEngine engine, string scope, CEnvoy envoy, string origin, CSubject subject)`

Builds the same desk under its own `origin` and `subject`, as the corpus and the playwright build theirs.

## `internal static void TDeskOccurrenceStart(this CDesk desk, long? situation)`

Starts a fresh entry already linked to the Situation, as the repertoire's new occurrence does.

## `internal static void TDeskQuotationStart(this CDesk desk, long? example)`

Starts a fresh entry already citing the Example, as the corpus's new quotation does.

## `internal static void TDeskFootnoteStart(this CDesk desk, long? reference)`

Starts a fresh entry already citing the Reference, as the sources tab's new footnote does.

## `internal static void TDeskMembershipStart(this CDesk desk, long? tag)`

Starts a fresh entry already carrying the Tag, as the tag list's new membership does.

## `internal static void TDeskCohortStart(this CDesk desk, long? register)`

Starts a fresh entry already filed under the Register, as the register list's new cohort does.

## `internal static void TDeskVistaRestore(this CDesk desk, LVista vista)`

Relays the restore of a saved vista into the desk, which only the navigation runs in production.

## `internal static void TDeskDefer(this CDesk desk, LRequest request)`

Hands the held tenure a request to defer, and does nothing when no tenure is held.

## `internal static LDraft? TDeskRead(this CDesk desk)`

Reads the desk's draft through its own draft read.

## `internal static IReadOnlyList<long> TDeskSentenceRead(this CDesk desk)`

The ids of every sentence row on the held draft's meaning and collocation cards, in ascending order.
It answers none with no draft held, so a fact names no engine draft member itself.

## `internal static CEsteem TEsteemCreate(LEngine engine, LGraspPort grasps, long stored)`

Builds the editor's marks over a real desk holding the stored entry `stored`, as the editor does.
The desk is an Input desk bound to a fresh library vista, over a real draft outlet.
Its display rules ask `grasps`, so a fake lets a fact answer the grasp reads with hostile values.
The other display ports are the engine's own facades.
Its envoy answers no and records nothing, and its media port is a bare stub.

## `internal static LDraft? TDeskHeldRead(this CDesk desk)`

Reads the draft the held tenure holds, or nothing when no tenure is held.

## `internal static void TDeskVarietySet(this CDesk desk, bool primary, long pronunciation, string variety)`

Sets a pronunciation's variety through a quill over the held tenure, and does nothing when none is held.

## `internal static LForay? TDeskForayStart(this CDesk desk, string scheme)`

Starts a transcription foray under `scheme` on the held tenure's errand, or answers nothing when none is held.

## `internal static bool TDeskChangeCheck(this CDesk desk)`

Relays whether the desk holds an unsaved change.

## `internal static CRecording? TErrandRecordingRead(LRecording? recording)`

Relays the errand's map of a stored recording into the one the desk shows.

## `internal static LRecording TErrandRecordingRead(CRecording recording)`

Relays the errand's map of a shown recording back into the stored one.

## `internal static CCandidate? TErrandCandidateRead(LCandidate? candidate)`

Relays the errand's map of a stored candidate into the one the desk shows.

## `internal static void TErrandHarvestResonate(this CErrand errand, CHarvestStep step)`

Relays the errand's answer to a harvest step.

## `internal static void TErrandLookupResonate(this CErrand errand, CLookupStep step, LForay foray)`

Relays the errand's answer to a lookup step of `foray`.

## `internal static CSession TSessionCreate(CDesk desk, IReadOnlyList<Func<bool>> pending, Func<bool> readySeam, Action<long> storedSeam)`

Builds a session over `desk` alone, with no editor, as the guild does.
The overload over an editor, below, records each editor finish in `seen`.
Both hand a fake envoy that answers false, since no session test asks the leave question.

## `internal static CSession TSessionCreate(CDesk desk, CEditor editor, Func<bool> shownSeam, List<string> seen)`

Builds a session over `desk` and `editor` with the given shown seam.
Each editor finish adds `Finish` or `Drop` to `seen` and answers yes.

## `internal static bool TSessionChangeCheck(this CSession session)`

Relays whether the session holds an unsaved change.

## `internal static bool TSessionFinish(this CSession session, bool store)`

Relays the session's finish, stored or dropped as `store` says.
It answers whether the finish went through.
