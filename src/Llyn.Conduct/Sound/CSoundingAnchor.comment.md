# CSoundingAnchor.cs
Hash: `a130b046675f544f`

## `public sealed class CSoundingAnchor`

The anchor menu of the editor's reflex rows, one row at a time.
It is split from `CSounding` by role, since its menu holds a selection and the `CSounding` reads hold none.
The chosen row is the session's selection, so it lives here and no driver holds it.
The editor's driver builds one beside the `CEditor` it shows.

## `private readonly CTimbre _cSoundingAnchorTimbre;`

The editor's timbre, whose reflex quill sends the anchor edit.
The quill needs the reflex port the timbre holds, so the menu borrows it rather than building one.

## `private long? _cSoundingAnchorReflex`

The reflex row the open menu edits, or null while the menu is closed.

## `public static CSoundingAnchor CSoundingAnchorCreate(CEditor editor)`

The anchor menu over the timbre of `editor`, built by the driver that shows that editor.
It stands outside `CSounding`, whose reads hold no selection, and outside `CEditor`, whose gates are the entry's.

## `public CAnchor CSoundingAnchorOpen(long reflex)`

The user opened the anchor menu of the reflex row `reflex`.
The row becomes the menu's selection, and the menu answers the readings the row may anchor to.
The timbre's reflex quill reads the row's anchors, language and tone off the draft.
So the driver hands only the id.
The rows stay empty while the desk fills its view or holds nothing stored.
The menu then shows its notice, as it does whenever no reading is offered.

## `public void CSoundingAnchorSet(long fanqie, bool anchored)`

The user ticked or cleared one reading in the open menu.
It ties or unties the selected row and that reading through the timbre's reflex quill.
It does nothing while the menu is closed or the desk fills.
The row repaints from the draft change, so the gate answers nothing.

## `public void CSoundingAnchorClose()`

The menu closed, so no row stays selected.
