# LLacunaClerk.cs
Hash: `6384e98e5452c0ea`

## `public sealed class LLacunaClerk`

The inflection fetch that fills the empty slots of an entry's paradigm and records the lacunae left.
It is a sibling of the inflection clerk, because the fetch reads the held drafts through the claim clerk.
The inflection clerk cannot, since the entry clerk composes it and the commit round composes the entry clerk.
The engine's gate is shared, so a fetch that lands writes under the same lock as every other vault call.

## `public LLacunaClerk(LRig rig, LLanguageCache languages, LParadigmClerk paradigms, LClaimClerk claims, object gate, Func<LSettings> settings, Action<LSubject, long> raise)`

Reads the entry, inflection and lacuna ports, the source factory and the fault log out of `rig`.
`settings` is read at fetch time, so a setting turned off mid-fetch discards the answer.

## `public bool LLacunaClerkCheck(long entryId)`

Whether a fetch is pending for the entry.

## `public void LLacunaClerkStart(long entryId)`

Starts a fetch for an entry with unspecified slots and no held draft.
Nothing starts for an entry whose fetch went unanswered earlier in this session.
Nothing starts when the setting is off or the language declares no morphology source.

## `public void LLacunaClerkCancel(long entryId)`

Cancels the pending fetch of the entry and deletes its lacunae, before a save rewrites the inflections.
It also forgets an unanswered fetch, so an edited entry is asked again.

## `public void LLacunaClerkClear()`

Cancels every pending fetch and forgets every unanswered one.

## `private LDraft? LDraftFind(long entryId)`

The held draft of the entry, or null.

## `private IReadOnlyList<LSource> LMorphologySourceRead(string language)`

The morphology sources of `language`, built from the pack on first use.

## `private async Task<(IReadOnlyDictionary<string, string> LLacunaFound, bool LLacunaReached)> LLacunaClerkScan(string word, string language, CancellationToken cancellation)`

Asks every source for `word` and keeps the first form each morphology code got.

## `private void LLacunaClerkApply(LEntry entry, IReadOnlyDictionary<string, string> found)`

Appends one inflection per slot not yet specified that the answer filled and records the rest as lacunae.

## `private async Task LLacunaClerkRun(LEntry entry, CancellationTokenSource fetch)`

The answer is written only when the fetch still stands and the setting is still on.
The entry must still read the same and no draft may hold it.
A source that never answered is not an answer and stores nothing.
An unanswered entry is remembered in memory for the session only.
The same fetch does not start again until a restart, a clear or an edit.
The fetch is never awaited, so a failure is recorded in the fault log rather than lost unobserved.
Cancellation by the fetch's own token is not a failure and is not recorded.
