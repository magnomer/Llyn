# QQuotation.cs
Hash: `19aa8cfe966fbd31`

## `internal sealed class QQuotation`

The middle column of the Corpus panel, with the dredge field over it.
It lists the entries quoting the chosen Example, or every Entry while none is chosen.
A chosen row swaps the Example reading for the entry display in place, without leaving the tab.
It subscribes what it paints itself, so the owner `QCorpus` only builds and introduces it.

## `internal QQuotation(UserControl scope)`

Takes the Corpus page, and finds `PDredge`, `PQuotation` and `PQuotationEmpty` in it by contract ID.
It sets the dredge hint and subscribes the dredge field.

## `internal void QQuotationIntroduce(CCorpus corpus, CAtelier atelier)`

`QCorpusIntroduce` calls it once the corpus Conduct exists.
It subscribes the quotation list's rows event.
It takes the atelier only to hear the workspace opening, which gives the list its first paint.
It binds the list and attaches its row fill.

## `private async void QQuotationVistaRefine()`

Answers `CWorkspaceOpened` with the middle column's first paint in the workspace.
A row keeps the flag it was built with, so the flags are loaded before the rows are read.
The catalog's restore paints the catalog, so each list reads only its own rows.
Its one request is `CQuotationRowsLoad`, which runs the flag fill and then answers the rows it paints.

## `private void QDredgeObserve(object sender, TextChangedEventArgs e)`

Each keystroke hands the dredge text to the quotation vista, whose announcement refills the entry column.

## `private void QQuotationRefine()`

Refills the middle column with the rows the quotation list reads, when the list's rows change.
The engine matches the dredge text and drops the hidden languages, so the driver decides nothing about what matches.
A failed read is reported by the list, which answers no rows.
An empty result is shown rather than hidden.
Its wording key is the aperture's `CApertureKey`, chosen by whether the list holds a query.

## `private void QQuotationRefine(IReadOnlyList<CVistaRow> rows)`

Paints `rows` the area answered ready, so the paint itself asks Conduct nothing.
The parameterless form reads them, and a flag-fill Refine hands in what its load answered.

## `private void QQuotationObserve(object sender, RoutedEventArgs e)`

A clicked Entry row asks the corpus diptych's gate `CDiptychChildSelect` to show it without leaving the tab.
It shows in the display or the editor, whichever the mode holds.

## `private void QQuotationApply(FrameworkElement container, object item, string? _)`

Fills one quotation row from its item.
The row carries the `Chosen` cue on the chosen item and none otherwise, which the look sheet paints.
The click is subscribed once per row, removed first so a refill never doubles it.
