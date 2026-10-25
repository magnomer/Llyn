# Library

Tests of the Library tab's list, keys, menus and window routing.
`TLibrary` holds the shared set-up, which starts a real engine, an atelier and a fake envoy.
A test may pass a seed that fills the engine before the tab opens.

## `TIndex`

Drives the list alone with a small catalog and three rows.
A chosen row that is not lit wears the pending mark, and a lit one does not.
The filter mark shows only when the list is told it is filtered.
A narrow list cuts the language by cells.
A focused list lights its title and row.
An empty list shows the notice it is given, and draws no words when given none.
A list whose read failed draws only blank rows.
A refine with the rows reordered keeps the lit entry.
A refine that drops the lit entry falls back to the chosen row.
Enter runs the lit row's callback and answers true, and Insert marks the lit row and moves down.
Typing sends the query on, and Backspace on an empty query climbs out only if a climb is given.
Escape empties the query once and then answers false.
Ctrl+Down shifts the lit row one place down.
Enter answers false on an empty list, and Insert and Ctrl+Down answer false when no callback is given.
Without a query, a letter jumps to the next matching name.
The glyph test pins that the ascii, double and single glyph sets carry the list marks with the same counts.
The marks include the tree marks, which read `|- `- |` in ascii and `├─ └─ │` in single lines.

## `TLibrary`

Drives the whole tab through a fake envoy.
Arrows alone select nothing, and Enter selects once and records one navigation station.
After a failed select the lit row stays and the prompt still shows.
Ctrl+F4 orders the headwords in reverse.
Each Enter records one station, and moving alone records none.
Down then Enter on a two-row list records one station.
A chosen entry that fails to load leaves the prompt up, and the delete key stays live.
Ctrl+F7 and Ctrl+F8 each open a `BSParley` box, and only the order one carries a task that ends it.
The menus are Entry and View, with their hotkeys.
When the tab is not shown, keys, drawing and menus do nothing.

## `TLibraryRail`

Pins the key rows against the panel's state.
F2, F4 and F8 start dim, and reading an entry lights F4 and F8 but not F2.
With the tab not shown, F7, F2, F4 and F8 are all hidden.
F8 asks before deleting, and F7 then F4 opens and then closes a new draft.
A query that matches nothing shows the unmatched notice, not the empty one.
An empty workspace centres its notice.
Through the window, a key on another tab never reaches the library.
A dim row swallows its key and makes no repaint.

## `TLibraryRoof`

Drives the Entry menu items.
Import asks for a file, then the customs, then the omissions, and the list repaints with the new entry.
A cancelled import asks nothing more.
A malformed file shows the import failure with its reason.
Export is live only while an entry is read, and not in the editor.
Export writes the chosen file, shows nothing when cancelled, and shows a failure when the path cannot be written.
These tests wait on the loop for the async work, up to 10 seconds.

## Stamp

Commit: `cb8ab7e8feb4`
- `TIndex.cs` `dadf98e3d8f50222`
- `TLibrary.cs` `6535a9c30cf4468d`
- `TLibraryRail.cs` `1b299a9ddc78a139`
- `TLibraryRoof.cs` `d91256827a5a974b`
