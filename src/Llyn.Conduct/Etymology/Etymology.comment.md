# Etymology

This folder holds the etymology map as the editor and the reading view read it.
The map is a tree of boxes.
Each box is a word with its tongue, its gloss and its relation to the box it feeds.
Core's lineage supplies the tree, and Conduct flattens it for drivers to draw.

## `CAncestor`

One box of the map, ready to paint.
`CAncestorRead` flattens the lineage depth first, with each box's sources straight after it.
The rows keep that order, so a driver rebuilds the tree from the depth alone.
The key is the path of box ids from the root.
So a box keeps its key while its siblings change.
A repaint can then update a row in place, as the key names the same box.
A box is linked when it points at an entry.
A borrowed box is a copy of a linked entry's own map, so the editor shows it read only.
Only a linked box that is not borrowed can be restorable, and only the one the gate shelved last.
The walk uses its own stack, so a deep map cannot overflow the call stack.

## `CAncestorChoice`

One option in the relation list or the tongue list.
A relation option holds its id, its localization key and whether the user made it.
A built-in relation has a key, and a custom one has its own name.
The addition option is the last row of each list, and it makes a new relation or tongue.
It carries a null id, so choosing it never writes an id.

## `CEtymologyState`

Everything one repaint of the map needs, read in one pass.
It holds the rows, the relation choices, the tongue choices and the shown headword.
The headword defaults to blank, since only the reading view carries it.
No held draft answers three empty lists.

## `CEtymology`

This is the editor's gate for the map.
It acts only on the held draft, and it acts on nothing while the draft is still filling.
Every change but text and gloss raises `CEtymologyChanged`, so the driver repaints from one read.
Each state read goes through repaint memory under `Input.AncestryFailed`.
A refused read answers an empty list, so the editor still draws.

### `CEtymologyTargetSet`

A picked entry becomes the box's link.
The gate shelves the box only when it had sources of its own and the link took.
The shelf holds the draft it was set on, so only that draft offers the restore.
A refused link shelves nothing.
The change notice is raised either way.

### `CEtymologyRestore`

It unlinks the box and gives back the text, tongue and sources it held before the link.
It clears the shelf only when the shelved box is the one restored.
The engine refuses a box that is not the shelved one, so no other box can be restored.

### `CEtymologyTextSet`

A typed word is written to the box, and the answer is the mention offer for that text.
The editor reuses the card's prospect, so a source word is picked the way a mention is.
No notice is raised, since the box already shows the typed text.

### `CEtymologyLanguageSet` and `CEtymologyRelationSet`

The addition option asks the envoy for a new name first.
A cancelled or blank name changes nothing.
A refused create shows `Tongue.CreateFailed` or `Relation.CreateFailed` and changes nothing.
A created option is set on the box straight away.

### `CEtymologyRelationDelete` and `CEtymologyLanguageDelete`

Only a custom option can be deleted, and a built-in relation is refused even when asked.
A used option asks first how many boxes it detaches.
An unused one asks a plain delete question.
A declined or missing option changes nothing.
A refused delete shows `Relation.DeleteFailed` or `Tongue.DeleteFailed`.
After a delete, every box of the held draft that used the option is cleared.
So no box keeps an id or a tongue that no longer exists.

### `CEtymologyInsert`, `CEtymologyAdd` and `CEtymologyRemove`

Insert puts a new box above the given box, or above the headword when the id is null.
The engine refuses an insert above a linked box.
Add puts a new co-source right after the given box, sharing its child.
Remove takes the given box away.
Each one is a quill call on the held draft, and the notice follows it.

## Stamp

Commit: `c6e60141a172`
- `CAncestor.cs` `3c2b97b19aee4853`
- `CAncestorChoice.cs` `89298fe938ae145f`
- `CEtymology.cs` `c96a6a7b2b4a5072`
- `CEtymologyState.cs` `6b8a493710da06d8`
