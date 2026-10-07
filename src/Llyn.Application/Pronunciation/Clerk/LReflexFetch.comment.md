# LReflexFetch.cs
Hash: `861e6e074c0aeefe`

## `public sealed class LReflexFetch`

The fetch that fills an entry with no reflex rows from the reflex source.
It owns the pending fetches, the remembered misses, the admission gate and the request spacing.
The engine's gate is shared, so a fetch that lands writes under the same lock as every other vault call.
Rules are read and the epithet rewritten through `LReflexClerk`, which builds and holds this fetch.

## `public LReflexFetch(LRig rig, LReflexClerk clerk, LLanguageCache languages, LClaimClerk claims, object gate, Action<LSubject, long> raise)`

Reads the entry and reflex ports, the reflex source, the clock and the fault log out of `rig`.
The claim clerk supplies the held drafts a fetch fills.

## `public void LReflexFetchStart(long entryId)`

Starts a fetch for an entry that has rules, no rows and no remembered miss.

## `public void LReflexFetchRebuild(long entryId)`

Does nothing while a fetch is pending or the language has no rules.
Otherwise it clears the rows and the miss, rewrites the epithet, empties the plain held drafts, and fetches again.
Before clearing, it hands the stored rows to `LReflexGloss` to remember their user-owned meanings.
When the fetch lands, those meanings replace the scraped meanings of matching rows.
The bulletins for the entry and every emptied draft are raised here.

## `public bool LReflexFetchCheck(long entryId)`

Whether a fetch is pending for the entry.

## `public void LReflexFetchClear()`

Cancels every pending fetch, forgets the misses and drops every remembered meaning.

## `private async Task<(IReadOnlyList<LReflexDraft> LReflexFound, bool LReflexReached)> LReflexFetchScan(string headword, string language, CancellationToken cancellation)`

One request per rule and character, spaced by the rule's interval.
The wait before each request runs on the rig's clock, so a harness can make it virtual.
The rounds are merged and every row is respelled and given its anatomy.

## `private async Task LReflexFetchRun(LEntry entry, CancellationTokenSource fetch)`

One fetch admitted through the two-wide gate.
The answer is written only when the fetch still stands and the entry still has no rows.
The entry must still read the same headword and language.
A miss is remembered and a failure still raises the bulletin, so the panel stops waiting.
The fetch is never awaited, so a failure is recorded in the fault log rather than lost unobserved.
Cancellation by the fetch's own token is not a failure and is not recorded.

## `private List<long> LReflexFetchPropagate(long entryId, IReadOnlyList<LReflex> saved, bool sweep = false)`

Copies the saved rows into every plain held draft of the entry that has no reflex rows of its own.
A draft carrying an example, situation, reference or author is skipped.
`sweep` overwrites drafts that have rows, for a rebuild.
The rows are put in the order the draft's language pack declares, not the order the rules ran in.
They become drafts through `LReflexClerk.LReflexClerkScan`, the one copy of a stored row into a draft.
