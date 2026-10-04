# LLookup.cs
Hash: `75ae370208c8aaae`

## `public sealed class LLookup : LSeeker`

Fans a pronunciation request out to every source it was given at once.
It runs the whole set, because the caller hands it the transcription sources alone.
Each candidate carries the position its source holds in that set, so a slow source keeps its declared place.
Streams each result back to the receiver as it arrives.
One slow source never blocks the others, and one failing source never stops them running.
A source that answered produces one candidate per reading, all sharing the source's position.
Each candidate carries its reading's variety.
The readings pass through `LReading.LReadingScan` first, so an untagged reading fans out over the pack's declared varieties.
Each fanned-out reading is then normalized and run through the pack's cleanup groups before it becomes a candidate.
The fan-out comes first so a cleanup group scoped to one variety sees the variety it targets.
Cleanup has no switch except a literal lookup, so every other candidate leaving here carries cleaned text.
The returned set therefore holds cleaned text too, and a cache built on it never needs a second fetch.
The user's optional respelling is not applied here, because the cache must stay free of it.
A literal lookup skips the normalization and the cleanup both.
It keeps each reading as the source wrote it, apart from trimming.
That is the transcription path, where a scheme such as Pinyin keeps its spaces and knows no IPA cleanup.
A source that had nothing or was never reached still produces one candidate with no phonetic.
So the menu can name a broken source rather than leaving its row silently absent.
The returned set is ordered by source position with a stable sort.
So readings keep their order inside a source.
Each source runs under its own minute-long deadline, linked to the caller's cancellation.
A host that neither answers nor refuses is written off at that deadline instead of holding the whole lookup open.
The deadline is per source, so one dead host never eats another's budget.
The lookup reports complete once all sources have finished.
It also returns the whole set it streamed, in source order, for a caller that wants to keep it.
Collecting it here costs nothing, since every candidate already passes through this one place.
The sources are supplied ready-built and language-agnostic (see `LSource`).
This orchestrator knows nothing about any particular source or language.

## `public LLookup(IReadOnlyList<LSource> sources, IReadOnlyList<LVariety> varieties, IReadOnlyList<LRespelling> cleanups, bool literal = false)`

Holds the sources, the pack's varieties and cleanup groups, and whether the lookup is literal.
A null list refuses construction.

## `public async Task<IReadOnlyList<LCandidate>> LSeekerStart(string word, LReceiver receiver, CancellationToken cancellation)`

Starts every source at once and waits for all of them.
A cancelled lookup withholds `LReceiverLookupFinish`, since the menu it would close is already gone.
Every other ending reports it, a source failure included.
The lookup started the receiver, so it alone ends it, and the caller never reports the end again.

## `private static async Task LLookupSourceRun(LSource source, int order, string word, IReadOnlyList<LVariety> varieties, IReadOnlyList<LRespelling> cleanups, bool literal, LReceiver receiver, List<LCandidate> found, CancellationToken cancellation)`

Runs one source and streams its candidates to `receiver`.
It announces the source first, so the menu can show the row while the answer is pending.
It adds the candidates to `found` under a lock before streaming them.

## `private static async Task<LAnswer> LLookupAnswerRead(LSource source, string word, CancellationToken cancellation)`

Runs one source under its own deadline.
The caller's own cancellation escapes, because that ends the lookup rather than one source.
Only the deadline becomes an answer saying the source was never reached.
Any other failure propagates, so the caller records it instead of hiding it as a lost answer.
Reading the deadline off the outer token instead would have failed the whole search over one dead host.

## Inline notes

### `cancellation.ThrowIfCancellationRequested();`

A source that answered after the user closed the menu reports nothing.
The row it would fill is already gone.
