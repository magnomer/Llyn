# LAtlas.cs

## `public sealed class LAtlas`

The deportment of the repertoire panel's situation list: the panel state over the situation vista and its rows.
The list finds rows and their usage, and takes the inquest, tier and mesh.
Its panel loads, edits and deletes the chosen Situation.
The panel's delete question is worded under the Situation scope and counted by the vista.

## `public bool LAtlasNarrowed`

Whether the inquest or the filter hides any row, as the vista answers it.

## `public IReadOnlyList<CCatalogSituation> LAtlasRowsRead(string unknown, string untitled)`

The matching rows as shapes, with the engine's names for unknown and untitled situations.
No row is read while no vista is restored.

## `public Task LAtlasPortraitPrint(CPortraitLegend legend, CPressTicket ticket)`

Prints the vista's situations with the driver's words, turned into the engine's legend by `CPortrait`.

## `internal static CSituationDraft? LAtlasSituationRead(LSituation? situation)`

The one map for a whole situation, read by the repertoire's scenario, its vignette and the text gate.
It answers null when no situation is held.
