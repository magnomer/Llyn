# LHarvest.cs
Hash: `626d00a270675e5e`

## `public sealed class LHarvest`

Fans an audio request out to every source it was given at once.
It runs the whole set, because the caller hands it the recording sources alone.
Each recording carries the position its source holds in that set, so a slow source keeps its declared place.
Streams each downloadable recording back to the listener as it arrives.
The download counterpart to `LLookup`.
One slow source never blocks the others, and one failing source never stops them running.
A source that answered produces one recording per reading, all sharing the source's position.
Each recording carries its reading's variety.
The readings pass through `LReading.LReadingScan` first, so an untagged recording fans out over the pack's varieties.
What is streamed then passes through `LHarvestRecordingScan`, which narrows it to the variety the caller asked for.
A source with nothing, never reached, or with nothing in the asked variety still streams one addressless recording.
So the menu can name a broken source rather than leaving its row silently absent.
Each source runs under its own minute-long deadline, exactly as a lookup source does.
The discovery reports complete once all sources have finished.
It also returns the whole set it found, ordered by source position with a stable sort.
The returned set is never narrowed, so a caller can keep it and narrow it again for another row.
The sources are supplied ready-built and language-agnostic, so this orchestrator knows nothing about any particular source or language.

## `public LHarvest(IReadOnlyList<LSource> sources, IReadOnlyList<LVariety> varieties)`

Holds the sources and the pack's varieties.
A null list refuses construction.

## `public async Task<IReadOnlyList<LRecording>> LHarvestStart(string word, string variety, LListener listener, CancellationToken cancellation)`

Starts the discovery for `word` and streams every recording to `listener`.
`variety` is the variety of the row that opened the menu, or empty when that row has none.
Empty streams every variety, so the menu shows each recording under its own flag.
A named variety streams that variety only, so the menu offers what fits the row.
The returned set holds every variety either way.
The harvest alone ends the listener, once, whether the sources succeed or one throws.
A cancelled harvest withholds `LListenerFinish`, since the menu it would close is already gone.

## `private static async Task LHarvestSourceRun(LSource source, int order, string word, string variety, IReadOnlyList<LVariety> varieties, LListener listener, List<LRecording> found, CancellationToken cancellation)`

Runs one source and streams its recordings to `listener`.
It announces the source first, so the menu can show the row while the answer is pending.
It adds every recording to `found` under a lock, then streams the narrowed ones.
A source cancelled after its answer streams nothing.

## `public static IReadOnlyList<LRecording> LHarvestRecordingScan(IReadOnlyList<LRecording> recordings, string variety)`

Narrows already fanned-out recordings to the one variety asked for.
An empty variety returns the recordings untouched.
Otherwise a recording survives only when its tag equals the variety ordinally.
The fan-out ran first, so an untagged reading already became one recording per declared variety.
A recording tagged with another variety is dropped, since the row it would fill belongs elsewhere.
A source left with nothing gets one addressless recording under its position, reached as its answer was.
So the menu still names that source rather than leaving its row silently absent.
The recordings are walked in source position runs, which is how the harvest returns them.

## `private static async Task<LAnswer> LHarvestAnswerRead(LSource source, string word, CancellationToken cancellation)`

The recording counterpart of `LLookupAnswerRead`.
Only the deadline becomes a lost answer.
Any other failure propagates to the caller instead of hiding as one.
