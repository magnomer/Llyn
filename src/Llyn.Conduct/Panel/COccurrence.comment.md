# COccurrence.cs

## `public sealed class COccurrence`

The repertoire panel's occurrence list: the entries referencing the chosen Situation.
It holds the entry and portrait ports, the situation vista as its roll and its own vista.
The occurrence side never deletes, so its panel has no delete scope.
It is sealed, so its rows, labels, tickets and formats cross as Conduct shapes.

## `internal COccurrence(`

Builds the list's panel under the `List.LoadFailed` key and with no delete scope.
The change seam is the entry editor's desk, and the finish seam is the repertoire session's.
It takes engine ports, so it stays internal to the repertoire that builds it.

## `public CPanel COccurrencePanel { get; }`

The panel that holds the chosen entry and the edit mode of the occurrence side.

## `public string COccurrenceEmptyKey`

The wording key of the empty list, chosen by whether the vista holds a query.
No query means nothing references the Situation, and a query means nothing matched.

## `internal void COccurrenceVistaRestore(LVista roll, LVista vista)`

Takes the situation vista as the roll the rows follow, and its own vista for the panel.
It takes engine vistas, so only the repertoire's vista restore calls it.

## `public void COccurrenceQuerySet(string query)`

Takes the text typed into the search field as the list's query.

## `public IReadOnlyList<CVistaRow> COccurrenceRowsRead()`

Reads the entries referencing the roll's chosen Situation, or every entry while none is chosen.
The engine matches the query and drops the hidden languages, so the list decides nothing about matching.

## `public string COccurrenceFileRead()`

The file name an export of the entry on display is offered under.

## `public Task COccurrencePortraitPrint(CPortraitLabel label, CPressTicket ticket)`

Prints the chosen entry, mapping the label and ticket to the engine's records unread.

## `public Task COccurrencePortraitExport(string path, CPortraitMedium format, CPortraitLabel label)`

Exports the chosen entry to `path` in the chosen format.
