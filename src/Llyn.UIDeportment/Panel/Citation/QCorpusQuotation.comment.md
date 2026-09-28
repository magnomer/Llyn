# QCorpusQuotation.cs

## `internal sealed partial class QCorpus`

The middle column of the Corpus panel, and the entry display it opens on the right.
`QQuotation` lists the entries quoting the chosen Example, or every Entry while none is chosen.
A chosen row swaps the Example reading for the entry display in place, without leaving the tab.

## `private void QQuotationFind()`

Refills the middle column with the rows the quotation deportment reads.
The engine matches the dredge text and drops the hidden languages, so the driver decides nothing about what matches.
An empty result is shown rather than hidden.
Its wording is the deportment's `LQuotationEmptyRead`, which reads the dredge field's text.

## `private void QQuotationHandle(object sender, RoutedEventArgs e)`

A clicked Entry row asks the gate `CCorpusQuotationSelect` to show it without leaving the tab.
It shows in the display or the editor, whichever the mode holds.

## `private void QQuotationApply(FrameworkElement container, object item, string? _)`

Fills one quotation row from its item.
The row carries the `Chosen` cue on the chosen item and none otherwise, which the look sheet paints.
The click is subscribed once per row, removed first so a refill never doubles it.
