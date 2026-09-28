# LCorpus.cs

## `public sealed class LCorpus`

What is left of the corpus panel's deportment around the Conduct `CCorpus`.
The corpus Conduct holds the lists, the desk, the session, the mode and the gates.
This class keeps the quotation list's deportment, the lectern wiring, the prints and the vista restore.
It holds no port, and the quotation list keeps its own until its job.
Its constructor and vista restore take engine handles, so they stay internal.

## `internal LCorpus(CAtelier atelier, LEditor editor, LLectern lectern, Func<bool> shownSeam, CEnvoy envoy)`

Builds the corpus Conduct over the atelier and the entry editor, then the quotation list over its quotation panel.
The quotation list reads its ports off the atelier, so this class never holds one.
The quotation panel's loads and clears go straight to the lectern, so the veneer relays no draft.

## `private LEditor LCorpusEditor { get; }`

The entry editor's deportment on the quotation side, which takes the quotation vista when the vistas are restored.

## `public CCorpus LCorpusStudio { get; }`

The corpus Conduct, which the driver calls for every gate, read and mode flag.

## `public LQuotation LCorpusQuotation { get; }`

The quotation list, whose rows follow the chosen Example.

## `public void LCorpusRowsApply(IReadOnlyList<CCatalogExample> rows)`

Checks the rows the veneer has just applied, so a clear never re-enters a read in progress.
A chosen Example missing from them clears both lists while the excerpt shows it.

## `public void LCorpusDelete()`

Deletes the chosen Example, and does nothing on the quotation side.
A failure reads `Example.DeleteFailed`, the delete key `CAnthology` hands its panel.

## `public Task LCorpusPortraitPrint(CPortraitLabel label, CPortraitLegend legend, CPressTicket ticket)`

Prints the entry on display, else the chosen Example, else nothing.

## `public Task LCorpusPortraitExport(string path, CPortraitMedium format, CPortraitLabel label)`

Exports the entry on display, and does nothing otherwise.
