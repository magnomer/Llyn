# Etymology

These tests cover the `CEtymology` gate, the Conduct face of an entry's etymology map.
They run over a real workspace and a held draft, so every answer is what the engine really holds.

## `TEtymology.cs`

It covers each edit of the map and the rows `CEtymologyRead` answers after it.
A row carries its depth, its text, whether it is linked, borrowed or restorable, and its language, gloss and relation.
An empty desk answers no rows.
Inserting with no box answers one empty text row on the headword.
Inserting above a box answers the new box one level higher.
Adding answers a co-source beside the box, at the same depth.
Removing a box answers its source in its place.
A typed text or gloss lands once the deferred work lands, so the tests settle it first.
A text typed on a linked box drops the link at once, since the typed text is now the answer.
Linking a box with no source answers the linked entry's headword and language with no undo.
Linking a box with a source sets the source aside and offers an undo.
That read signals once, after the undo settles, and carries the relation and language lists with it.
Linking an entry that has its own map answers its boxes as borrowed rows one level above the link.
A borrowed row is never restorable.
Restoring a set-aside source returns the text and the source.
A language typed free is answered trimmed, and a custom tongue's name is answered as its language.
`TEtymologyEditorPrepare` builds the editor fixture over a saved entry, and `TEtymologySave` stores one bare entry.

## `TEtymologyDisplay.cs`

It covers the read that serves the display card and the failure paths of both reads.
Two boxes that link one entry answer four rows, two of them borrowed copies of one box.
Each row key stays unique, because a key is the path of box ids from the root.
A stored entry's rows answer from the display gate with the plain id as the key.
When the engine fails, the read shows one failure notice and answers no rows.
The editor read asks `Input.AncestryFailed`, and the display read asks `Display.AncestryFailed`.

## `TEtymologyList.cs`

It covers the relation and tongue lists the pickers show and the prompts around their edits.
The relation list answers the built-ins, then the custom ones, then one addition row.
The language list answers the app languages, then the tongues, then one addition row.
Picking the addition row asks for a name through the coinage prompt.
The answer creates the item and sets it on the box.
A declined prompt creates nothing and leaves the box as it was.
Deleting a used custom item asks a confirmation with the count, then clears it from every box.
A declined confirmation keeps the item.
Deleting a built-in relation shows no prompt.
Deleting an unused tongue still confirms, then deletes.
`TEtymologyStoredRead` reads the map the store holds, so a test can tell a cleared store from a cleared view.

## Stamp

Commit: `2f9a06b8eb95`
- `TEtymology.cs` `358aa907a8d96520`
- `TEtymologyDisplay.cs` `b03edd16391799ce`
- `TEtymologyList.cs` `71792b146951b7d6`
