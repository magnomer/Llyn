# Pedigree

These tests cover an entry's etymology map.
They span the Ancestor boxes, their clerk and saving, and the Lineage tree read from them.

## `TAncestor.cs`

It covers the edits a held draft applies to its map of Ancestor boxes.
A box names the box it feeds as its child, and a box with no child is a root.
Inserting above a box takes over that box's sources, and inserting above nothing takes over the roots.
Adding beside a box shares its child and lands after it.
Removing a box hands its sources its place.
A box linked to an entry sets its sources aside, and a text edit clears the link.
Restoring a pick returns the text, the language and the set-aside sources.
Restoring twice, or after a text edit, is refused as an item error.
A linked box refuses a new source and a language with a link refusal, since its entry answers for both.
A link to an unknown entry is refused as a link error.
An unknown tongue, relation or box is refused as an item error.
Choosing a tongue clears the app language, and choosing the relation of zero clears the relation.
`TAncestorTreeCreate` builds a root with two sources and returns the three ids.
`TAncestorHeldRead` reads the open draft's map, and `TAncestorEntryCreate` stores one bare entry.

## `TAncestorClerk.cs`

It covers the relation and tongue lists the clerk keeps beside the boxes.
The built-in relations come first, then the typed ones.
A typed name is kept trimmed, and a name that matches a built-in returns the built-in.
The language list holds the app languages first, then the added tongues.
A tongue named like an app language returns that app language and adds no row.
A listed item counts the boxes that use it.
Deleting a used custom item clears it from every box and returns the count.
Deleting a built-in relation or a missing tongue returns nothing and changes no box.
`TLanguageListed` is a language vault that lists two app languages.

## `TAncestorSaving.cs`

It covers what a commit stores of the map and the revision it records.
An edited map stores in order with sources under their child, and a new entry stores its map at once.
A first map records a create, a changed map an update and a cleared map a delete.
The create's summary lists the box texts, here the root's alone.
A map of the same shape under new keys records no revision.
The compare reads shape and not keys, so a removal and re-insertion that changes nothing stays silent.

## `TLineage.cs`

It covers the Lineage tree the clerk reads from a draft's map for display.
A linked box shows its entry's current headword and language, whatever text it stored.
An entry that is gone shows the stored text and no link.
A linked entry's own stored map joins the tree as borrowed sources of the linked box.
Sources and co-sources keep draft order.
A tongue box shows the tongue's name as its language.
Borrowing stops after `LLineageClerkDepth` levels above the draft box, so a long chain of linked entries ends.
An entry already on the path shows once more and then stops, so a cycle cannot loop.

## Stamp

Commit: `c6e60141a172`
- `TAncestor.cs` `4ada25a33d377d64`
- `TAncestorClerk.cs` `32e5a74d6c4fc997`
- `TAncestorSaving.cs` `8e0e044061e8fb46`
- `TLineage.cs` `ab0ce0d8b348f71d`
