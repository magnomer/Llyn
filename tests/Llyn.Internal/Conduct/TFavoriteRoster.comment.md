# TFavoriteRoster.cs

## `public sealed class TFavoriteRoster`

Covers the favorites panel's Conduct end to end on a real workspace.
Before a vista arrives the list reads nothing, and afterwards it lists only the marked entries.
A typed query narrows the rows, no order keeps the ordering, and a hidden language marks the list filtered.
A grasp notice refreshes the rows only under the grasp ordering.
The filter offers the catalog's languages.
An opened row names the export file, opens in the panel's own editor when editing, and a close empties it.

## `private static CFavorite TFavoriteRosterPrepare(CAtelier atelier)`

Builds the panel over the atelier and starts its vista.
Its seam answers that the tab is in front, and its envoy declines every question.

## `private static LEntry TFavoriteSave(LEngine engine, string headword, string language)`

Stores one entry and marks it as a favorite.

## `private static LEntry TFavoriteEntrySave(LEngine engine, string headword, string language)`

Stores one entry with a single meaning and leaves it unmarked.
