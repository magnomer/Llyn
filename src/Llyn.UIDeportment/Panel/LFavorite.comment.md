# LFavorite.cs

## `public sealed class LFavorite`

The favorites panel's deportment: the one vista it holds and what the panel asks of it.
It finds the marked rows, takes the query, order and language filter, and loads or deletes the chosen entry.
The vista is handed in by the window, which restores every tab's vistas together.
The panel's loads and clears go straight to the lectern its view hands in, so the veneer relays no draft.

## `public LEditor LFavoriteEditor { get; }`

The entry editor's deportment, which takes the favorite vista when the panel's vista is restored.

## `public CPanel LFavoritePanel { get; }`

The shared panel state: the chosen row, the scribe mode, the bin and the leave guard.

## `private bool LFavoriteGraspOrdered => _lFavoriteVista?.LVistaOrderMatch(LCatalogOrder.LCatalogOrderGrasp) ?? false;`

Whether the rows are ordered by grasp, in which case a grasp change re-sorts the list.

## `public void LFavoriteGraspApply(Action find)`

Runs the view's roster read only while the rows are ordered by grasp.
The view hands its read in, so the ordering never becomes a view's branch.
It runs on the view's thread, since the view calls it from its own observer.

## `public long LFavoriteVoyageRead()`

The Entry the panel stands on, as the station the window's trail records.
Zero says the panel stands on none, so there is no place to come back to.

## `internal LFavorite(`

Only the panel factory builds it over the engine's ports, so no view names a port.

## `internal void LFavoriteVistaRestore(LVista vista)`

Hands the vista to the panel state and the editor.
It stays internal, so only the atelier's vista restore passes an engine vista.

## `public IReadOnlyList<CVistaRow> LFavoriteRowsRead()`

The marked rows the engine returns for the vista, already filtered, sorted, numbered and marked.
They cross as shapes, so the view names no engine row.

## `public Task LFavoritePortraitPrint(CPortraitLabel label, CPressTicket ticket)`

Prints the chosen entry as the engine portrays it, with the labels the window localized.
The label and the ticket are mapped to the engine's own by the panel's shared maps.
