# PAnthologyItem.cs

## `internal sealed class PAnthologyItem`

Presentation item for one Example row in `PAnthology`.
Carries the sentence, its language and flag, the cited Source, the translation count and the usage count.
The id is identity and never displayed.
Two Examples may carry the same sentence, so a row is never found by what it reads.
A sentence standing empty is not one thing.
It may never have been written, or it may have been written and be unreadable now.
The row is given the mark for the second so the two stay distinct in the catalog.
It is given the unwritten text for the first so no row stands blank beside the next.

## `internal PAnthologyItem(LExample example, int usage, string source, string unreadable, string unwritten)`

Builds the row from the stored Example and the number of places quoting it.
The Source arrives already resolved, because resolving it needs the shelf the panel holds.
The two texts are handed in rather than read here, because a row is built while the list is being filled.

## `public string PAnthologyItemUsage { get; }`

How many Entries, Meanings and Collocations quote this Example, as the row shows it.
It is the figure that decides whether a delete is legal, so the catalog carries it and not only the display.

## `public string PAnthologyItemTranslation { get; }`

How many translations the Example owns.
A translation lives and dies with its Example, so the count is a property of the row rather than of its use.
