# QTaxonomyBrowse.cs
Hash: `bf286c630610ad78`

## `internal sealed partial class QTaxonomy`

Browsing behavior of the taxonomy panel.
The search field and the sorting dropdown refill the tag catalog.
A chosen tag refills the entries beside it, and a chosen entry is loaded back from the workspace.
It is rendered read-only in the reader, which the mode toggle swaps for the editor.
This is the same read half of the entry round trip the library panel offers.
It is reached through a tag instead of a headword.
The entry list itself lives in [QTaxonomyMembership.cs](QTaxonomyMembership.comment.md).

## Inline notes

### `private async void QTaxonomyWorkspaceRefine()`

Answers the area's workspace event, which has already emptied the panel and raised the rows.
It loads the flags of the workspace that moved.
The window's envoy goes with the load, so the catalog reports a failed load and answers no languages.

### `private void QExplorationObserve(object sender, TextChangedEventArgs e)`

Each keystroke hands the search text to the vista, whose announcement rebuilds the catalog.

### `private void QFunnelObserve(object sender, RoutedEventArgs e)`

A chosen ordering goes raw to the order gate, then the dropdown is closed.
The vista saves it and announces it, and the announcement rebuilds the catalog.

### `private void QFunnelDropperRefine()`

Closes the ordering dropdown once a choice is made.

### `private void QLatticeObserve(object sender, RoutedEventArgs e)`

The ticked languages are read off the menu and handed to the vista, which saves and announces them.
The mark on the button is redrawn from the vista at once.

### `internal async void QTaxonomyVistaRefine()`

Answers the workspace opening, after `CTaxonomy` has restored its vistas and attached its observers.
The ordering menu, the dropdown mark and the filter mark are drawn from the vista first.
The flags are loaded before any row is built.
The language menu is then built from the languages that load answers.
The catalog is then listed once, so a row keeps the flag it was built with.
Its read raises the entry rows, so the entry list is painted after the flags too.
Its one request is `CTaxonomyRowsLoad`, which runs the flag fill and then answers the rows it paints.

### `private void QLatticeListRefine(IReadOnlyList<string> languages)`

Builds the language menu from the loaded packs, each box ticked unless the vista hides its language.

### `private void QLatticeRefine()`

Shows the filter mark while the vista hides any language.

### `private void QTaxonomyFreshObserve(object sender, RoutedEventArgs e)`

New makes whatever the emptier panel would list, and the one fresh gate decides which.
The gate asks the leave question and any wording through the envoy, so the view asks nothing.

### `private void QTaxonomyViewerObserve(object sender, RoutedEventArgs e)`

The viewer button hands false to the panel's scribe toggle, with no test of who sent it.

### `private void QTaxonomyScribeObserve(object sender, RoutedEventArgs e)`

The scribe button hands true to the panel's scribe toggle, and each button subscribes its own.

### `private void QDirectoryRefine()`

Answers the area's rows event and lists the Tags the area reads, already in the vista's ordering.
The chosen Tag is re-marked as the catalog is rebuilt, so the selection survives a re-sort.
A failed read has already been shown by the area, which then answers no rows.
A read that succeeds makes the area raise the entry rows, so the entry list follows.

### `private void QDirectoryRefine(IReadOnlyList<CCatalogTag> rows)`

Paints `rows` the area answered ready, so the paint itself asks Conduct nothing.
The parameterless form reads them, and a flag-fill Refine hands in what its load answered.

### `private void QDirectoryTagRefine()`

Answers the area's opening event after a chip's arrival or a coinage.
The area has already emptied both queries, so the fields only show it.
The fields' own handlers still hear the change, and their gates find the queries already empty.
The area raises the rows right after, so the catalog is rebuilt without the view asking.

### `private void QDirectoryObserve(object sender, RoutedEventArgs e)`

A clicked row hands its Tag's id to the toggle gate, and a click that carries no item is ignored.

### `CTaxonomyTagToggle(item.QDirectoryItemId);`

The gate records the station and toggles, so clicking the chosen tag lets go of it.
That is how the panel is put back on the whole workspace without a separate control saying so.
The gate raises the rows, so the view rebuilds nothing itself.

## `private void QDirectoryItemRefine(FrameworkElement container, object item, string? _)`

Fills one tag row from its item, the work its bindings did before.
The row carries the `Chosen` cue on the chosen item and none otherwise, which the look sheet paints.
The click is subscribed once per row, removed first so a refill never doubles it.
It runs again on every change the item raises, so a chosen row moves without a refill.
The chosen tag stays visible while the eye is on the entries beside it.
Without it the membership list would show a filtered set with nothing on screen saying which filter.
