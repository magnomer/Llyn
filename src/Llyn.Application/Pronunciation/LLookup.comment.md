# LLookup.cs

## `public sealed class LLookup : LSeeker`

Fans a pronunciation request out to every source it was given at once.
It runs the whole set, because the caller hands it the transcription sources alone.
Each candidate carries the position its source holds in that set, so a slow source keeps its declared place.
Streams each result back to the receiver as it arrives.
One slow or failing source never blocks or fails the others.
A source that answered produces one candidate per reading, all sharing the source's position and each carrying its variety.
A source that had nothing or was never reached still produces one candidate with no phonetic.
So the menu can name a broken source rather than leaving its row silently absent.
The returned set is ordered by source position with a stable sort, so readings keep their order inside a source.
Each source runs under its own minute-long deadline, linked to the caller's cancellation.
A host that neither answers nor refuses is written off at that deadline instead of holding the whole lookup open.
The deadline is per source, so one dead host never eats another's budget.
The lookup reports complete once all sources have finished.
It also returns the whole set it streamed, in source order, for a caller that wants to keep it.
Collecting it here costs nothing, since every candidate already passes through this one place.
The sources are supplied ready-built and language-agnostic (see `LSource`).
This orchestrator knows nothing about any particular source or language.

## `private static Task<LAnswer> LLookupAnswerRead(LSource source, string word, CancellationToken cancellation)`

Runs one source under its own deadline and never lets it throw at the caller.
Only the caller's own cancellation escapes, because that ends the lookup rather than one source.
Everything else, the deadline included, becomes an answer saying the source was never reached.
Reading the deadline off the outer token instead would have failed the whole search over one dead host.

## Inline notes

### `cancellation.ThrowIfCancellationRequested();`

A source that answered after the user closed the menu reports nothing.
The row it would fill is already gone.
