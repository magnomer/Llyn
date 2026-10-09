# TEngineInflectionCell.cs
Hash: `e93f8ca290ed0cd5`

## `public sealed class TEngineInflectionCell`

Covers the engine's inflection fetch over a fixture pack whose verb paradigm states two-value cells.
The fixture's morphology source names each reading by the cell key, as the Spanish pack does.
The second cell is listed out of code order, so its key is read sorted.
Each fact runs the fetch in a first engine and reads the outcome in a reopened one.

## `public async Task InflectionStart_CellFound_StoresEveryValue()`

Both cells are found, and each stored form carries every value of its cell.
Nothing is recorded as missed.

## `public async Task InflectionStart_CellMissed_MarksOnlyThatCellUnknown()`

Only the first cell is found, so only the second reads unknown.
Its lacuna row names the cell by its key.

## `public async Task InflectionStart_AfterReopen_KeepsUnknownCell()`

A missed cell survives a reopen, so a new start asks the web nothing.

## `private static async Task<LEntry> TInflectionCellRun(TWorkspace workspace, TSourceHandler handler, string language)`

Saves a fixture verb, runs one fetch and disposes the engine, so every fact shares one setup.
The frequency fill is off, so the stub client only sees the morphology request.
The wait ends once the handler was asked and the fetch no longer stands.
The client leaves the handler alive on dispose, since the reopened engine sends through the same one.

## `private static TLanguageFixture TInflectionPackCreate()`

Writes the fixture's source and vocabulary before the workspace opens, so the engine imports its values.
