# LHarvest.cs

## `public sealed class LHarvest`

Fans an audio request out to every source it was given at once.
It runs the whole set, because the caller hands it the recording sources alone.
Each recording carries the position its source holds in that set, so a slow source keeps its declared place.
Streams each downloadable recording back to the listener as it arrives.
The download counterpart to `LLookup`.
One slow or failing source never blocks or fails the others.
A source that answered produces one recording per reading, all sharing the source's position and each carrying its variety.
The readings pass through `LReading.LReadingScan` first, so an untagged recording fans out over the pack's declared varieties.
What is streamed then passes through `LHarvestRecordingScan`, which narrows it to the variety the caller asked for.
A source with nothing, never reached, or with nothing in the asked variety still streams one addressless recording.
So the menu can name a broken source rather than leaving its row silently absent.
Each source runs under its own minute-long deadline, exactly as a lookup source does.
The discovery reports complete once all sources have finished.
It also returns the whole set it found, ordered by source position with a stable sort, exactly as `LLookup` does.
The returned set is never narrowed, so a caller can keep it and narrow it again for another row.
The sources are supplied ready-built and language-agnostic, so this orchestrator knows nothing about any particular source or language.

## `public async Task<IReadOnlyList<LRecording>> LHarvestStart(string word, string variety, LListener listener, CancellationToken cancellation)`

Starts the discovery for `word` and streams every recording to `listener`.
`variety` is the variety of the row that opened the menu, or empty when that row has none.
Empty streams every variety, so the menu shows each recording under its own flag.
A named variety streams that variety only, so the menu offers what fits the row.
The returned set holds every variety either way.

## `public static IReadOnlyList<LRecording> LHarvestRecordingScan(IReadOnlyList<LRecording> recordings, string variety)`

Narrows already fanned-out recordings to the one variety asked for.
An empty variety returns the recordings untouched.
Otherwise a recording survives only when its tag equals the variety ordinally.
The fan-out ran first, so an untagged recording collapses to one row carrying the asked variety.
A recording tagged with another variety is dropped, since the row it would fill belongs elsewhere.
A source left with nothing gets one addressless recording under its position, reached as its answer was.
So the menu still names that source rather than leaving its row silently absent.
The recordings are walked in source position runs, which is how the harvest returns them.

## `private static Task<LAnswer> LHarvestAnswerRead(LSource source, string word, CancellationToken cancellation)`

The recording counterpart of `LLookupAnswerRead`.
