# LLacunaClerk.cs
Hash: `0edf452de718c310`

## `public sealed class LLacunaClerk`

Morphology answers fill unspecified paradigm slots without overwriting specified forms.
A held draft with unsaved changes to the forms or what decides them blocks fetching and applying.
A held draft that still matches the entry there is filled with the stored forms instead.
So an editor reopened on a saved entry neither loses the answer nor erases it on its next save.
The shared gate serializes fetch bookkeeping and vault access.

## `public LLacunaClerk(LRig rig, LLanguageCache languages, LParadigmClerk paradigms, LClaimClerk claims, object gate, Func<LSettings> settings, Action<LSubject, long> raise)`

Reads the entry, inflection and lacuna ports, the source factory and the fault log out of `rig`.
Keeps `languages` for the pack's morphology sources and its rule book.
`settings` is read at fetch time, so a setting turned off mid-fetch discards the answer.

## `public bool LLacunaClerkCheck(long entryId)`

Whether a fetch is pending for the entry.

## `public void LLacunaClerkStart(long entryId)`

Starts a fetch for an entry with unspecified slots and no blocking held draft.
Nothing starts for an entry whose fetch went unanswered earlier in this session.
Nothing starts when the setting is off or the language declares no morphology source.
Unless that draft or a pending fetch stops it, it first checks the stored analysis against the pack's rule book.
That check runs whatever the setting and the slots.
A stored form a slot shows is stale when its stamp differs from the book's.
A missing stamp counts as different, so rows written before the book and imported rows are caught.
Covered and uncovered rows alike carry the stamp once analysed, so each is analysed once per stamp.
The entry is then analysed again from its stored text by `LParadigmClerkUpdate`, with no network.
The slots are read once and serve both the stale check and the fetch check.
A re-analysis raises the inflection subject, so a shown box repaints with the new marks.
A blocking held draft or a pending fetch leaves the entry alone, as its forms are about to change.

## `public void LLacunaClerkCancel(long entryId)`

Cancels the pending fetch of the entry and deletes its lacunae, before a save rewrites the inflections.
It also forgets an unanswered fetch, so an edited entry is asked again.

## `public void LLacunaClerkClear()`

Cancels every pending fetch and forgets every unanswered one.

## `private LDraft? LDraftFind(long entryId)`

The held draft of the entry that blocks a fetch, or null.
A draft blocks when its headword, language, parts of speech or forms differ from the stored entry.
Any other change leaves the stored forms valid, so that draft does not block.
A draft whose entry can no longer be loaded blocks.

## `private List<long> LDraftPropagate(long entryId)`

Writes the entry's stored forms into every held draft of the entry and returns their ids.
It runs only after an answer was stored, when no held draft blocks.
Each of those drafts matched the entry before the answer, so it loses no edit of its own.
Without this, the draft's next save would set the entry's forms back to the draft's empty list.

## `private IReadOnlyList<LSource> LMorphologySourceRead(string language)`

The morphology sources of `language`, built from the pack on first use.

## `private async Task<(IReadOnlyDictionary<string, string> LLacunaFound, bool LLacunaReached)> LLacunaClerkScan(string word, string language, CancellationToken cancellation)`

Asks every source for `word` and keeps the first form each reading variety got.
The flag tells whether any source answered at all, so silence is told apart from an empty answer.
A variety is a slot key, a bare code or the joined codes of a multi-value cell.

## `private IReadOnlyList<long> LLacunaClerkApply(LEntry entry, IReadOnlyDictionary<string, string> found)`

Appends one inflection per slot not yet specified that the answer filled and records the rest as lacunae.
The answer is keyed by the slot key, so a multi-value cell finds the reading named after its codes.
The inflection carries every value of the cell.
A missed multi-value cell is stored with its key, and a missed one-value slot by its id alone.
It returns the ids of the held drafts it filled, or none when the answer added no form.

## `private async Task LLacunaClerkRun(LEntry entry, CancellationTokenSource fetch)`

The answer is written only when the fetch still stands and the setting is still on.
The entry must still read the same and no blocking draft may hold it.
A source that never answered is not an answer and stores nothing.
An unanswered entry is remembered in memory for the session only.
The same fetch does not start again until a restart, a clear or an edit.
The fetch is never awaited, so a failure is recorded in the fault log rather than lost unobserved.
Cancellation by the fetch's own token is not a failure and is not recorded.
A finished fetch raises the inflection subject, answered or not.
Then the draft subject is raised for each filled draft.
