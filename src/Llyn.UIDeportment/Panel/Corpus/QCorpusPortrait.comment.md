# QCorpusPortrait.cs
Hash: `2f360e25db81a22c`

## `internal sealed class QCorpusPortrait`

The print and export commands of the Corpus panel, which the rail's buttons reach.
Both act on what the display reads, so neither needs any other part of the panel.
The owner `QCorpus` only builds and introduces it.

## `private CCorpus _cCorpus`

The corpus Conduct, whose verdicts and gates the two commands call.
It is null until the owner hands one over, so the command checks answer false before that.

## `internal QCorpusPortrait(UserControl scope)`

Takes the Corpus page and adds the print and export command bindings to it.

## `internal void QCorpusPortraitIntroduce(CCorpus corpus)`

`QCorpusIntroduce` calls it once the corpus Conduct exists.

## `private void QCorpusPressCheck(object sender, CanExecuteRoutedEventArgs e)`

Whether the print button is live, which holds while an entry or an example is read.
An editor on screen prints nothing, because what is printed is what is read.

## `private async void QCorpusPressObserve(object sender, ExecutedRoutedEventArgs e)`

Hears the print command and calls the one gate `CCorpusPortraitPrint`.
The gate picks the page from the side in front and asks for the printer.
The engine builds the page from stored rows.

## `private void QCorpusPortraitCheck(object sender, CanExecuteRoutedEventArgs e)`

Whether the export button is live, exactly when an entry is read in the display.
Print may also act on the other page this panel reads, but export acts on entries alone.

## `private async void QCorpusPortraitObserve(object sender, ExecutedRoutedEventArgs e)`

Hears the export command and calls the one gate `CQuotationPortraitExport`, which exports the entry being read.
The gate asks for the file and the format through the envoy.
The engine writes the document from stored rows.
