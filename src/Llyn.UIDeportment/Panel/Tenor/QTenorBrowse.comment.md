# QTenorBrowse.cs

## `internal sealed partial class QTenor`

Browsing behavior of the tenor panel.
The search field and the sorting dropdown refill the register catalog.
A chosen Register refills the Entries beside it, and a chosen Entry is loaded back from the workspace.
It is rendered read-only in the reader, which the mode toggle swaps for the editor.
This is the same read half of the entry round trip the library panel offers, reached through a Register.
The entry list itself lives in [QTenorCohort.cs](QTenorCohort.comment.md).

## Inline notes

### `private CTenor _cTenor = null!;`

The panel's Conduct, holding the register vista and the cohort vista it restored.
The vista carries the order, the query, the chosen Register, and the languages hidden from its entries.
The panel keeps no copy of any of the four and asks Conduct for each where it needs it.
The Register is held by its id rather than its name, since a name may be rewritten under the panel.
A null Register is not an absence to be corrected.
It is the whole workspace, which is what the panel shows first.
The vista is null until the window hands one over, so the handlers do nothing before that.
A switched workspace hands over a fresh vista, read from that workspace's own layout.

### `private async void QTenorWorkspaceRefine()`

Answers the area's workspace event, which has already emptied the panel and raised the rows.
It loads the flags of the workspace that moved.

### `private void QSoundingObserve(object sender, TextChangedEventArgs e)`

Each keystroke hands the search text to the vista, whose announcement rebuilds the catalog.

### `private void QQuestObserve(object sender, TextChangedEventArgs e)`

Each keystroke hands the entry search text to the cohort's query gate.

### `private void QDegreeObserve(object sender, RoutedEventArgs e)`

A chosen ordering goes raw to the order gate, then the dropdown is closed.
The vista saves it and announces it, and the announcement rebuilds the catalog.

### `private void QDegreeDropperRefine()`

Closes the ordering dropdown once a choice is made.

### `private void QGrilleObserve(object sender, RoutedEventArgs e)`

The ticked languages are read off the menu and handed to the vista, which saves and announces them.
The mark on the button is redrawn from the vista at once.

### `internal async void QTenorVistaRefine()`

Answers the workspace opening, after `CTenor` has restored its vistas and attached its observers.
The ordering menu, the dropdown mark and the filter mark are drawn from the vista first.
The flags are loaded before any row is built.
The language menu is then built from the languages that load answers.
The catalog is then listed once, so a row keeps the flag it was built with.
Its read raises the entry rows, so the entry list is painted after the flags too.

### `private void QGrilleListRefine(IReadOnlyList<string> languages)`

Builds the language menu from the loaded packs, each box ticked unless the vista hides its language.

### `private void QGrilleRefine()`

Shows the filter mark while the vista hides any language.

### `private void QTenorFreshObserve(object sender, RoutedEventArgs e)`

New makes whatever the emptier panel would list, and the one fresh gate decides which.
The gate asks the leave question and any wording through the envoy, so the view asks nothing.

### `private void QTenorViewerObserve(object sender, RoutedEventArgs e)`

The viewer button hands false to the panel's scribe toggle, with no test of who sent it.

### `private void QTenorScribeObserve(object sender, RoutedEventArgs e)`

The scribe button hands true to the panel's scribe toggle, and each button subscribes its own.

### `private void QTenorStoreObserve(object sender, RoutedEventArgs e)`

The store button calls the editor's save gate.

### `private void QTenorBinObserve(object sender, RoutedEventArgs e)`

The bin button calls the panel's delete gate.

### `private void QGamutRefine()`

Answers the area's rows event and lists the Registers the area reads, already counted and in the vista's ordering.
The rows the language packs name and the rows the user wrote arrive as one shelf.
The chosen Register is re-marked as the catalog is rebuilt, so the selection survives a re-sort.
A failed read has already been shown by the area, which then answers no rows.
A read that succeeds makes the area raise the entry rows, so the entry list follows.

### `private void QGamutRegisterRefine()`

Answers the area's opening event after a chip's arrival or a coinage.
The area has already emptied both queries, so the fields only show it.
The fields' own handlers still hear the change, and their gates find the queries already empty.
The area raises the rows right after, so the catalog is rebuilt without the view asking.

### `private void QGamutObserve(object sender, RoutedEventArgs e)`

A clicked row hands its Register id to the toggle gate, and a click that carries no item is ignored.
The gate records the station, toggles and raises the rows, so clicking the chosen Register lets go of it.
That is how the panel is put back on the whole workspace without a separate control saying so.

## `private void QGamutItemRefine(FrameworkElement container, object item, string? _)`

Fills one register row from its item, the work its bindings did before.
The row carries the `Chosen` cue on the chosen item and none otherwise, which the look sheet paints.
The click is subscribed once per row, removed first so a refill never doubles it.
It runs again on every change the item raises, so a chosen row moves without a refill.
The chosen Register stays visible while the eye is on the Entries beside it.
Without it the entry list would show a filtered set with nothing on screen saying which filter.
