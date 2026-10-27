# Yunjing

This folder holds the yunjing panel's gates.
They cover the rime table, its diwei pages, portrait and vista.

## `TYunjing`

Covers the yunjing panel's gates end to end on a real workspace.
The cells are placed straight into the store, so nothing here reaches the web.
`TYunjingVista` covers the vista restore and its observers, and `TEntryList` covers the list's close.
`TYunjingDiwei` covers the shown page, and `TYunjingPortrait` covers export and print.
`TYunjingPack` names one fixture language, in which the tests place their cells.
`TYunjingBook` is the language the bundled rime-book pack declares.
The flag-fill load answers the cell's entries and the catalog languages once the fill has run.
The bundled rime-book pack shows the tab, and with nothing placed the panel lists nothing under the bare keys.
With no cell chosen, the columns list the first language declaring a book and the entry list stays empty.
Opening a cell by key lists its language with the cell chosen, and shows its page and its entries.
The page key names the column the opened cell belongs to, initial or rime.
An unknown key opens nothing and raises nothing.
An arrival empties both column searches before the driver hears of it.
Choosing the chosen rime again hides the page and empties the entry list.
A click without an id or a side does nothing.
Choosing a cell over an unsaved draft asks the leave question first.
A kept draft leaves the cell, the draft and the change event as they were.
A discarded draft closes the editor and raises the change once.
Unmatched queries read the unmatched keys, and a narrowed column unchooses its cell.
Both columns start ordered by name, and a null order keeps the chosen one in each.
An entry opened in scribe mode shows in the editor, and closing the entry empties it.
The order menu lists name, reverse and usage, in that order.
`TYunjingPrepare` builds the panel over a fake envoy that answers no, or over a given envoy.

## `TYunjingDiwei`

Covers the page a yunjing cell shows, on a real workspace.
It builds its panel through `TYunjing.TYunjingPrepare`.
The page of an initial cell groups its lines by division, with the characters sorted and the switch flags copied.
The section label is localized from a shared catalog, so `TDiwei` checks it with a localizer handed in.
The page carries the headword and glyph fonts of its language, and the blank page carries none.
Under the bundled book language, a Korean reading under the cell reaches the page as one tally.
The tally switch is saved once, and a switch without a side changes nothing.
A refused tally save is shown as `Settings.SaveFailed`, and the saved switch stays off.
A glyph opens in the library tab only while a cell page shows, in the language of that page.

## `TYunjingPortrait`

Covers the yunjing panel's export and print of the chosen entry, on a real workspace.
It builds its panel through `TYunjing.TYunjingPrepare`.
Export writes nothing until an entry is chosen, and the chosen entry leaves the page for the reader.
Print does nothing while no entry is chosen.

## `TYunjingVista`

Covers the yunjing panel's vista restore and its observers on a real workspace.
A Workspace notice unchooses both columns, empties the entry list and tells the driver once.
A change in either column raises the columns once and changes only that column's rows or order.
No Conduct surface tells which column raised, since both share the one refresh event.
A Fanqie, Settings or Reflex notice raises the columns and the entry list.
An Entry notice also raises the panel.
A second restore carries the held queries into the fresh vistas.
After it a Fanqie notice raises the columns once, through the marshal.
A create alone restores once, so the same notice after only a create raises once too.
The atelier's close of the entry list is covered by `TEntryList`.

## Stamp

Commit: `9377e657b395`
- `TYunjing.cs` `09516b16c471d83c`
- `TYunjingDiwei.cs` `0ce664a0879b7dd9`
- `TYunjingPortrait.cs` `1a8534aff8a36dc9`
- `TYunjingVista.cs` `a54ce985a60a2ffd`
