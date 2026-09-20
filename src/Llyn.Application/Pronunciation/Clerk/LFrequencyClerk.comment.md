# LFrequencyClerk.cs

## `public sealed class LFrequencyClerk`

Word frequencies fetched, stored and graded for one rig.
The pending fetches and the entries a source answered nothing for are the clerk's own state.
The engine's gate is shared, so a fetch that lands writes under the same lock as every other vault call.

## `public LFrequencyClerk(LRig rig, LLanguageCache languages, object gate, Func<LSettings> settings, Action<LSubject, long> raise)`

Reads the entry and frequency ports and the source factory out of `rig`.
`settings` is read at fetch time, so a setting turned off mid-fetch discards the answer.
`raise` publishes the bulletin when a fetch lands.

## `public async Task<IReadOnlyList<LFrequency>> LFrequencyClerkFind(string word, string language, CancellationToken cancellation)`

The frequencies of `word` fetched now, one row per source that answered.

## `public string? LBandResolve(string language, string source, string raw)`

The band of a raw figure under the source's declaration, or null when no band matches.

## `public IReadOnlyList<LFrequency> LFrequencyClerkRead(long entryId)`

The stored rows of an entry, regraded under the current pack.
A regrade that changed is written back.
An entry with no rows starts a fetch unless a source already answered nothing for it.

## `public void LFrequencyClerkStart(long entryId)`

Starts a fetch for the entry, cancelling one already pending.
Nothing starts when the setting is off or the language declares no source.

## `public void LFrequencyClerkClear()`

Cancels every pending fetch and forgets the misses.

## `private void LFrequencyClerkCancel(long entryId)`

Cancels and disposes the pending fetch of one entry.

## `private IReadOnlyList<LSource> LFrequencySourceRead(string language)`

The frequency sources of `language`, built from the pack on first use.

## `private async Task<(IReadOnlyList<LFrequency> LFrequencyFound, bool LFrequencyReached)> LFrequencyClerkScan(string word, string language, CancellationToken cancellation)`

Asks every source for `word` and keeps the first phonetic figure each one gives.
`reached` says whether any source answered at all.

## `private LFrequency LFrequencyResolve(string language, LFrequency row)`

The row with its band, once-per figure and unit filled from the source's declaration.

## `private LSourceSpec? LSourceSpecFind(string language, string source)`

The declaration of `source` in the pack of `language`, or null.

## `private static string? LBandResolve(LSourceSpec spec, string raw)`

A numeric figure is banded by threshold, any other by the declared patterns.

## `private static bool LBandMatch(string raw, string pattern)`

A pattern match with a one-second patience, false on timeout.

## `private async Task LFrequencyClerkRun(LEntry entry, CancellationTokenSource fetch)`

One fetch admitted through the four-wide gate.
The answer is written only when the fetch still stands and the setting is still on.
The entry must still read the same headword and language.
A miss is remembered so the read does not start the same fetch again.
