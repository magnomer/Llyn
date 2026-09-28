# TYunjing.cs

## `public sealed class TYunjing`

Covers the yunjing panel's gates end to end on a real workspace.
The cells are placed straight into the store, so nothing here reaches the web.

The bundled rime-book pack shows the tab, and with nothing placed the panel lists nothing under the bare keys.
Opening a cell by key lists its language with the cell chosen, and shows its page and entry.
An unknown key opens nothing and raises nothing.
Choosing the chosen rime again hides the page, and a click without an id or a side does nothing.
The page of an initial cell groups its lines by division, with the characters sorted.
The tally switch is saved once, and a switch without a side changes nothing.
A glyph is raised only while a cell page shows, in the language of that page.
Unmatched queries read the unmatched keys, and a narrowed column unchooses its cell.
Cancelling unchooses both columns and empties the entry list.
A null order keeps the chosen one in each column.
Export writes nothing until an entry is chosen, and the chosen entry leaves the page for the reader.
An entry opened in scribe mode shows in the editor, and closing the panel empties it.

## `private static CYunjing TYunjingPrepare(CAtelier atelier)`

Builds the yunjing panel and restores its vistas, as the window does for the tab.
