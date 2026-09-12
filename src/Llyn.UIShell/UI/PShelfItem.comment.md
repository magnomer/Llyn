# PShelfItem.cs

## `internal sealed class PShelfItem`

Presentation item for one Source row in `PShelf`.
Carries the resolved name, the credited authors, the year, and the citation count.
The id is identity and never displayed.
A Source is shown by the first field it actually states, then by its id if it names itself nowhere.
It is never offered as a blank row the reader could not tell from the next one.
`Untitled` is a stated title and shows as one, and `Anonymous` is a credited Author.

## `internal PShelfItem(LCatalogReference row, string unknown, string unset)`

Builds the row from the catalog row the engine returned.
The name, the credits and the citation count arrive with it, so nothing is derived or read again here.
The two texts are handed in rather than read here.
A row is built while the list is being filled.

## `internal static string PShelfCreditRead(LReference reference, IReadOnlyList<LAuthor> credits, string unknown, string unset)`

Names the credits in the Source's own order, or shows the author state where there are none.
The read area shows the same line, so the rule lives here rather than twice.

## `public string PShelfItemCount { get; }`

How many Entries and Examples cite this Source, as the row shows it.
It is the figure that decides whether a delete is legal.
The catalog carries it and not only the editor.

## `public bool PShelfItemChosen`

Whether this row is the one the panel stands on, which the row template paints an accent edge for.
It is the only value of the row that changes after the row is built.
The panel sets it instead of refilling the list, so the catalog keeps its scroll position.
