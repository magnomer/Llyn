# PWing.cs

## `public class PWing : UserControl`

One side of the duplex panel: a search bar, the matches it finds and the Entry picked from them.
The two sides are peers, so each is one control with its own vista and its own display.
The panel never writes, so there is no editor and nothing to discard.

## `public PWing()`

Loads the wing's markup from the Veneer and wears it as its content.
The markup's name scope is copied onto the wing, so its named parts answer `FindName`.
It ties both droppers to their popups, sets both icons, and subscribes the field and list events.
It attaches the index row fill and watches the list's visibility for the tray.

## `private ToggleButton PWingOrderDropper`

Each named part of the markup is read through `FindName`, so call sites keep the old generated names.

## `private void PWingIndexApply(FrameworkElement container, object item, string? change)`

Fills one result row through the shared index fill, then subscribes its click once.

## `private void PWingTrayHandle(object? sender, EventArgs e)`

Copies the list's visibility onto `PWingSeam` and `PWingTray`, the work their bindings did before.

## Inline notes

### `private LWing _lWing = null!;`

The wing's deportment, holding the vista of this side's tab and the reading view's deportment.
The vista carries the order, filter, query and the Entry the side stands on.
The wing keeps no copy of any of the four and asks the deportment for each where it needs it.
The tab is `left` or `right`, and the vista is blank, so nothing typed lists nothing.

### `internal void PWingAttach(PWindow host)`

Puts the side to work on the window deportment, which builds the wing over the engine's ports.
It hands the list, notice and field to the wing, and wires the wing's events to the display.
The vista arrives with each restore, since it belongs to the workspace open then.
The display is its own subscriber, so it stays current on its own.

### `internal async void PWingRestore(string tab, long? id)`

Puts the side back on the workspace open now, on the vista of `tab`, standing on the Entry `id` names.
A switched workspace hands a fresh vista, so the order and filter are the new workspace's own.
The side answers the engine through the vista, one subject per observer.
A vista announcement re-lists the matches, since order, filter or query moved.
A stored entry, a reflex fill or a flipped setting can change a listed row, so each re-lists too.
The flags are loaded before any row is built, then the language menu is built from the loaded packs.
The dropdown lists the shared entry orderings, and the wing draws the filter mark from the vista.
The query is emptied and the wing shows the Entry `id` names, or clears the side for none.
The fresh vista starts with nothing typed and nothing chosen, so neither needs setting.
A different workspace has its own database.
So the Entry the side was comparing came from a workspace no longer open.
A side the state names nothing for stays empty.
The same vista is handed to the display, which reads its chosen entry from it.

### `internal void PWingClose()`

Stops the side: the display releases its playback.

### `private void PWingOrderHandle(object sender, RoutedEventArgs e)`

A chosen ordering goes to the wing with the dropper, which it closes.

### `private void PWingSieveHandle(object sender, RoutedEventArgs e)`

The menu and the mark go to the wing, which reads the ticked languages and redraws the mark.

### `private void PWingQueryHandle(object sender, TextChangedEventArgs e)`

Each keystroke goes to the wing, which shows or hides the list and hands the text to the vista.

### `private void PWingKeyHandle(object sender, KeyEventArgs e)`

The field's keys go to the wing, which steers the open list.

### `private void PWingLeaveHandle(object sender, KeyboardFocusChangedEventArgs e)`

A focus change goes to the wing, which hides the list when focus left both field and list.

### `private void PWingIndexHandle(object sender, RoutedEventArgs e)`

A clicked row goes to the wing, which shows its entry and saves the side's standing.
