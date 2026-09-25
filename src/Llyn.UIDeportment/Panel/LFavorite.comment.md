# LFavorite.cs

## `public sealed class LFavorite`

The favorites panel's deportment: the one vista it holds and what the panel asks of it.
It finds the marked rows, takes the query, order and language filter, and loads or deletes the chosen entry.
The vista is handed in by the window, which restores every tab's vistas together.
The panel's loads and clears go straight to the editor's lectern, so the veneer relays no draft.

## `public LEditor LFavoriteEditor { get; }`

The entry editor's deportment, which takes the favorite vista when the panel's vista is restored.

## `public LPanel LFavoritePanel { get; }`

The shared panel state: the chosen row, the scribe mode, the bin and the leave guard.

## `public bool LFavoriteGraspOrdered => _lFavoriteVista?.LVistaOrderMatch(LCatalogOrder.LCatalogOrderGrasp) ?? false;`

Whether the rows are ordered by grasp, in which case a grasp change re-sorts the list.

## `public long LFavoriteVoyageRead()`

The Entry the panel stands on, as the station the window's trail records.
Zero says the panel stands on none, so there is no place to come back to.
