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

Prints the vista's situations with the driver's words, turned into the engine's legend here.

## `internal static LPortraitLegend LAtlasLegendRead(CPortraitLegend legend)`

The one map from the driver's legend words to the engine's legend.
Each source kind takes the word keyed by its localization key.
A missing word falls back to the name the engine gives the kind.
It sits here until the shared panel map has room, since only the atlas prints with a shape legend yet.

## `internal static CSituationDraft? LAtlasSituationRead(LSituation? situation)`

The one map for a whole situation, read by the repertoire's scenario, its vignette and the text gate.
It answers null when no situation is held.
