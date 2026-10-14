# Pedigree

The Pedigree folder holds the engine's facade over an entry's etymology map.
It serves the relation and tongue lists and the Lineage tree through the Ancestor port.
It sits apart from the record facades, since the map is a concern of its own.

## `LAncestorFacade`

Every call takes the engine gate before it reaches a clerk, as the other facades do.
Reading the Lineage takes the entry id and the draft map, so the tree shows edits not yet saved.
Deleting a relation or tongue answers the count of boxes that used it, and null when nothing was deleted.
The facade adds no rule of its own.
Each call forwards to the Ancestor clerk or the Lineage clerk of the entry staff.

## Stamp

Commit: `c6e60141a172`
- `LAncestorFacade.cs` `90f7617330604e58`
