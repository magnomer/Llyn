# LVista.cs

## `public sealed class LVista`

The engine's view state for one catalog tab: order, language filter, query and chosen row.
A panel holds one vista and its controls, and keeps no copy of any of the four.
Order and filter are saved under the tab as they change, so a reopen finds them.
Query and chosen row live only for the session, as they do today.
Every change to order, filter or query raises a vista bulletin carrying this vista's id.
The panel re-lists its rows from that bulletin, so the vista and the list never disagree.
An unchanged value raises nothing, so a repeated click costs no re-list.
The chosen row raises nothing, since the panel that chose it re-marks its rows in place.
It is driven on the UI thread alone and so takes no lock of its own.

## `private readonly LEngine _lEngine;`

The engine that saves the layout and raises the bulletin.

## `internal LVista(LEngine engine, long id, string tab, LCatalogOrder order, LCatalogFilter filter, bool blank)`

Made by the engine alone, with the order and filter already read from the tab's layout.

## `public long LVistaId { get; }`

The id every vista bulletin carries, so a panel answers only its own vista.
It counts up per engine and names no stored record.

## `public string LVistaTab { get; }`

The layout tab the order and filter are saved under.

## `public bool LVistaBlank { get; }`

Whether an empty query lists nothing rather than every row.
The duplex wings search on demand, so a wing with nothing typed offers no rows.
Every other tab lists its whole catalog on an empty query.

## `public LCatalogOrder LVistaOrder { get; private set; }`

The ordering the rows are listed in.

## `public LCatalogFilter LVistaFilter { get; private set; }`

The languages kept out of the list.

## `public string LVistaQuery { get; private set; } = string.Empty;`

The search text the rows are matched against, empty for every row.

## `public long? LVistaChosen { get; private set; }`

The id of the row the panel stands on, or null when none is chosen.

## `public void LVistaOrderSet(LCatalogOrder order)`

Takes the ordering, saves it under the tab and announces the move.

## `public void LVistaFilterSet(LCatalogFilter filter)`

Takes the filter, saves it under the tab and announces the move.
Two filters compare by the list they hold, so the same list built twice still announces.

## `public void LVistaQuerySet(string query)`

Takes the search text and announces the move.

## `public void LVistaSelect(long? id)`

Makes the row of `id` the chosen one, or none for null.
No bulletin follows, because a chosen row changes no list and the chooser marks it in place.
