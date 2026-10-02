# QCorpusBrowse.cs
Hash: `8f83881952bc4cb6`

## `internal sealed partial class QCorpus`

Browsing behavior of the Corpus panel.
`QAnthology` lists every Example the workspace holds, including one nothing quotes.
A chosen row is read back and shown, and the middle column narrows to the entries quoting it.
`QCorpusScribe` swaps the reading for the editor.
The panel answers two questions rather than one.
They are what this sentence is, and where it is quoted.

## `private async void QCorpusWorkspaceRefine()`

Answers `CCorpusWorkspaceChanged`, raised after the corpus closed its Example on a workspace notice.
The flags and the language menu belong to the old folder, so both are loaded again.
One ensign read loads the flags and answers the languages the speaker menu lists.

## `private void QQueryObserve(object sender, TextChangedEventArgs e)`

Each keystroke hands the search text to the vista, whose announcement refills the catalog.

## `private void QRankObserve(object sender, RoutedEventArgs e)`

A chosen ordering goes to the gate `CAnthologyOrderSet`, which saves and announces it.
The dropdown then closes.

## `private void QRankDropperRefine()`

Closes the ordering dropdown.

## `internal async void QCorpusVistaRefine()`

Answers `CWorkspaceOpened`, after the corpus restored its vistas, carried the queries and attached its observers.
The dropdown mark and the filter mark are drawn from the anthology first.
The flags are loaded before any row is built, since a row keeps the flag it was built with.
The same read answers the languages for the filter menu and the speaker menu.
The catalog is then read and painted, which is the panel's first paint in the workspace.
Its one request is `CCorpusRowsLoad`, which runs the flag fill and then answers the rows it paints.

## `internal async void QQuotationVistaRefine()`

Answers `CWorkspaceOpened` with the middle column's first paint in the workspace.
A row keeps the flag it was built with, so the flags are loaded before the rows are read.
The catalog's restore paints the catalog, so each list reads only its own rows.
Its one request is `CQuotationRowsLoad`, which runs the flag fill and then answers the rows it paints.

## `private void QGauzeRefine()`

Shows the filter mark while the vista hides any language.

## `private void QGauzeBuild(IReadOnlyList<string> languages)`

Builds the language menu from the given languages, ticked as the vista's filter reads.
The restore and the dropped search share it.

## `private void QQueryClear()`

Empties the search box and rebuilds the language menu after an arrival dropped both on the vista.

## `private void QDredgeObserve(object sender, TextChangedEventArgs e)`

Each keystroke hands the dredge text to the quotation vista, whose announcement refills the entry column.

## `private void QGauzeObserve(object sender, RoutedEventArgs e)`

The ticked languages are read off the clicked box's menu and handed to the gate `CAnthologyFilterSet`.
The mark on the button is then redrawn from the vista.

## `private void QAnthologyRefine()`

Refills the catalog with the ready rows `CCorpusRowsRead` answers, already matched, ordered and worded.
The driver hands over nothing and decides nothing about the ordering, the query or the wording.
The read reports its own failure, and it drops a shown selection whose row no longer stands.

## `private void QAnthologyRefine(IReadOnlyList<CCatalogExample> rows)`

Paints `rows` the area answered ready, so the paint itself asks Conduct nothing.
The parameterless form reads them, and a flag-fill Refine hands in what its load answered.

## `private void QCorpusTallyRefine()`

Rewrites both tally chips from `CPanelTallyRead` when the catalog's rows change.
A quotation added elsewhere refills the catalog, so its count shows at once.
Both chips show the chosen Example's count, since only one of them is in view.

## `private void QAnthologyObserve(object sender, RoutedEventArgs e)`

A clicked row asks the gate `CCorpusExampleSelect` to select it.
The gate asks about unsaved work first, and records the station only when the move goes ahead.

## `private void QAnthologyApply(FrameworkElement container, object item, string? _)`

Fills one catalog row from its item.
The row carries the `Chosen` cue on the chosen item and none otherwise, which the look sheet paints.
The click is subscribed once per row, removed first so a refill never doubles it.

## `private void QCorpusBinObserve(object sender, RoutedEventArgs e)`

Hands the delete to the gate `CCorpusExampleDelete`, which acts only while an Example and not an Entry is shown.

## `private void QCorpusStoreObserve(object sender, RoutedEventArgs e)`

The rail's save, standing for whichever editor is in front.

## `private void QCorpusViewerObserve(object sender, RoutedEventArgs e)`

Puts whichever side the corpus stands on back on its reading page, through `CCorpusScribeToggle`.
The viewer and scribe segments have their own handlers, so no control decides the request.

## `private void QCorpusScribeObserve(object sender, RoutedEventArgs e)`

Swaps the reading for the editor on whichever side the corpus stands.
The gate asks before leaving an editor, so unsaved wording is never lost silently.

## `internal void QCorpusVoyageShow(bool past, bool future)`

Lights the two trail buttons from the voyage state the navigation raises.

## `private void QCorpusRetreatObserve(object sender, RoutedEventArgs e)`

Steps the navigation's trail back one station through `CNavigationStationUndo`.

## `private void QCorpusAdvanceObserve(object sender, RoutedEventArgs e)`

Steps the navigation's trail forward one station through `CNavigationStationRedo`.
