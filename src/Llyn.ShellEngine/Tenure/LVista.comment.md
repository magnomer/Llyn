# LVista.cs
Hash: `e25b5a4e56ad7afb`

## `public sealed class LVista`

The engine's view state for one catalog tab: order, language filter, query, chosen row and editing mode.
A panel holds one vista and its controls, and keeps no copy of these values.
Order and filter are announced as they change, and the posture stores them under the tab.
Query and chosen row live only for the session.
Every change to order, filter or query raises a vista bulletin carrying this vista's id.
The panel re-lists its rows from that bulletin, so the vista and the list never disagree.
An unchanged value raises nothing, so a repeated click costs no re-list.
The chosen row raises nothing, since the panel that chose it re-marks its rows in place.
It is also the panel's subscription to the engine's bulletins.
The engine attaches every vista it starts, and the panel attaches one observer per subject to the vista.
So the panel writes no switch over the subject and compares no bulletin id against its own.
A vista bulletin reaches the panel only when it names this vista, so a sibling tab's move is never seen.
A chosen observer is reached only for the row the panel stands on, or for a bulletin naming no row.
The setters run on the UI thread, while a bulletin may arrive on the thread that raised it.
So the observer lists and the chosen row are read under a gate of their own.

## `public event Action<bool>? LVistaEditingSaved;`

The mode the panel asked for, told on every ask whether or not it changed the vista.
The posture stores it as the split every tab opens on next time.

## `private readonly LEngine _lEngine;`

The engine that raises the vista bulletin.

## `private readonly object _lVistaGate = new();`

Guards the observer lists and the chosen row against a bulletin raised on a worker thread.

## `private readonly LBulletinRoster _lVistaRoster = new(null);`

The observers reached for every bulletin of their subject, in the order they were attached.

## `private readonly LBulletinRoster _lVistaChosenRoster;`

The observers reached only when the bulletin names the chosen row or no row at all.
The constructor builds it over `LVistaChosenCheck`, which needs the vista itself.

## `internal LVista(LEngine engine, long id, string tab, LSubject? subject, LCatalogOrder order, LCatalogFilter filter, bool blank, bool editing)`

Made by the engine alone, with the order and filter already read from the tab's layout.

## `public long LVistaId { get; }`

The id every vista bulletin carries, so a panel answers only its own vista.
It names no stored record.

## `public string LVistaTab { get; }`

The layout tab the order and filter are saved under.

## `public bool LVistaBlank { get; }`

Whether an empty query lists nothing rather than every row.
The facade lists nothing for an empty query when this is set.

## `public LCatalogOrder LVistaOrder { get; private set; }`

The ordering the rows are listed in.

## `public LCatalogFilter LVistaFilter { get; private set; }`

The languages kept out of the list.

## `public string LVistaQuery { get; private set; } = string.Empty;`

The search text the rows are matched against, empty for every row.

## `public long? LVistaChosen { get; private set; }`

The id of the row the panel stands on, or null when none is chosen.

## `public static bool LVistaStoredCheck(long? id)`

Whether a row id names a stored record.
A zero or negative id marks a row the engine words itself.
`LVistaStored` asks it, and so does `CGuild` before it keeps edit mode on an opened row.

## `public void LVistaOrderSet(LCatalogOrder? order)`

Takes the ordering and announces the move, which the posture stores under the tab.
No ordering keeps the one the vista has, so a sender that is no order row changes nothing.

## `public void LVistaFilterSet(LCatalogFilter filter)`

Takes the filter and announces the move, which the posture stores under the tab.
Two filters are equal only when they hold the same list object, so an equal list built twice still announces.

## `public void LVistaFilterSet(IReadOnlyList<string> hidden)`

Takes the languages a driver ticked off and builds the filter from them.
The filter's own rule makes a list hiding nothing the shared empty filter.

## `public void LVistaQuerySet(string query)`

Takes the search text and announces the move.

## `public void LVistaSelect(long? id)`

Makes the row of `id` the chosen one, or none for null.
No bulletin follows, because a chosen row changes no list and the chooser marks it in place.
The write is gated, since a bulletin on another thread reads the chosen row to pick its observers.

## `public void LVistaToggle(long id)`

Selects the row, or unselects it when it is the chosen one already.

## `public void LVistaObserverAttach(LSubject subject, Action<LBulletin> observer)`

Subscribes `observer` to every bulletin of `subject` that reaches this vista.
An observer is a delegate over a bulletin, so a surface hands a method and implements no contract.
A vista bulletin reaches it only when it carries this vista's id.
Observers are reached in the order attached, so an earlier one may move the chosen row for a later one.

## `public void LVistaChosenAttach(LSubject subject, Action<LBulletin> observer)`

Subscribes `observer` to the bulletins of `subject` that name the chosen row or no row at all.
The display attaches its per-record redraws here, so another entry's favorite mark never redraws its heart.
A bulletin with an id of zero or less names every record, so it is delivered whatever row is chosen.
The chosen observers are reached after every plain observer, so a plain one that selects the stored row is honoured.

## `internal void LVistaBulletinHandle(LBulletin bulletin)`

The engine's announcement, forwarded to the observers whose subject it names.
Internal, because the engine attaches this method group at the start and detaches it when the tab is replaced.
A vista bulletin for another vista is dropped at the door.
Both rosters are copied in one gate hold and dispatched outside it, as the engine does.
Plain observers are reached first, then chosen ones.

## `private bool LVistaChosenCheck(LBulletin bulletin)`

The chosen roster's filter, asked afresh before each chosen observer.
So a selection made a moment ago by an earlier observer counts.

## `public LSubject? LVistaSubject { get; }`

The stored subject represented by this tab, named by the panel that started it.
Structural sound tables pass no subject and cannot load or delete records.

## `public long? LVistaStored`

The chosen id when it names a stored record, otherwise null.

## `public bool LVistaMatch(long id)`

Whether `id` is the chosen row.

## `public bool LVistaFiltered`

Whether the filter hides any language.

## `public bool LVistaQueried`

Whether the query holds anything besides whitespace.

## `public bool LVistaLeft`

Whether the tab is the left duplex side.

## `public bool LVistaInput`

Whether the tab is the input tab.

## `public bool LVistaNarrowed`

Whether the filter or the query narrows the rows, so a panel asks one question instead of combining two.

## `public static LCatalogOrder LVistaOrderRead(LVista? vista)`

The ordering a panel shows, headword order before a vista arrives.

## `public static LCatalogFilter LVistaFilterRead(LVista? vista)`

The filter a panel shows, the shared empty filter before a vista arrives.

## `public bool LVistaEditing { get; private set; }`

Each live vista owns its current mode while newly started vistas inherit the workspace preference.

## `public void LVistaEditingSet(bool editing)`

Tells the editing event the mode asked for, then notifies observers of a changed local mode.
An unchanged local mode still tells the event, since another vista may have moved the shared split.
The posture listens to the event and stores the split, so the engine holds no preference of its own.
