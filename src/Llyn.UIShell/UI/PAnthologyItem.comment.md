# PAnthologyItem.cs

## `internal sealed class PAnthologyItem`

Presentation item for one Example row in `PAnthology`.
Carries the sentence, its language and flag, the cited Source, and the usage count.
The id is identity and never displayed.
Two Examples may carry the same sentence, so a row is never found by what it reads.
A sentence standing empty is not one thing.
It may never have been written, or it may have been written and be unreadable now.
The row is given the mark for the second so the two stay distinct in the catalog.
It is given the unwritten text for the first so no row stands blank beside the next.

## `internal PAnthologyItem(LExample example, int usage, string source, string unreadable, string unwritten)`

Builds the row from the stored Example and the number of places quoting it.
The Source arrives already resolved, because resolving it needs the shelf the panel holds.
The two texts are handed in rather than read here.
A row is built while the list is being filled.

## `public string PAnthologyItemCount { get; }`

How many Entries, Meanings and Collocations quote this Example, as the row shows it.
It is the figure that decides whether a delete is legal.
The catalog carries it and not only the display.

## `public bool PAnthologyItemChosen`

Whether this row is the one the panel stands on, which the row template paints an accent edge for.
It is the only value of the row that changes after the row is built.
The panel sets it instead of refilling the list, so the catalog keeps its scroll position.

## `public string PAnthologyItemName`

The sentence as the row shows it, numbered `(1)`, `(2)` while another row carries the same sentence.
`PTwin` writes it once the list is filled, because a repeat is only visible across rows.
`PAnthologyItemText` keeps the plain sentence for everything that is not display.
