# Library

The console's Library tab, a searchable entry list beside the reading area.
The folder draws and routes keys.
Every decision about entries comes from the `CLibrary` and its `CPanel`.

## `BIndex`

The list is generic over its row.
It knows only four cells of a row, which are name, epithet, language and chosen.
It owns the lit row and the search field, and nothing else about the rows.
The choose callback is required.
The climb, the mark and the shift are optional, and a missing one turns its key off.
A missing query turns typing into a jump to the next matching name.
So the same list serves other tabs.
The lit row follows the chosen row until the user moves, then the lit row is kept.
A new query clears the lit row so it returns to the chosen row.
A refresh that brings a different chosen row clears it too, so the lit row follows a choice made elsewhere.
Enter chooses the lit row and answers false on an empty list, so the key can fall through.
The list does not close or follow the lit row afterwards.
The caller's choose decides what the choice means.
Escape empties a non-empty query and otherwise answers false for the window to use.
Backspace edits the query, and on an empty query climbs out.
Without a query callback, a letter jumps to the next name that starts with it, wrapping round.
Control and Alt keys and control characters are never taken as search text.
The chosen row wears a mark in its first cell, except while it is the lit row.
The language column is at most a quarter of the inner width.
So a long language is cut before it takes the name's room.
Names and languages are cut by cells, not characters.
The page size is set at each draw and read by PageUp and PageDown.
With no rows, the list shows the aperture's notice centred, and draws nothing when the notice is empty.
The window passes the notice only while the aperture is empty, so a failed read shows the empty notice.
The bottom edge shows the position, the order and the filter mark.
The list draws nothing under 3 cells wide or 4 high.

## `BLibrary`

The tab itself, joining a `BIndex` to a browse panel.
The Library tab creates a `CLibrary` here with the shown test.
The Favorite tab hands in a `CFavorite` builder instead, and shares every other line of the tab.
So the panel, editor, keys, row loads and wording all arrive from the owner.
This class names no panel type in its body.
The order wording takes the owner's series key, so the favorites tab words its orders apart from the Library.
The first four orders take Ctrl with F3 to F6.
So keys, menus and drawing go quiet when this tab is not shown.
Each verdict comes from `CPanel` or the desk draft.
Whether a key row is live, dim or hidden is read from them on every call.
The window never copies it.
Choosing a row asks `CPanel.CPanelRowSelect`.
When it answers that the panel stayed, the lit row is cleared.
That happens when the user cancels the leave prompt or the save fails.
The lit row then returns to the chosen row.
Failure to open a row leaves the lit row where it is, and the envoy words the failure.
The rows are refreshed when the aperture reports a change and when the workspace opens.
Ctrl with F3 onward orders by each order in turn, Ctrl+F7 opens the order chooser and Ctrl+F8 the language chooser.
F4 and F8 are edit or view and delete, and F2, F7 and F6 come from the tab's `BEditor`.
So save and the editor's undo work in the Library and Favorite tabs as they do in the Input tab.
Edit and delete show dim when the panel will not allow them.
New and view never show dim.
New is absent when the owner offers no new entry.
Import is absent when it offers no import, so the Favorite tab shows neither.
The two F4 rows share a key, and only one is shown at a time.
The tab's own F7 is New, and the editor's F7 undo stays hidden because the editor is not owned here.
Each key row calls the changed callback after its action.
`BLibraryObserve` calls it after every key it hands to the list.
The Entry menu holds import and export, and export is live only while the panel allows a press.
The View menu holds the same two choosers as the keys.
The menu hotkey is the one-letter catalog word if it is an ASCII letter, and none otherwise.
The order chooser closes itself on a choice, by completing the task it hands to the docket.
The language chooser has no such task, so it stays until the user closes it.
The language list is whatever the last load on workspace open returned.
The tab owns a `BEditor` and builds it through a callback its owner supplies.
So the Library and Favorite tabs each get an editor over their own `CEditor`.
The editor makes the `BLectern`, the reading view, and hands it the whole atelier, not only its navigation.
So the lectern can open a note from the page.
The display part shows the lectern while the viewer shows, and the editor frame while the scribe shows.
With no entry open the reading view draws the select prompt.
The tab holds which side has the keys, the index or the display side.
The display side is the lectern while the viewer shows and the editor while the scribe shows.
`BLibraryChronicle` hands the window the editor's undo and redo flags.
It answers null unless the tab is shown and the scribe shows.
`BLibraryKeymapFocus` tells the window this, so a key the display side does not bind never falls through to the index.
Tab hands the keys to the display side in either mode, and Tab there hands them back.
The index draws as focused only while it has the keys.
F6 toggles the favourite mark, live only while the viewer shows and an entry is open.
The toggle can drop the row from a favorites list, so the index settles its lit row around it.
The settle keeps the lit row on its entry while the entry stays.
Else it lights the row now at that place.
As in the GUI, only an opened entry moves anything.
Opening a row resets the reading view and gives it the keys.
Enter on the index thus hands the focus to the display side.
A row that only stayed gives the display side nothing.
F3 widens the display side to the whole tab, and gives it the keys.
The index then draws nothing, and Tab no longer hands the keys back.
F3 or Escape on the display side narrows it back.
The editor takes Escape first while its field is open.
So the first Escape closes the field and the next narrows.
The whole view counts only while the tab is shown.

## Stamp

Commit: `c05429890cec`
- `BIndex.cs` `9f0dd2c4a8edf784`
- `BLibrary.cs` `cbf70905486c25bf`
