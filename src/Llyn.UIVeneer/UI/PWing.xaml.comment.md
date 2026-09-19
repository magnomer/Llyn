# PWing.xaml.cs

## `public partial class PWing : UserControl`

One side of the duplex panel: a search bar, the matches it finds and the Entry picked from them.
The two sides are peers, so each is one control with its own vista and its own display.
The panel never writes, so there is no editor and nothing to discard.

## Inline notes

### `private readonly ObservableCollection<PIndexItem> _pWingIndex = [];`

The matches this side's query found, held per side because the sides are peers.
Changing one side leaves the other where it was.

### `private LVista? _pWingVista;`

The engine's view state for this side's tab: order, filter, query and the Entry the side stands on.
The wing keeps no copy of any of the four and reads each from the vista where it needs it.
The tab is `left` or `right`, and the vista is blank, so nothing typed lists nothing.

### `internal void PWingAttach(PWindow host, LEngine engine)`

Puts the side to work on `engine`.
It binds the match list, subscribes to the engine and attaches the display.
The vista arrives with each restore, since it belongs to the workspace open then.
The display is its own subscriber, so it stays current on its own.

### `internal async void PWingRestore(LVista vista, long? id)`

Puts the side back on the workspace open now, with its vista, standing on the Entry `id` names.
A switched workspace hands a fresh vista, so the order and filter are the new workspace's own.
The side answers the engine through the vista, one subject per observer.
A vista announcement re-lists the matches, since order, filter or query moved.
A stored entry, a reflex fill or a flipped setting can change a listed row, so each re-lists too.
The flags are loaded before any row is built, then the language menu is built from the loaded packs.
The dropdown mark and the filter mark are drawn from the vista.
The query is emptied and the side is cleared first.
The fresh vista starts with nothing typed and nothing chosen, so neither needs setting.
A different workspace has its own database.
So the Entry the side was comparing came from a workspace no longer open.
A side the state names nothing for stays empty.
The same vista is handed to the display, which reads its chosen entry from it.

### `internal void PWingClose()`

Stops the side: the display releases its playback.

### `private void PWingOrderHandle(object sender, RoutedEventArgs e)`

A chosen ordering closes the dropdown and hands the ordering to the vista.
The vista saves it under the side's tab and announces it, and the announcement re-lists the matches.

### `private void PWingSieveHandle(object sender, RoutedEventArgs e)`

The ticked languages are read off the menu and handed to the vista, which saves and announces them.
The mark on the button is redrawn from the vista at once.

### `private void PWingQueryHandle(object sender, TextChangedEventArgs e)`

Each keystroke shows the list while something is typed and hides it when nothing is.
The text is then handed to the vista, whose announcement re-lists the matches.

### `private void PWingKeyHandle(object sender, KeyEventArgs e)`

The keyboard path from the field into the open list.
Escape hides the list and leaves the query standing.
Enter picks the chosen row, as a click on it would, but only while that row is in the list.
A choice made under an earlier query may name an Entry the rows no longer hold, and Enter then idles.
Down and Up move the choice one row and stop at either end.
The choice is the vista's, and the rows are re-marked in place and the chosen one scrolled into view.
A closed list takes none of these, so the keys keep their plain meaning in the field.

### `private void PWingLeaveHandle(object sender, KeyboardFocusChangedEventArgs e)`

Focus leaving the field and the list hides the list.
Focus moving between the two is not a leave, since a clicked row takes focus before its click lands.

### `private void PWingIndexHandle(object sender, RoutedEventArgs e)`

A clicked row hides the list, shows its Entry and saves the side's standing.

### `private void PWingOrderRestore()`

Moves the dropdown mark onto the ordering the vista holds.

### `private void PWingSieveRestore()`

Shows the mark on the sieve button while the vista hides any language.

### `private void PWingIndexFind()`

Lists the matches from the vista, already filtered, sorted, numbered and marked by the engine.
`PSplice` moves the marks in place when only the choice changed, so the list keeps its scroll position.
A blank vista with nothing typed answers no rows, so nothing typed lists nothing.
The empty text shows only while a typed query matched nothing.

### `private void PWingEntryShow(long id)`

Loads one Entry back from the workspace onto this side.
An Entry that is gone leaves the side empty rather than showing what it was.
A load that failed leaves the side where it stood and reports the failure.
The vista's choice follows what the side now stands on.

### `private void PWingEntrySave()`

Saves the Entry this side stands on under the workspace state slot its tab names.
A side that stands on nothing saves nothing rather than an id pointing at what is gone.
