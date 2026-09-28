# CQuotation.cs

## `public sealed class CQuotation`

The corpus panel's quotation list: the entries quoting the chosen Example.
It holds the entry and portrait ports, the example vista as its roll and its own vista.
The corpus builds it and restores its vistas, so no driver holds a port or a vista.
It is sealed, so its rows, labels, tickets and formats cross as Conduct shapes.

## `internal CQuotation(`

Builds the list's panel under the `List.LoadFailed` key and with no delete scope.
The change seam is the entry editor's desk, and the finish seam is the corpus session's.

## `public CPanel CQuotationPanel { get; }`

The panel that holds the chosen entry and the edit mode of the quotation side.

## `public string CQuotationEmptyKey`

The wording key of the empty list, chosen by whether the vista holds a query.
No query means nothing quotes the Example, and a query means nothing matched.

## `internal void CQuotationVistaRestore(LVista roll, LVista vista)`

Takes the example vista as the roll the rows follow, and its own vista for the panel.

## `public void CQuotationQuerySet(string query)`

Takes the text typed into the dredge field as the list's query.

## `public IReadOnlyList<CVistaRow> CQuotationRowsRead()`

Reads the entries quoting the roll's chosen Example, or every entry while none is chosen.
The engine matches the query and drops the hidden languages, so the list decides nothing about matching.

## `public string CQuotationFileRead()`

The file name an export of the entry on display is offered under.

## `internal Task LQuotationPortraitPrint(CPortraitLabel label, CPressTicket ticket)`

Prints the chosen entry, mapping the label and ticket to the engine's records unread.
No driver calls it, since `CCorpusPortraitPrint` chooses which side prints.

## `internal Task LQuotationPortraitExport(string path, CPortraitMedium format, CPortraitLabel label)`

Exports the chosen entry to `path` in the chosen format.
