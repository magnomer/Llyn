# LFavorite.cs

## `public sealed class LFavorite`

The favorites panel's deportment: the one vista it holds and what the panel asks of it.
It finds the marked rows, takes the query, order and language filter, and loads or deletes the chosen entry.
The vista is handed in by the window, which restores every tab's vistas together.
The panel's mode, its bin and its scribe toggle stay in the veneer until the tabs share one panel deportment.

## `public LEditor LFavoriteEditor { get; }`

The entry editor's deportment, which takes the favorite vista when the panel's vista is restored.

## `public bool LFavoriteGraspOrdered => _lFavoriteVista?.LVistaOrderMatch(LCatalogOrder.LCatalogOrderGrasp) ?? false;`

Whether the rows are ordered by grasp, in which case a grasp change re-sorts the list.

## `public LVista? LFavoriteVista => _lFavoriteVista;`

The vista, exposed for the window's export dialog alone, which names its file after the vista.
