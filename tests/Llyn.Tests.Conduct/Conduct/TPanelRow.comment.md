# TPanelRow.cs
Hash: `65d73c5e9eb981ad`

## `public sealed class TPanelRow`

Covers how the browsing panel chooses a row and follows the stored entry, end to end on a real workspace.
A refused leave keeps the row the panel stands on.
A row click asks the leave question before it records the station.
A declined leave records no station and opens nothing.
Opening a stored row chooses it, and in edit mode hands it to the editor.
Opening a row that no longer loads closes the panel without a failure notice.
A quiet reload repaints a stored entry and closes the panel on a deleted one.
A quiet reload the engine fails shows the panel's load key once and repaints nothing.
A stored-entry notice re-lists the rows and repaints the mode once each.
A stored entry is adopted only by a panel editing on nothing.
Each test builds its panel through `TPanel.TPanelPrepare`.
