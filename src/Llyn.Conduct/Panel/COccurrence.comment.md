# COccurrence.cs

## `public sealed class COccurrence`

The repertoire panel's occurrence list: the entries referencing the chosen Situation.
It holds the entry and portrait ports, the situation vista as its roll and its own vista.
The occurrence side never deletes, so its panel has no delete scope.
It is sealed, so its rows, labels, tickets and formats cross as Conduct shapes.

## `internal COccurrence(`

Builds the list's panel under the `List.LoadFailed` key and with no delete scope.
The change seam is the entry editor's desk, and the finish seam is the repertoire session's.
It takes engine ports, so it stays internal to `CRepertoire`, which builds it.

## `public CPanel COccurrencePanel { get; }`

The panel that holds the chosen entry and the edit mode of the occurrence side.

## `public string COccurrenceEmptyKey`

The wording key of the empty list, chosen by whether the vista holds a query.
No query means nothing references the Situation, and a query means nothing matched.

## `internal void LOccurrenceVistaRestore(LVista roll, LVista vista)`

Takes the situation vista as the roll the rows follow, and its own vista for the panel.
It takes engine vistas, so only the repertoire's vista restore calls it.

## `public void COccurrenceQuerySet(string query)`

Takes the text typed into the search field as the list's query.

## `public IReadOnlyList<CVistaRow> COccurrenceRowsRead()`

Reads the entries referencing the roll's chosen Situation, or every entry while none is chosen.
The engine matches the query and drops the hidden languages, so the list decides nothing about matching.

## `internal string LOccurrenceFileRead()`

The file name an export of the entry on display is offered under.

## `internal Task LOccurrencePortraitPrint(CEnvoy envoy, LSettingsPort settings)`

Prints the chosen entry through `CPortrait`, worded through `settings`, with failures shown through `envoy`.
Only the repertoire's print gate calls it, which chooses the side that prints.

## `internal Task LOccurrencePortraitExport(CEnvoy envoy, LSettingsPort settings)`

Exports the chosen entry to `path` in the chosen format.
Only the repertoire's export gate calls it, which checks that an entry is on display.
