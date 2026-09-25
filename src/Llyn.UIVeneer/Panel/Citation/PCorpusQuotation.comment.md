# PCorpusQuotation.cs

## `public partial class PCorpus`

The middle column of the Corpus panel, and the entry display it opens on the right.
`PQuotation` lists the entries quoting the chosen Example, or every Entry while none is chosen.
A chosen row swaps the Example reading for the entry display in place, without leaving the tab.

## Inline notes

### Quotation vista

The child vista whose chosen entry the right-hand side stands on, while it shows an Entry and not an Example.
The right-hand side shows one or the other and never both.
An Entry row swaps the Example reading for the entry display in place, and an Example row swaps it back.
The chosen Example keeps its mark meanwhile, because it still narrows the middle column.

## `private void PQuotationFind()`

Refills the middle column with the entries quoting the chosen Example, or every Entry while none is chosen.
The engine matches the typed text and drops the hidden languages, so the panel decides nothing about what matches.
An empty result is shown rather than hidden, and its text says whether nothing quotes the Example or nothing matches.
Rows sharing a headword are numbered afterwards, so the reader can tell them apart.
The quotation panel raises the refill, so its row notice drives this method.

## `private void PQuotationHandle(object sender, RoutedEventArgs e)`

A clicked Entry row asks the deportment to show it, in the display or the editor, without leaving the tab.
The deportment asks about unsaved work first and cancels an open Example editor.
