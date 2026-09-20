# LEngineFrequency.cs

## `public sealed partial class LEngine`

The frequency side of the engine.
An Entry earns one Frequency row per web source its language pack names, fetched once.
The database keeps the raw figure as ground truth and the band as a cache of its last label.
The word interval and the band are derived from the raw figure on every read, both being arithmetic.
The fill runs in the background and never blocks the save that asked for it.

## `private static readonly TimeSpan LEngineBandPatience`

How long one band regex may run on one raw figure before it counts as no match.
A pack author writes the pattern, so the engine bounds it.

## `private IReadOnlyList<LSource> LEngineFrequencyLoad(string language)`

The built frequency sources of one language, made once and kept for the engine's life.
A pack without a `frequency` list yields no sources, so every fill for that language ends at once.
An Entry with no language named has no pack, so it yields no sources either.

## `internal async Task<IReadOnlyList<LFrequency>> LEngineFrequencyFind(string word, string language, CancellationToken cancellation)`

Asks every frequency source of `language` for `word` and returns one resolved row per source that answered, in written order.
Returns an empty list when every source came up empty.

## `private async Task<(IReadOnlyList<LFrequency> LFrequencyFound, bool LFrequencyReached)> LEngineFrequencyScan(string word, string language, CancellationToken cancellation)`

The lookup behind the find, one source at a time in written order.
Every source is asked, because each corpus is its own measure and the rows are kept side by side.
The lookup runs literal with no variety fan-out and no cleanup, because a figure is not IPA.
The receiver is a relay over a delegate that drops every step, since nothing streams the figures to a caller.
The second value says whether any source was reached at all.
A word every source reached yet none knew is a miss, while sources that never answered are not.

## `private LFrequency LEngineFrequencyResolve(string language, LFrequency row)`

Stamps one row with its band, its word interval and its unit from the pack source of the same name.
The interval and the unit are stamped only when the raw figure is numeric.
The band is always recomputed from the raw figure, so a stored label never outlives the ladder that made it.
A row whose source the pack no longer names is returned as it is.

## `private LSourceSpec? LEngineSpecFind(string language, string source)`

The pack's frequency source of the given name, or null when the language is blank or names no such source.

## `internal string? LEngineBandResolve(string language, string source, string raw)`

Grades `raw` on the shared ladder, else walks the pattern bands of the named source in order.
Returns null when the pack names no such source or nothing labels the figure, which then shows as is.

## `private static string? LEngineBandResolve(LSourceSpec spec, string raw)`

A numeric figure with a word interval is graded by [LFrequency](../Llyn.Core/Pronunciation/LFrequency.comment.md) on the shared ladder.
Otherwise the first [LBand](../Llyn.Core/Pronunciation/LBand.comment.md) whose pattern matches names it.
The figure is parsed as an invariant decimal, so `3.07` and `1e3` grade as numbers.

## `private static bool LEngineBandMatch(string raw, string pattern)`

Runs one pack regex culture-invariant under the band patience.
A pattern that times out is a non-match rather than a failure.

## `public IReadOnlyList<LFrequency> LEngineFrequencyRead(long entryId)`

Reads the stored frequency rows of the Entry identified by `entryId` and stamps each with its word interval.
Returns an empty list when the Entry is missing or holds no row.
A row whose stored band differs from the one its raw figure now earns has the new band stored.
An Entry with no row whose pack has sources starts a fill before returning.
So an old Entry fills itself on first display.
An Entry the sources already answered with nothing this session is not asked again on every display.

## `internal void LEngineFrequencyStart(long entryId)`

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
At most four fills fetch at once.
An import of many entries queues them rather than opening one connection each.
A fill cancelled while it queues never fetches.
The rows are written only when the fill was not cancelled and the setting is still on.
The Entry must also still exist with the same headword and language as fetched.
A fill writes no revision row, because a machine fill is not a user edit.
Every exception is swallowed, since a missing figure is not an error the user can act on.
The pending mark is dropped only when it is still this fill's own.
So a newer fill is never unmarked by an older one.
The bulletin is raised outside the gate after the write.
