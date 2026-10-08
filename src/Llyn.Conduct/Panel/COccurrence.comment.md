# COccurrence.cs
Hash: `1681ee24647b97b8`

## `public sealed class COccurrence`

The repertoire panel's occurrence list: the entries referencing the chosen Situation.
It holds the vista and portrait ports and the situation vista as its roll.
Its own vista sits in the panel's aperture.
The occurrence side never deletes, so its panel has no delete scope.
It is sealed, so its rows, labels, tickets and formats cross as Conduct shapes.

## `internal COccurrence(LVistaPort vistas, LPortraitPort portraits, LSettingsPort settings, CEnvoy envoy, Func<bool> changeSeam, Func<bool, bool> finishSeam, Func<bool> shownSeam)`

Builds the list's panel under the `List.LoadFailed` key and with no delete scope.
The empty list reads `Situation.Vacant` when nothing references the Situation, and `Situation.Unmatched` under a search.
The change seam is the entry editor's desk, and the finish seam is the repertoire session's.
It takes engine ports, so it stays internal to `CRepertoire`, which builds it.

## `public CPanel COccurrencePanel { get; }`

The panel that holds the chosen entry and the edit mode of the occurrence side.

## `internal void LOccurrenceVistaRestore(LVista roll, LVista vista)`

Takes the situation vista as the roll the rows follow, and its own vista for the panel.
It takes engine vistas, so only the repertoire's vista restore calls it.
The aperture carries the former query into the fresh vista, so a switched workspace keeps the search.

## `internal void LOccurrenceObserverAttach(Action<Action> marshal, Action roll, Action chosen)`

Attaches the subjects that refill the entry column on the fresh vista, each answer run through the marshal.
An entry notice selects a stored entry, refills the situation rows, and hands the chosen entry to the area.
A vista notice raises the column's own rows.

## `public IReadOnlyList<CVistaRow> COccurrenceRowsRead()`

Reads the entries referencing the roll's chosen Situation, or every entry while none is chosen.
The engine matches the query and drops the hidden languages, so the list decides nothing about matching.
A failed read shows the `Situation.LoadFailed` notice once through the ledger and answers no rows.

## `public Task<CEnsignSheet<IReadOnlyList<CVistaRow>>> COccurrenceRowsLoad(Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)`

Runs the flag fill into the driver's `store`, then answers `COccurrenceRowsRead` beside the loaded languages.
The shared rule `CCatalog.LCatalogEnsignLoad` orders the two, so the driver makes one request.
A failed flag fill shows `Situation.LoadFailed` and still answers the rows with no languages.
The driver awaits it from an event handler, where a fault would end the app.

## `internal string LOccurrenceFileRead()`

The file name an export of the entry on display is offered under.
`CPortrait` reads it only once the export starts, and a failed read shows `Export.NameFailed`.

## `internal Task LOccurrencePortraitPrint(CEnvoy envoy, LSettingsPort settings)`

Prints the chosen entry through `CPortrait`, worded through `settings`, with failures shown through `envoy`.
Only the repertoire's print gate calls it, which chooses the side that prints.

## `internal Task LOccurrencePortraitExport(CEnvoy envoy, LSettingsPort settings)`

Exports the chosen entry under the file name and format the reader picks through `envoy`.
Only the repertoire's export gate calls it, which checks that an entry is on display.
