# Panel

This folder holds what every browsing panel shares.
That is state, bin, row choice, aperture, catalog gates and filter.
It also holds the shared entry list, print and export, and the New prompt.

## `TAperture`

Covers the list slot of the browsing panel on a real workspace.
A vista the panel restores opens in the viewer, and the slot then holds that very vista.
The split is switched off by the panel's restore, not by the slot's.

## `TCatalog`

Covers the catalog gates over a workspace engine, over the fake rig, or over fake ports alone.
The consonant chart keys every header and side, and splits each cell into its symbols.
The vowel chart does the same over its own three columns and seven rows.
A hostile chart still gives one cell row per side, none wider than the headers.
So an empty chart, missing rows, extra rows and wide rows each keep the grid.
Those cases hand a raw engine chart through the interface, since the catalog's chart reader is internal.
The flag load hands the store Conduct rows and answers the loaded languages.
Its two cases hand it a fake envoy, since the gate takes one for its failure notice.
The shared fill rule reads an area's rows only once the fill completes.
The case holds the fill open with a completion it sets itself.
A fill that faults or throws shows the handed key through the envoy and answers no languages.
The rows are still read, so both cases expect the rows beside an empty language list.
On a workspace the flag load answers the same languages the catalog reads.

## `TCatalogFilter`

Covers the language filter a driver ticks in a catalog menu.
A hidden language fails the match whatever its case, and any other language passes.
A row with no language passes unless the empty name is hidden.

## `TCoinage`

Covers the naming dialog's gate.
Blank or whitespace-only wording names nothing.

## `TEntryList`

Covers the entry list shared by the xiesheng and yunjing panels, on a real workspace.
It builds its panels through `TXiesheng` and `TYunjing`, so their fixtures place the entries.
Its flag-fill load answers the series entries and the catalog languages once the fill has run.
The test hands a store that keeps nothing, so only the sheet is compared.
Export writes nothing until an entry is chosen, and the chosen entry leaves the page for the reader.
The exported file then holds the entry, and the offered name carries its headword.
An entry opened in scribe mode shows in the editor and puts its entry on the desk.
Closing the panel hides the editor and empties the desk.
The atelier's close lets each list's draft go and stops the recording once.

## `TPanel`

Covers the browsing panel's scribe gates end to end on a real workspace.
Leaving the scribe with unstored changes asks the envoy once.
Keeping stays in the scribe, storing finishes the draft, and discarding shows the viewer unfinished.
A clean editor never asks, and entering the scribe hands the chosen row to the editor.
The fresh gate drops the chosen row and opens the scribe on nothing.
Restoring the scribe needs a row.
The order and subject maps carry every member by name, and the filter map carries its hidden languages.
Row choice and stored-entry notices live in `TPanelRow`, the delete gate in `TPanelBin`, and the vista restore in `TAperture`.

## `TPanelPrepare`

Builds a panel over a library vista standing on one stored entry.
The envoy answers a given answer and records each question.
The finish seam records each store, and a callback says whether the editor holds changes.
`TPanelRow` and `TPanelBin` build their panel through it.

## `TPanelBin`

Covers the browsing panel's `CPanelBin` delete gate end to end on a real workspace.
The gate asks through the envoy, keeps everything on a refusal and drops the row on a yes.
A panel with no delete scope never deletes, and a referenced record is asked about with its tally.
Each test builds its panel through `TPanel.TPanelPrepare`, except the credited author, which builds a guild panel itself.

## `TPanelRow`

Covers how the browsing panel chooses a row and follows the stored entry, end to end on a real workspace.
A refused leave keeps the row the panel stands on.
A row select answers opened, stayed or failed.
A cancelled leave answers stayed, and a list that no longer loads answers failed with `List.LoadFailed`.
A select with no id answers failed.
A row click asks the leave question before it records the station.
A declined leave records no station and opens nothing.
Opening a stored row chooses it, and in edit mode hands it to the editor.
Opening a row that no longer loads closes the panel without a failure notice.
A quiet reload repaints a stored entry and closes the panel on a deleted one.
A quiet reload the engine fails shows the panel's load key once and repaints nothing.
A stored-entry notice re-lists the rows and repaints the mode once each.
A stored entry is adopted only by a panel editing on nothing.
Each test builds its panel through `TPanel.TPanelPrepare`.

## `TPortrait`

Covers what every panel's print and export gate shares through `CPortrait`.
The export format maps by name, and an unknown member throws.
Every label and legend word is the engine's wording of a key Conduct chose.
The fake wording answers `text:` plus the key.
The unit, freedom and source kind names are worded the same way.
The legend holds one kind name per reference kind.
The formats offered and the default one come from the engine.
Hostile engine rows still give exactly one chosen format and no `|` in a suffix.
No rows give the markup format, no chosen row chooses the first, and many keep the first.
An unknown engine format is a programming mismatch, so reading it throws.
A declined file question does nothing.
A print with nothing printable opens no window and asks nothing.
A failing export or print call shows `Export.Failed` or `Print.Failed` through the envoy.
A failing name read shows `Export.NameFailed` and exports nothing.
An export handed no settings port throws.
A print window that closes or throws still disposes the relay's board.
Those facts open the session through the real print route, over a fake board they hold.
A throwing window escapes to the caller, since only the open call is guarded by `Print.Failed`.

## Stamp

Commit: `fe7e11daae88`
- `TAperture.cs` `5ec65d9a18316bee`
- `TCatalog.cs` `490668e03758f42c`
- `TCatalogFilter.cs` `d3f1b60b77c6957d`
- `TCoinage.cs` `28e73c2c1f3b58a7`
- `TEntryList.cs` `3ed9071e02cf0aad`
- `TPanel.cs` `75b98cf0395da741`
- `TPanelBin.cs` `30b10a6f864ccc72`
- `TPanelRow.cs` `dbd20cdc2c054230`
- `TPortrait.cs` `82c5fca76ce1769b`
