# LReflexClerk.cs
Hash: `a792f584f32b011e`

## `public sealed class LReflexClerk`

Reflexes of an entry, their anchors, their rules and the epithet derived from them.
The row sync serves the entry save, and the epithet follows every write of the rows.
The fetch that fills an entry with no rows lives in `LReflexFetch`, held here.

## `public LReflexClerk(LRig rig, LLanguageCache languages, LClaimClerk claims, object gate, Action<LSubject, long> raise)`

Reads the entry and reflex ports out of `rig`.
The claims, the engine's gate and the bulletin are handed on to the fetch it builds.

## `public LReflexFetch LReflexClerkFetch { get; }`

The fetch of this clerk, built with it and handed it for the rules and the epithet.

## `public static IReadOnlyList<LAnchorRow> LReflexAnchorScan(IReadOnlyList<LFanqieRow> rows, IReadOnlyList<long> anchors, IReadOnlyList<LDescent> tones, string reflex, string tone)`

The stored fanqie rows marked held and estimated for the anchor dropdown.
The estimated classes are the ones `tones` resolves for the `reflex` language and `tone`.

## `public static bool LReflexAnchorCheck(IReadOnlyList<LFanqieRow> rows, string headword)`

Whether the reflex rows of `headword` may carry anchors.

## `public static string LReflexAnchorFormat(IReadOnlyList<LFanqieRow> rows, IReadOnlyList<long> anchors, string headword, string separator)`

The readings of the anchored rows joined with `separator`, or empty when the rows cannot be anchored.

## `public IReadOnlyList<LReflexRule> LReflexRuleRead(string language)`

The reflex rules the pack of `language` declares, or none for a blank language.

## `public IReadOnlyList<LReflexDraft> LReflexClerkSort(string language, IReadOnlyList<LReflexDraft> rows)`

The rows in the order the pack of `language` declares, the one door every view's reflex rows pass.
The rule itself is `LReflexOrderSort`, and this only finds the pack's order for it.
A blank language or a pack without an order sorts by language name, kind name, main flag, text and id.
Stored positions are left alone, since the order is applied on every read.

## `public static IReadOnlyList<LReflexDraft> LReflexClerkScan(IReadOnlyList<LReflex> rows)`

The stored rows as drafts, every field and the stored id kept, in the order given.
It lets stored rows pass `LReflexClerkSort`, which orders drafts.
The fetch's copy into held drafts, the epithet and the series members in `LStemClerk` all convert here.
So the copy is written once.

## `public static IReadOnlyList<LReflexDraft> LReflexWrittenScan(IReadOnlyList<LReflexDraft> rows)`

The rows that carry written text, in the order given.
A row the reflex fetch left blank stays out of a reading view.
The entry page and the series page both filter here, so the rule has one owner.

## `public IReadOnlyList<string> LReflexFoldedRead(string language)`

The languages the pack of `language` folds away, read from its reflex rules.
None for a blank language or a pack without folded rules.

## `public void LReflexClerkSync(long entryId, string language, IReadOnlyList<LReflexDraft> drafts, List<LRevisionDelta> changes, Dictionary<long, long> identity)`

The reflexes of an entry reconciled to its draft, anatomies filled from the pack.
An unchanged list writes nothing and records no revision, whatever order its rows arrive in.
A positive id naming no stored row refuses the commit.
A change is recorded only when something beside the anatomy moved.

## `public static IReadOnlyList<LReflexDraft> LReflexClerkReset(IReadOnlyList<LReflexDraft> drafts)`

The drafts with every positive id cleared, for a fresh entry that has no stored rows to match.

## `public void LReflexEpithetSave(long entryId)`

Rewrites the epithet of an entry from its reflex rows and the rules of its language.
A language whose rules declare no epithet clears it.
The stored rows pass `LReflexClerkSort` first, so the pieces follow the pack's declared order.
The stored position is fetch order, so it never decides where a piece stands.
