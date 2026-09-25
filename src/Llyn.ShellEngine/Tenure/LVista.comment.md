# LVista.cs

## `public sealed class LVista`

The engine's view state for one catalog tab: order, language filter, query, chosen row and editing mode.
A panel holds one vista and its controls, and keeps no copy of these values.
Order and filter are saved under the tab as they change, so a reopen finds them.
Query and chosen row live only for the session, as they do today.
Every change to order, filter or query raises a vista bulletin carrying this vista's id.
The panel re-lists its rows from that bulletin, so the vista and the list never disagree.
An unchanged value raises nothing, so a repeated click costs no re-list.
The chosen row raises nothing, since the panel that chose it re-marks its rows in place.
It is also the panel's subscription to the engine's bulletins.
The engine attaches every vista it starts, and the panel attaches one observer per subject to the vista.
So the panel writes no switch over the subject and compares no bulletin id against its own.
Before this every panel took every bulletin and sorted them itself, a hundred comparisons over the shell.
A vista bulletin reaches the panel only when it names this vista, so a sibling tab's move is never seen.
A chosen observer is reached only for the row the panel stands on, or for a bulletin naming no row.
The setters run on the UI thread, while a bulletin may arrive on the thread that raised it.
So the observer lists and the chosen row are read under a gate of their own.

## `public event Action<bool>? LVistaEditingSaved;`

The mode the panel asked for, told on every ask whether or not it changed the vista.
The posture stores it as the split every tab opens on next time.

## `private readonly LEngine _lEngine;`

The engine that saves the layout and raises the bulletin.

## `private readonly object _lVistaGate = new();`

Guards the observer lists and the chosen row against a bulletin raised on a worker thread.

## `private readonly List<(LSubject, Action<LBulletin>)> _lVistaObservers = [];`

The observers reached for every bulletin of their subject, in the order they were attached.

## `private readonly List<(LSubject, Action<LBulletin>)> _lVistaChosenObservers = [];`

The observers reached only when the bulletin names the chosen row or no row at all.

## `internal LVista(LEngine engine, long id, string tab, LCatalogOrder order, LCatalogFilter filter, bool blank, bool editing)`

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

Takes the ordering and announces the move, which the posture stores under the tab.

## `public void LVistaFilterSet(LCatalogFilter filter)`

Takes the filter and announces the move, which the posture stores under the tab.
Two filters compare by the list they hold, so the same list built twice still announces.

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

## `public void LVistaObserverDetach(Action<LBulletin> observer)`

Stops reaching `observer` from either list.
Delegate equality is target plus method, so the delegate that was attached is the one found.
A panel detaches its observers before it takes a replacement vista, so the old one falls silent.

## `internal void LVistaBulletinHandle(LBulletin bulletin)`

The engine's announcement, forwarded to the observers whose subject it names.
Internal, because the engine attaches this method group at the start and detaches it when the tab is replaced.
A vista bulletin for another vista is dropped at the door.
Both lists are copied under the gate and the calls are made outside it, as the engine does.
The chosen row is read afresh before each chosen observer, so a selection made a moment ago counts.

## `public LSubject? LVistaSubject { get; }`

The stored subject represented by this tab, named by the panel that started it.
Structural sound tables pass no subject and cannot load or delete records.

## `public LDraft? LVistaLoad()`

Loads the selected subject into a read-only snapshot without opening an editing session.
The snapshot carries its content in the matching draft member and has no draft identity.
A stored choice that no longer loads is dropped here, so a panel never branches on the missing answer.

## `public LDraft? LVistaLoad(long? id)`

Selects the row and loads it, restoring the previous choice when the load throws.
So a failed click leaves the panel where it stood, and no caller keeps the prior choice.

## `public bool LVistaNarrowed`

Whether the filter or the query narrows the rows, so a panel asks one question instead of combining two.

## `public static string LVistaFileRead(LVista? vista)`

The file name an export of the vista's entry is offered under: the headword with barred characters replaced.
The trail port of the vista's engine says which characters are barred, so the vista reads no file rule itself.
A vista that holds nothing, or fails to load, is offered as `entry`.
A cleared selection or missing record returns null.

## `public LRevision? LVistaDelete()`

Deletes the selected record of the vista's subject and clears the selection afterwards.
Only an entry delete records a revision, so every other subject answers null after deleting.
A catalog record is detached from every owner first, as the panel already confirmed the usage.
Tag, register and structural vistas delete nothing and answer null.

## `public bool LVistaEditing { get; private set; }`

Each live vista owns its current mode while newly started vistas inherit the workspace preference.

## `public void LVistaEditingSet(bool editing)`

Tells the editing event the mode asked for, then notifies observers of a changed local mode.
An unchanged local mode still tells the event, since another vista may have moved the shared split.
The posture listens to the event and stores the split, so the engine holds no preference of its own.
