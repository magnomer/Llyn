# Pedigree

The Pedigree folder holds the clerks over an entry's etymology map.
They keep the relation and tongue lists, save the boxes with the entry, and read the Lineage tree.
They sit apart from the lifecycle clerks, since the map is a concern of its own.

## `LAncestorClerk`

The relation list shows builtin relations first, then the user's own.
Creating a relation or tongue trims the name and refuses a blank one.
A name that matches an existing one, ignoring case, returns the held value instead of a duplicate.
The tongue match looks across the app languages too, so a user tongue never repeats an app language.
Deleting a builtin relation does nothing and answers null.
Deleting a missing relation or tongue answers null too.
A real delete answers the count of boxes that used it.
The vault clears the value on those boxes, as the design promises.

## `LEntryClerkAncestor`

Saves the boxes of an entry draft inside the entry's own save.
A draft equal to the stored map writes nothing and adds no revision delta.
Otherwise the texts and glosses are trimmed before they are stored.
The delta says delete for an empty map, create when nothing was stored, and update otherwise.
Its value lists the box texts from the headword upward, empty texts left out.
`LAncestorMatch` walks both lists breadth-first from the headword and never compares ids.
A negative draft id and a stored id may so stand for the same box.
A list whose boxes cannot all be reached from the headword never matches, so it is saved again.
Text and gloss compare trimmed, and the language compares exactly.

## `LLineageClerk`

Reads the full tree for one entry from the draft map, so unsaved edits show at once.
At a linked box, the linked entry's stored map continues above it, marked borrowed.
Borrowing stops at depth eight.
It also stops at an entry already on the path, so a cycle between entries ends instead of looping.
A box id met twice in one map is skipped, so a malformed draft cannot loop either.
A linked box whose entry is gone reads as text.
Its language comes from the tongue list when the box names a tongue, and from the stored language otherwise.

## Stamp

Commit: `c6e60141a172`
- `LAncestorClerk.cs` `02bc4321c296cd66`
- `LEntryClerkAncestor.cs` `b93eb870dbff10be`
- `LLineageClerk.cs` `79a253bcac2e3c32`
