# LEngineFrequency.cs

## `public sealed partial class LEngine`

The frequency side of the engine.
An Entry earns one Frequency value, fetched once from the web sources its language pack names.
The database keeps only the `"<source>|<raw>"` string, never the band.
The band is resolved from the pack each time the value is read, so a pack edit applies at once.
The fill runs in the background and never blocks the save that asked for it.

## `private static readonly TimeSpan LEngineBandPatience`

How long one band regex may run on one raw figure before it counts as no match.
A pack author writes the pattern, so the engine bounds it.

## `private IReadOnlyList<LSource> LEngineFrequencyLoad(string language)`

The built frequency sources of one language, made once and kept for the engine's life.
A pack without a `frequency` list yields no sources, so every fill for that language ends at once.
An Entry with no language named has no pack, so it yields no sources either.

## `public async Task<LFrequency?> LEngineFrequencyFind(string word, string language, CancellationToken cancellation)`

Asks the frequency sources of `language` for `word` and keeps the first non-empty answer in written order.
Returns null when every source came up empty.

## `private async Task<(LFrequency? LFrequencyFound, bool LFrequencyReached)> LEngineFrequencyScan(string word, string language, CancellationToken cancellation)`

The lookup behind the find, one source at a time in written order, stopping at the first answer.
A later source is never asked once an earlier one has answered, so no request is wasted.
The lookup runs literal with no variety fan-out and no cleanup, because a figure is not IPA.
The receiver is a private no-op, since nothing streams the figures to a caller.
The second value says whether any source was reached at all.
A word every source reached yet none knew is a miss, while sources that never answered are not.

## `internal string? LEngineBandResolve(string language, string raw)`

Walks the bands of the pack in order and returns the name of the first that matches `raw`.
A band matches by its numeric limit or by its regex pattern, as [LBand](../Llyn.Core/Pronunciation/LBand.comment.md) states.
The figure is parsed as an invariant decimal, so `3.07` and `1e3` compare against a limit.
Returns null when no band matches, so the raw figure shows as is.
An Entry with no language named has no bands, so it returns null too.

## `private static bool LEngineBandMatch(string raw, string pattern)`

Runs one pack regex culture-invariant under the band patience.
A pattern that times out is a non-match rather than a failure.

## `public LFrequency? LEngineFrequencyRead(long entryId)`

Reads the stored frequency of the Entry identified by `entryId` and resolves its band from the pack now.
Returns null when the Entry is missing or holds no value.
An empty value on an Entry whose pack has sources starts a fill before returning.
So an old Entry fills itself on first display.
An Entry the sources already answered with nothing this session is not asked again on every display.

## `public void LEngineFrequencyStart(long entryId)`

Begins a background fill for the Entry identified by `entryId` and returns at once.
A fill already running for that Entry is cancelled first, so the newest headword and language win.
An explicit start also forgets an earlier miss, since a save or a rename is a reason to ask again.
Nothing starts when the setting is off, the Entry is missing, or the pack has no sources.
The gate holds the pending map, so the same Entry never fetches twice at once.

## `private void LEngineFrequencyCancel(long entryId)`

Cancels and forgets the pending fill of one Entry, if any.
Called under the gate.

## `private void LEngineFrequencyClear()`

Cancels every pending fill and forgets every miss.
Called under the gate when the workspace changes or the engine is disposed.
A fill begun against one workspace must never write into the next.

## `private async Task LEngineFrequencyRun(LEntry entry, CancellationTokenSource fetch)`

The fill itself, run outside the gate for the whole fetch.
The value is written only when the fill was not cancelled and the setting is still on.
The Entry must also still exist with the same headword and language as fetched.
A fill writes no revision row, because a machine fill is not a user edit.
Every exception is swallowed, since a missing figure is not an error the user can act on.
The pending mark is dropped only when it is still this fill's own.
So a newer fill is never unmarked by an older one.
The bulletin is raised outside the gate after the write.

## `private sealed class LReceiverFrequency : LReceiver`

A receiver that ignores every callback.
The lookup requires one, but the fill only wants the returned list.
