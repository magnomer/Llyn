# CSoundingAnchor.cs

## `public sealed class CSoundingAnchor`

The anchor menu of the editor's reflex rows, one row at a time.
It is split from `CSounding` by role, and its gates keep the `CSounding` base, as `CDisplaySound` keeps `CDisplay`.
The chosen row is the session's selection, so it lives here and no driver holds it.
The editor's driver builds one beside the `CEditor` it shows.

## `private long? _cSoundingAnchorReflex`

The reflex row the open menu edits, or null while the menu is closed.

## `public static CSoundingAnchor CSoundingAnchorCreate(CEditor editor)`

The anchor menu over the desk of `editor`, built by the driver that shows that editor.
It stands outside `CSounding`, whose reads hold no selection, and outside `CEditor`, whose gates are the entry's.

## `public CAnchor CSoundingAnchorOpen(long reflex)`

The user opened the anchor menu of the reflex row `reflex`.
The row becomes the menu's selection, and the menu answers the readings the row may anchor to.
The tenure reads the row's anchors, language and tone off the draft, so the driver hands only the id.
The notice shows when no reading is offered.

## `public void CSoundingAnchorSet(long fanqie, bool anchored)`

The user ticked or cleared one reading in the open menu.
It ties or unties the selected row and that reading, and does nothing while the menu is closed.
The row repaints from the draft change, so the gate answers nothing.

## `public void CSoundingAnchorClose()`

The menu closed, so no row stays selected.
