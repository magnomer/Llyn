# CAtlas.cs

## `public sealed class CAtlas`

The repertoire panel's situation list: the situation vista it holds and the panel over it.
It finds the rows and their usage, and takes the query, order and language filter.
Its panel loads, edits and deletes the chosen Situation, worded under the Situation scope.
It also prints the listed situations.

## `internal CAtlas(LEntryPort entries, LPortraitPort portraits, LSettingsPort settings, CDesk desk, Func<bool> shownSeam, CEnvoy envoy, Func<bool, bool> finishSeam)`

Takes the ports it reads and prints through, the repertoire desk holding the situation, and the panel's seams.
The panel asks whether the desk changed before it leaves a Situation.
Only `CRepertoire` builds it, over the atelier's ports.

## `internal bool LAtlasNarrowed`

Whether the query or the filter hides any row, as the vista answers it.
Only `CRepertoire` reads it, when an arrival lands on a hidden row.

## `internal void LAtlasVistaRestore(LVista vista)`

Binds the list and its panel to the repertoire vista a workspace start or switch hands over.

## `public void CAtlasOrderSet(CCatalogOrder? order)`

Hands a chosen ordering to the vista, which keeps its own when the sender is no order row.

## `internal IReadOnlyList<CCatalogSituation> LAtlasRowsRead(string unknown, string untitled)`

The rows the vista lists, none before a vista arrives.
Drivers read them through `CRepertoireRowsRead`, which closes a stale selection.
The driver hands the words for an unknown or untitled situation, and the engine names each row with them.

## `public IReadOnlyDictionary<long, int> CAtlasUsageRead()`

How many places cite each Situation, keyed by id.

## `public IReadOnlyList<string> CAtlasLanguageRead()`

The workspace's languages, which the driver offers as the filter's choices.

## `internal Task LAtlasPortraitPrint(CEnvoy envoy, LSettingsPort settings)`

Prints the vista's situations through `CPortrait`, with the Situation realm's legend.
Only the repertoire's print gate calls it, which chooses the side that prints.

## `internal static CCatalogSituation LAtlasRowRead(LCatalogSituation row)`

The one map for a found situation, shared by the Proffer dropdown and the atlas.
The title is the name the engine gave the row, so the atlas keeps its unknown and untitled words.

## `internal static CSituationDraft? LAtlasDraftRead(LDraft? draft)`

Maps the Situation a draft carries to its shape, and a missing draft or Situation to null.
It is a plain map with no rule, so it stays in Conduct, since ShellEngine cannot name a Conduct shape.
`CRepertoire` hands it the desk's draft and every loaded atlas draft unread.
