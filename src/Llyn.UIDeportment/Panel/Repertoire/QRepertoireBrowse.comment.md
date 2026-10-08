# QRepertoireBrowse.cs
Hash: `30b5e8589bf5509c`

## `internal sealed partial class QRepertoire`

Browsing behavior of the Repertoire panel.
`QAtlas` lists every Situation the workspace holds, including one nothing references.
A chosen row is read back and shown, and the middle column narrows to the entries referencing it.
`QRepertoireScribe` swaps the reading for the editor.
The editor's own fields live in `QRepertoireEditor.cs`.
The entry column lives in `QRepertoireEntry.cs`.
The reading side that draws the chosen Situation lives in `QRepertoireVignette.cs`.
The panel answers two questions rather than one.
It shows what this Situation is, and where it is used.

## `private async void QRepertoireWorkspaceRefine()`

Answers the Conduct's workspace change by reloading the flags, since they do not belong to the old folder's rows.
The Conduct has already closed the shown Situation and dropped the old selection.
The window's envoy goes with the load, so the catalog reports a failed load and answers no languages.

## `private void QInquestObserve(object sender, TextChangedEventArgs e)`

Each keystroke hands the search text to the vista, whose announcement refills the catalog.

## `private void QTierObserve(object sender, RoutedEventArgs e)`

A chosen ordering is handed to the vista.
The vista saves it and announces it, and the announcement refills the catalog.
The dropdown then closes through `QTierRefine`.

## `private void QTierRefine()`

Unticks the ordering button so its dropdown closes.

## `internal async void QRepertoireVistaRefine()`

Answers the workspace opening, after the Conduct has started the fresh vistas and attached their observers.
The ordering menu is ticked from the atlas and the filter mark is drawn.
The flags are loaded before any row is built.
The language menu is then built from the languages that load answers.
The catalog is then listed once.
The Conduct carried any search text held into the fresh vistas, so the boxes need no re-send.
Its one request is `CRepertoireRowsLoad`, which runs the flag fill and then answers the rows it paints.

## `internal async void QOccurrenceVistaRefine()`

Answers the workspace opening for the entry column, which shows the same flags.
It waits for the same load, then lists the column once.
Its one request is `COccurrenceRowsLoad`, which runs the flag fill and then answers the rows it paints.

## `private void QMeshRefine()`

Shows the filter mark while the vista hides any language.

## `private void QMeshListRefine(IReadOnlyList<string> languages)`

Builds the language menu from the given languages, ticked as the vista's filter reads.
The workspace opening and the dropped inquest share it.

## `private void QInquestRefine()`

Empties the inquest box and rebuilds the language menu after an arrival dropped both on the vista.
The menu is read back from the vista's cleared filter, and the filter mark follows.

## `private void QSortieObserve(object sender, TextChangedEventArgs e)`

Each keystroke hands the entry search text to the occurrence vista, whose announcement refills the entry column.

## `private void QMeshObserve(object sender, RoutedEventArgs e)`

The ticked languages are read off the menu of the clicked box and handed to the vista.
The vista saves and announces them.
The mark on the button is redrawn from the vista at once.

## `private void QAtlasRefine()`

Refills the catalog with the ready rows the Conduct reads, already matched and already ordered.
The Conduct words the unknown and untitled texts, and reports its own read failure, answering no rows.
The rows arrive as `CCatalogSituation` shapes, the kind and the count carrying their own text.
The Conduct read drops a shown selection whose row no longer stands, and keeps one an open editor holds.
Its clear announces fresh rows that list the same Situations, so the nested refill is harmless.
The tally chips and the entry column follow through the atlas's row notice, so neither is painted here.

## `private void QAtlasRefine(IReadOnlyList<CCatalogSituation> rows)`

Paints `rows` the area answered ready, so the paint itself asks Conduct nothing.
The parameterless form reads them, and a flag-fill Refine hands in what its load answered.

## `private void QRepertoireTallyRefine()`

Writes the chosen Situation's tally on both sides of the panel.
So the reader and the writer see the same figure.
The Conduct words the sentence through the engine, so a reference added elsewhere shows at the next row notice.

## `private void QAtlasObserve(object sender, RoutedEventArgs e)`

A clicked row asks the Conduct to select its Situation.
The Conduct asks about unsaved work first, and records the station only when the move goes ahead.

## `private void QAtlasItemRefine(FrameworkElement container, object item, string? _)`

Fills one atlas row from its item, the work its bindings did before.
The row carries the `Chosen` cue on the chosen item and none otherwise, which the look sheet paints.
The click is subscribed once per row, removed first so a refill never doubles it.
It runs again on every change the item raises, so a chosen row moves without a refill.
The kind is written as it is, and the look sheet collapses it while empty.

## `private void QRepertoireBinObserve(object sender, RoutedEventArgs e)`

Hands the delete to the Conduct, which acts only while a Situation and not an Entry is shown.
The envoy asks the user first, and a Situation something references is named in the question.

## `private void QRepertoireViewerObserve(object sender, RoutedEventArgs e)`

Swaps the editor for the reading, on whichever side the Conduct stands.
Each segment has its own observer, so no control is read to decide which was pressed.

## `private void QRepertoireScribeObserve(object sender, RoutedEventArgs e)`

Swaps the reading for the editor, on whichever side the Conduct stands.
The Conduct asks before leaving an editor, so unsaved wording is never lost silently.

## `internal void QRepertoireVoyageRefine(bool past, bool future)`

Lights the two trail buttons from the voyage state the navigation raises.
The navigation owns the trail, so the panel only shows what it is told.

## `private void QRepertoireRetreatObserve(object sender, RoutedEventArgs e)`

Steps the navigation's trail back one station.

## `private void QRepertoireAdvanceObserve(object sender, RoutedEventArgs e)`

Steps the navigation's trail forward one station.
