# PCitationItem.cs

## `internal sealed class PCitationItem`

One Source offered in a row's Source list.
That is the id the row cites when this one is picked, and the name shown for it.
The list is the workspace's whole shelf of Sources.
So the same row object is offered to every Example and Situation row on the form.

## `internal static PCitationItem PCitationItemCreate(LReference reference)`

Reads a stored Source into the row that offers it.
The name is the one the Source states for itself, which the record answers rather than this row.
So the shelf offered here and the catalog browsed elsewhere never name one Source two ways.
