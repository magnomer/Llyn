# Favorite

This folder holds the favorites panel's roster and vista.

## `TFavoriteRoster`

Covers the favorites panel's Conduct end to end on a real workspace.
A fresh area already stands on its vista, and it lists only the marked entries.
A typed query narrows the rows, no order keeps the ordering, and a hidden language marks the list filtered.
A grasp notice raised on the engine refreshes the rows only under the grasp ordering.
An opened row names the export file, opens in the panel's own editor when editing, and a close empties it.

## `TFavoriteRosterPrepare`

Builds the panel over the atelier, which starts its vista itself.
Its seam answers that the tab is in front, and its envoy declines every question.
Its marshal runs each answer at once.

## `TFavoriteSave`

Stores one entry and marks it as a favorite.
`TFavoriteEntrySave` stores one entry with a single meaning and leaves it unmarked.

## `TFavoriteVista`

Covers the favorites panel's vista restore, its observers, its rows failure and its close on a real workspace.
A second restore carries the held query into the fresh vista.
After a second restore a favorite notice raises the rows once, through the marshal, from the fresh vista only.
A create alone restores once, so the same notice after only a create raises once too.
A workspace notice closes the chosen entry and tells the driver once.
A settings notice raises the rows through the marshal.
A failing favorite find shows `Favorite.LoadFailed` and answers no rows.
The menu offers five orderings.
The window's exit gate closes the editor and stops the recording.
The panel's flag-fill load answers the marked rows and the catalog languages once the fill has run.

## Stamp

Commit: `9377e657b395`
- `TFavoriteRoster.cs` `0693ae85c9441923`
- `TFavoriteVista.cs` `fbcce44113645183`
