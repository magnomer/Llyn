# LAuthorUsage.cs

## `public sealed class LAuthorUsage`

Reads the citing side of an Author: which cards and Examples cite a Source they are credited on.
It lives beside `LAuthorArchive` rather than inside it, as `LReferenceUsage` lives beside `LReferenceArchive`.
Tracing back up to the Entries is a different responsibility from storing an Author.
Nothing here creates, changes, or deletes anything.

The trace is computed on read and stored nowhere.
No table links an Author to an Entry, so no stored row can contradict the chain.
Every hop is already indexed: `source_author_member`, `example_source`, then the two card member indexes.
The return type is `LUsage`, which no write path accepts, so a trace cannot become an input.

An Author credited on no Source, or on a Source no Example cites, traces to nothing.
That is correct under the chain rather than a fault, and a tab renders it as not cited yet.

## `public LAuthorUsage(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public IReadOnlyList<LUsage> LAuthorUsageRead(long id)`

Reads the Meanings, Collocations and Examples citing any Source the Author is credited on.
A card names its own id, the Entry it belongs to, that Entry's headword, and its title.
An Example names its sentence and its translation, because an Example belongs to no Entry of its own.
A card is listed once however many credited Examples it holds, because the row leads to the card.

The count of an Author is the Example rows, which are the citations themselves.
The card rows are the way up to those citations rather than additions to them.
Counting cards and Examples together would count one citation twice, as `LReferenceUsage.comment.md` records.
