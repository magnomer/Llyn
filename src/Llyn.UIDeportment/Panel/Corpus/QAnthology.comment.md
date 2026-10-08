# QAnthology.cs
Hash: `a770dbcc55e7ffd8`

## `internal sealed class QAnthology`

The left column of the Corpus panel, with the Example search field over it.
It lists every Example the workspace holds, including one nothing quotes.
A chosen row is read back and shown, and the middle column narrows to the entries quoting it.
It subscribes what it paints itself, so the owner `QCorpus` only builds and introduces it.

## `internal QAnthology(UserControl scope)`

Takes the Corpus page, and finds `PQuery`, `PAnthology` and `PAnthologyEmpty` in it by contract ID.
It sets the search hint and subscribes the search field.

## `internal void QAnthologyIntroduce(CCorpus corpus)`

`QCorpusIntroduce` calls it once the corpus Conduct exists.
It subscribes the anthology's rows event and the corpus's dropped search.
It binds the list and attaches its row fill.

## `private void QQueryObserve(object sender, TextChangedEventArgs e)`

Each keystroke hands the search text to the vista, whose announcement refills the catalog.

## `private void QQueryClear()`

Empties the search box after an arrival dropped the query on the vista.
The owner's `QCorpusClearRefine` rebuilds the filter menu on the same notice.

## `private void QAnthologyRefine()`

Refills the catalog with the ready rows `CCorpusRowsRead` answers, already matched, ordered and worded.
The driver hands over nothing and decides nothing about the ordering, the query or the wording.
The read reports its own failure, and it drops a shown selection whose row no longer stands.

## `internal void QAnthologyRefine(IReadOnlyList<CCatalogExample> rows)`

Paints `rows` the area answered ready, so the paint itself asks Conduct nothing.
The parameterless form reads them, and the owner's flag-fill Refine hands in what its load answered.

## `private void QAnthologyObserve(object sender, RoutedEventArgs e)`

A clicked row asks the corpus diptych's gate `CDiptychParentSelect` to select it.
The gate asks about unsaved work first, and records the station only when the move goes ahead.

## `private void QAnthologyApply(FrameworkElement container, object item, string? _)`

Fills one catalog row from its item.
The row carries the `Chosen` cue on the chosen item and none otherwise, which the look sheet paints.
The click is subscribed once per row, removed first so a refill never doubles it.
