# TCitationRow.cs

## `public sealed class TCitationRow`

Covers the rows the corpus citation field offers for a typed word.
A byline holding the word splits around its first match, whatever the case.
A byline without it reads whole, and a reference nothing cites shows no count.
At most eight rows are offered in found order.
The engine drops a blank word and the cited Source's byline, which `TAnthology` covers.

## `private static CCatalogReference TCitationSourceCreate(long id, string byline, int usage)`

One found reference with its byline as its title, unknown credit and year, and the given usage.
