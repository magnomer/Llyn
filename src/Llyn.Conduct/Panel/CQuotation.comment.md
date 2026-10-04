# CQuotation.cs
Hash: `0e2f5eb2c6d6b1a3`

## `public sealed class CQuotation`

The corpus panel's quotation list: the entries quoting the chosen Example.
It holds the entry and portrait ports, the example vista as its roll and its own vista.
The corpus builds it and restores its vistas, so no driver holds a port or a vista.
It is sealed, so its rows, labels, tickets and formats cross as Conduct shapes.

## `internal CQuotation(LEntryPort entries, LPortraitPort portraits, LSettingsPort settings, CEnvoy envoy, Func<bool> changeSeam, Func<bool, bool> finishSeam, Func<bool> shownSeam)`

Builds the list's panel under the `List.LoadFailed` key and with no delete scope.
The change seam is the entry editor's desk, and the finish seam is the corpus session's.

## `public CPanel CQuotationPanel { get; }`

The panel that holds the chosen entry and the edit mode of the quotation side.

## `public bool CQuotationShown`

Whether the quotation side is on and its entry shows outside edit mode, which gates the export.

## `public string CQuotationEmptyKey`

The wording key of the empty list, chosen by whether the vista holds a query.
No query means nothing quotes the Example, and a query means nothing matched.

## `internal void LQuotationVistaRestore(LVista roll, LVista vista)`

Takes the example vista as the roll the rows follow, and its own vista for the panel.
The query the former vista held is carried into the fresh one first.
Only the corpus's vista restore calls it, so it takes `L`.

## `internal void LQuotationObserverAttach(Action<Action> marshal, Action roll, Action chosen)`

Attaches the list's observers on its vista, each run through the corpus's `marshal`.
An entry notice goes to the panel first, which may adopt a freshly stored Entry.
The example rows follow through `roll`, since the store may quote an Example.
The chosen entry's own notice runs `chosen`, which redraws or drops it.
A vista notice refills the list's own rows.

## `public void CQuotationQuerySet(string query)`

Takes the text typed into the dredge field as the list's query.

## `public IReadOnlyList<CVistaRow> CQuotationRowsRead()`

Reads the entries quoting the roll's chosen Example, or every entry while none is chosen.
A failed read shows `Example.LoadFailed` through the envoy and answers no rows.
The engine matches the query and drops the hidden languages, so the list decides nothing about matching.

## `public Task<CEnsignSheet<IReadOnlyList<CVistaRow>>> CQuotationRowsLoad(Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)`

Runs the flag fill into the driver's `store`, then answers `CQuotationRowsRead` beside the loaded languages.
The shared rule `CCatalog.LCatalogEnsignLoad` orders the two, so the driver makes one request.

## `internal string LQuotationFileRead()`

The file name an export of the entry on display is offered under.
`CPortrait` reads it only once the export starts, and a failed read shows `Export.NameFailed`.

## `internal Task LQuotationPortraitPrint(CEnvoy envoy, LSettingsPort settings)`

Prints the chosen entry through `CPortrait`, worded through `settings`, with failures shown through `envoy`.
No driver calls it, since `CCorpusPortraitPrint` chooses which side prints.

## `public Task CQuotationPortraitExport()`

Exports the entry on display, and does nothing otherwise.
The export belongs to the quotation list, since only its shown entry exports.
The reader is asked for the file and format through the envoy, and a decline exports nothing.
`CPortrait` words the page through the engine and shows `Export.Failed` through the envoy.
