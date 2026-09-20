# PCitationItem.cs

## `internal sealed class PCitationItem`

One Source offered in a row's Source list.
That is the id the row cites when this one is picked, and the `Author (Year)` line shown for it.
The list is the workspace's whole shelf of Sources.
So the same row object is offered to every Example and Situation row on the form.

## `internal static PCitationItem PCitationItemCreate(LCatalogReference row)`

Reads a browsed Source into the row that offers it.
The line is the byline the catalog row derived, which the record answers rather than this row.
So the shelf offered here, the example line and the citation dropdown never write one Source two ways.
