# CAtlas.cs

## `public sealed class CAtlas`

The repertoire panel's situation list: the situation vista it holds and the panel over it.
It finds the rows and their usage, and takes the query, order and language filter.
Its panel loads, edits and deletes the chosen Situation, worded under the Situation scope.
It also prints the listed situations.

## `internal CAtlas(LEntryPort entries, LPortraitPort portraits, LSettingsPort settings, CDesk desk, Func<bool> shownSeam, CEnvoy envoy, Func<bool, bool> finishSeam)`

Takes the ports it reads and prints through, the repertoire desk holding the situation, and the panel's seams.
The panel asks whether the desk changed before it leaves a Situation.

## `public static CAtlas CAtlasCreate(CAtelier atelier, CDesk desk, Func<bool> shownSeam, CEnvoy envoy, Func<bool, bool> finishSeam)`

Builds the list over the atelier's ports, so the driver hands it no port.
Building it is no user action, so it is no gate on the atelier.

## `public bool CAtlasNarrowed`

Whether the query or the filter hides any row, as the vista answers it.

## `internal void CAtlasVistaRestore(LVista vista)`

Binds the list and its panel to the repertoire vista a workspace start or switch hands over.

## `public void CAtlasOrderSet(CCatalogOrder? order)`

Hands a chosen ordering to the vista, which keeps its own when the sender is no order row.

## `public IReadOnlyList<CCatalogSituation> CAtlasRowsRead(string unknown, string untitled)`

The rows the vista lists, none before a vista arrives.
The driver hands the words for an unknown or untitled situation, and the engine names each row with them.

## `public IReadOnlyDictionary<long, int> CAtlasUsageRead()`

How many places cite each Situation, keyed by id.

## `public IReadOnlyList<string> CAtlasLanguageRead()`

The workspace's languages, which the driver offers as the filter's choices.

## `public Task CAtlasPortraitPrint(CPortraitLegend legend, CPressTicket ticket)`

Prints the vista's situations with the driver's localized legend and dialog answer.

## `internal static CCatalogSituation CAtlasRowRead(LCatalogSituation row)`

The one map for a found situation, shared by the candidate popup and the atlas.
The title is the name the engine gave the row, so the atlas keeps its unknown and untitled words.

## `internal static CSituationDraft? CAtlasSituationRead(LSituation? situation)`

Maps a whole stored Situation to its shape, and null to null.
The repertoire desk still calls it until the repertoire session reaches Conduct.
