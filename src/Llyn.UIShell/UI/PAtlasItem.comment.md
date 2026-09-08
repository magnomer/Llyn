# PAtlasItem.cs

## `internal sealed class PAtlasItem`

Presentation item for one Situation row in `PAtlas`.
Carries the title, the kind, and the usage count the row shows, and the Situation id the row loads through.
The id is identity and never displayed.
Two Situations may carry the same title, so a row is never found by what it reads.
A title standing empty is not one thing.
It may never have been written, or it may have been written and be unreadable now.
The row is given the mark for the second so the two stay distinct in the catalog.
It is given the untitled text for the first so no row stands blank beside the next.

## `internal PAtlasItem(LSituation situation, int usage, string unreadable, string untitled)`

Builds the row from the stored Situation and the number of places referencing it.
The two texts are handed in rather than read here.
A row is built while the list is being filled.

## `public string PAtlasItemCount { get; }`

How many Meanings and Collocations reference this Situation, as the row shows it.
It is the figure that decides whether a delete is legal.
The catalog carries it and not only the display.
