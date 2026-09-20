# LReflexClerk.cs

## `public sealed class LReflexClerk`

Reflexes of an entry, their fetch, their anchors and the epithet derived from them.
The row sync serves the entry save, the fetch fills an entry that has none, and the epithet follows both.
The engine's gate is shared, so a fetch that lands writes under the same lock as every other vault call.

## `public LReflexClerk(LRig rig, LLanguageCache languages, LClaimClerk claims, object gate, Action<LSubject, long> raise)`

Reads the entry and reflex ports, the reflex source and the clock out of `rig`.
The claim clerk supplies the held drafts a fetch fills.

## `public static IReadOnlyList<long> LReflexAnchorToggle(IReadOnlyList<long> anchors, long fanqieId, bool anchored)`

The anchor list with `fanqieId` added or removed.

## `public static bool LReflexAnchorMatch(IReadOnlyList<long> one, IReadOnlyList<long> other)`

Whether two anchor lists name the same rows.

## `public IReadOnlyList<LReflexRule> LReflexRuleRead(string language)`

The reflex rules the pack of `language` declares, or none for a blank language.

## `public IReadOnlyList<LReflex> LReflexClerkRead(long entryId)`

The stored reflex rows of an entry.

## `public IReadOnlyList<LReflex> LReflexClerkSet(long entryId, IReadOnlyList<LReflex> reflexes)`

Replaces the rows of an entry, rewrites its epithet and marks it updated.

## `public void LReflexClerkSync(long entryId, string language, IReadOnlyList<LReflexDraft> drafts, List<LRevisionChange>? changes, Dictionary<long, long> identity)`

The reflexes of an entry reconciled to its draft, anatomies filled from the pack.
An unchanged list writes nothing.
A positive id naming no stored row refuses the commit.
A change is recorded only when something beside the anatomy moved.

## `public static IReadOnlyList<LReflexDraft> LReflexClerkReset(IReadOnlyList<LReflexDraft> drafts)`

The drafts with every positive id cleared, for a fresh entry that has no stored rows to match.

## `public void LReflexEpithetSave(long entryId)`

Rewrites the epithet of an entry from its reflex rows and the rules of its language.
A language whose rules declare no epithet clears it.

## `public void LReflexClerkStart(long entryId)`

Starts a fetch for an entry that has rules, no rows and no remembered miss.

## `public void LReflexClerkRebuild(long entryId)`

Clears the rows and the miss of an entry, empties its held drafts, and fetches again.
The bulletins for the entry and every emptied draft are raised here.

## `public bool LReflexClerkCheck(long entryId)`

Whether a fetch is pending for the entry.

## `public async Task<IReadOnlyList<LReflexDraft>> LReflexClerkFind(string headword, string language, CancellationToken cancellation)`

The reflexes of `headword` fetched now, merged across rules and characters.

## `public void LReflexClerkClear()`

Cancels every pending fetch and forgets the misses.

## `private async Task<(IReadOnlyList<LReflexDraft> LReflexFound, bool LReflexReached)> LReflexClerkScan(string headword, string language, CancellationToken cancellation)`

One request per rule and character, spaced by the rule's interval on the clock.
The rounds are merged and every row is respelled and given its anatomy.

## `private async Task LReflexClerkRun(LEntry entry, CancellationTokenSource fetch)`

One fetch admitted through the two-wide gate.
The answer is written only when the fetch still stands and the entry still has no rows.
The entry must still read the same headword and language.
A miss is remembered and a failure still raises the bulletin, so the panel stops waiting.

## `private List<long> LReflexClerkPropagate(long entryId, IReadOnlyList<LReflex> saved, bool sweep = false)`

Copies the saved rows into every held entry draft of the entry that has no reflex rows of its own.
`sweep` overwrites drafts that have rows, for a rebuild.
