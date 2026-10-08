# CAtlas.cs
Hash: `bd61b70482851f4b`

## `public sealed class CAtlas`

The repertoire panel's situation list: the panel over the situation vista.
It finds the rows and their usage, while the panel's aperture takes the query, order and language filter.
Its panel loads, edits and deletes the chosen Situation, worded under the Situation scope.
It also prints the listed situations.

## `internal CAtlas(LSituationPort situations, LPortraitPort portraits, LSettingsPort settings, LVistaPort vistas, CDesk desk, Func<bool> shownSeam, CEnvoy envoy, Func<bool, bool> finishSeam)`

Takes the ports it reads and prints through, the repertoire desk holding the situation, and the panel's seams.
The panel asks whether the desk changed before it leaves a Situation.
`vistas` passes to the panel, which loads and deletes its chosen row through it.
Only `CRepertoire` builds it, over the atelier's ports.

## `public CPanel CAtlasPanel { get; }`

The panel over the situation vista, built once with the Situation scope and the repertoire seams.

## `internal void LAtlasVistaRestore(LVista vista)`

Binds the list's aperture to the repertoire vista a workspace start or switch hands over.
The aperture carries the former vista's query into the fresh one, so a switched workspace keeps the search.

## `internal void LAtlasObserverAttach(Action<Action> marshal, Action workspace)`

Attaches the subjects that refill the list on the fresh vista, each answer run through the marshal.
A vista, Situation, reflex or settings notice raises the panel's rows.
A workspace notice runs the answer the area hands in.

## `public static IReadOnlyList<CCatalogOrder> CAtlasOrderRead()`

The orderings the list offers, in menu order.
A driver builds its ordering menu from it once.

## `internal IReadOnlyList<CCatalogSituation>? LAtlasRowsRead()`

The rows the vista lists, none before a vista arrives.
Drivers read them through `CRepertoireRowsRead`, which closes a stale selection.
The engine's own words name an unknown kind and an untitled situation.
A failed read shows the load failure through the ledger and answers null.

## `public IReadOnlyList<string> CAtlasLanguageRead()`

The workspace's languages, which the driver offers as the filter's choices.

## `internal Task LAtlasPortraitPrint(CEnvoy envoy, LSettingsPort settings)`

Prints the vista's situations through `CPortrait`, with the Situation realm's legend.
Only the repertoire's print gate calls it, which chooses the side that prints.

## `internal static CCatalogSituation LAtlasRowRead(LCatalogSituation row)`

The one map for a found situation to its row.
The title is the name the engine gave the row, so the atlas keeps its unknown and untitled words.
The kind and the count arrive worded, so no driver words either.

## `internal static CSituationDraft? LAtlasDraftRead(LDraft? draft, LMediaPort media)`

Maps the Situation a draft carries to its shape, and a missing draft or Situation to null.
It is a plain map with no rule, so it stays in Conduct, since ShellEngine cannot name a Conduct shape.
`CPlaywright` hands it the desk's draft unread, and the scenario fills from it.

## `internal static CSituation? LAtlasSituationRead(LDraft? draft, LMediaPort media, LMarkdownPort markdown)`

Maps the Situation a loaded atlas draft carries to the vignette's ready page, and a missing one to null.
It only chooses keys.
The title words untitled while unwritten, and the kind and description word nothing.
The engine parses the description's text into Markdown blocks, so the vignette never parses.
The engine answers which media rows are filled, so the map picks no row itself.
