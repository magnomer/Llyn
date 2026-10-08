# CSoundingAnchor.cs
Hash: `e74df300b44bd469`

## `public sealed class CSoundingAnchor`

The anchor menu of the editor's reflex rows, one row at a time.
It is split from `CSounding` by role, since its menu holds a selection and the `CSounding` reads hold none.
The chosen row is the session's selection, so it lives here and no driver holds it.
The editor's driver builds one beside the `CEditor` it shows.

## `private readonly CKindred _cSoundingAnchorKindred;`

The editor's reflex block, whose reflex quill sends the anchor edit.
The quill needs the reflex port the block holds, so the menu borrows it rather than building one.

## `private long? _cSoundingAnchorReflex;`

The reflex row the open menu edits, or null while the menu is closed.

## `internal CSoundingAnchor(CKindred kindred)`

Keeps the reflex block and starts with no row selected.
The factory below is the public way to build it.

## `public static CSoundingAnchor CSoundingAnchorCreate(CKindred kindred)`

The anchor menu over the reflex block of `kindred`, built by the driver that shows its editor.
It takes only the kindred facet, the one part of the editor it reads.
The constructor checks `kindred` for null.
It stands outside `CSounding`, whose reads hold no selection, and outside `CEditor`, whose gates are the entry's.

## `public CAnchor CSoundingAnchorOpen(long reflex)`

The user opened the anchor menu of the reflex row `reflex`.
The row becomes the menu's selection, and the menu answers the readings the row may anchor to.
The reflex block's quill reads the row's anchors, language and tone off the draft.
So the driver hands only the id.
The rows stay empty while the desk fills its view or holds nothing stored.
The menu then shows its notice, as it does whenever no reading is offered.

## `public void CSoundingAnchorSet(long fanqie, bool anchored)`

The user ticked or cleared one reading in the open menu.
It ties or unties the selected row and that reading through the reflex block's quill.
It does nothing while the menu is closed or the desk fills.
The row repaints from the draft change, so the gate answers nothing.

## `public void CSoundingAnchorClose()`

The menu closed, so no row stays selected.
