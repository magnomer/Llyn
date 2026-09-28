# QCorpusBrowse.cs

## `internal sealed partial class QCorpus`

Browsing behavior of the Corpus panel.
`QAnthology` lists every Example the workspace holds, including one nothing quotes.
A chosen row is read back and shown, and the middle column narrows to the entries quoting it.
`QCorpusScribe` swaps the reading for the editor.
The panel answers two questions rather than one: what this sentence is, and where it is quoted.

## `private IReadOnlyDictionary<long, int> _qAnthologyCount`

How many places quote each Example, read once per catalog fill rather than once per row.
Both tally chips read it, so it is held rather than asked for again.

## `private async void QCorpusWorkspaceUpdate()`

A workspace that moved reloads the flags and the language menu first, since neither belongs to the old folder's rows.
The panel is then emptied and listed again.

## `private void QQueryHandle(object sender, TextChangedEventArgs e)`

Each keystroke hands the search text to the vista, whose announcement refills the catalog.

## `private void QRankHandle(object sender, RoutedEventArgs e)`

A chosen ordering closes the dropdown and hands the ordering to the vista.
The vista saves it and announces it, and the announcement refills the catalog.

## `internal async void QCorpusVistaRestore()`

Takes the vistas the window started for this tab and puts the panel on them.
The dropdown mark and the filter mark are drawn from the anthology first.
The flags are loaded before any row is built, then the language menu is built from the loaded packs.
Search text still standing in the box is handed to the vista, so a switched workspace keeps the search.
The speakers, the citations and the catalog are then listed.

## `private void QCorpusObserverAttach()`

Attaches each subject the panel cares about once, so no handler sorts announcements by subject.
A vista announcement refills the catalog, since order, filter or query moved.
A stored Example or Source refills the citations and the catalog, because a row cites a Source.
A reflex fill or a flipped setting rewrites the epithet beside a headword, so each refills the catalog too.
An entry announcement goes to the quotation panel first, which may adopt a freshly stored Entry.
The citations and the catalog follow, since the store may quote an Example.
The chosen entry's own announcement reaches `CCorpusEntryResonate` last, which redraws or drops it.
Every observer is bound to the surface, since the driver is no control.

## `private void QGauzeRestore()`

Shows the filter mark while the vista hides any language.

## `private void QGauzeBuild()`

Builds the language menu from the loaded packs, ticked as the vista's filter reads.
The restore and the dropped search share it.

## `private void QQueryClear()`

Empties the search box and rebuilds the language menu after an arrival dropped both on the vista.

## `private void QDredgeHandle(object sender, TextChangedEventArgs e)`

Each keystroke hands the dredge text to the quotation vista, whose announcement refills the entry column.

## `private void QGauzeHandle(object sender, RoutedEventArgs e)`

The ticked languages are read off the clicked box's menu and handed to the vista, which saves and announces them.
The mark on the button is redrawn from the vista at once.

## `private void QAnthologyFind()`

Refills the catalog with the rows the engine returns for the vista, already matched and already ordered.
The driver hands over nothing and decides nothing about the ordering or the query.
The applied rows then go to the deportment, which drops a shown selection whose row no longer stands.
Both tally chips are rewritten from the fresh counts, so a quotation added elsewhere shows at once.

## `private void QAnthologyHandle(object sender, RoutedEventArgs e)`

A clicked row asks the gate `CCorpusExampleSelect` to select it, handing over the trail's record.
The gate asks about unsaved work first, and records the station only when the move goes ahead.

## `private void QAnthologyApply(FrameworkElement container, object item, string? _)`

Fills one catalog row from its item.
The row carries the `Chosen` cue on the chosen item and none otherwise, which the look sheet paints.
The click is subscribed once per row, removed first so a refill never doubles it.

## `private void QCorpusBinHandle(object sender, RoutedEventArgs e)`

Hands the delete to the deportment, which acts only while an Example and not an Entry is shown.

## `private string QCorpusTallyRead(long? id)`

The usage count as the tally chip reads it, a sentence rather than a bare number.

## `private void QCorpusStoreHandle(object sender, RoutedEventArgs e)`

The rail's save, standing for whichever editor is in front.

## `private void QCorpusViewerHandle(object sender, RoutedEventArgs e)`

Puts whichever side the corpus stands on back on its reading page, through `CCorpusScribeToggle`.
The viewer and scribe segments have their own handlers, so no control decides the request.

## `private void QCorpusScribeHandle(object sender, RoutedEventArgs e)`

Swaps the reading for the editor on whichever side the corpus stands.
The gate asks before leaving an editor, so unsaved wording is never lost silently.

## `internal void QCorpusVoyageShow(bool past, bool future)`

Lights the two trail buttons from the voyage state the navigation raises.

## `private void QCorpusRetreatHandle(object sender, RoutedEventArgs e)`

Steps the navigation's trail back one station.

## `private void QCorpusAdvanceHandle(object sender, RoutedEventArgs e)`

Steps the navigation's trail forward one station.
