# LEngineDiwei.cs

## `public sealed partial class LEngine`

The diwei side of the engine: the 音韻地位 categories the stored placements of a language fall into.
The categories are derived, never edited, so the engine only reads them and keeps them in step.

## `internal IReadOnlyList<LDiwei> LEngineDiweiRead(string language, string kind)`

The categories of one kind with their entry counts.

## `public LDiwei? LEngineDiweiFind(string language, string kind, string key)`

The category a link in the reading view names, or `null` when nothing is placed there.

## `public IReadOnlyList<LDiwei> LEngineDiweiFind(LVista vista, string language, string kind)`

The categories of one kind as the yunjing panel's vista lists them.
A category answers the vista's query when its key carries the trimmed text, case aside.
The rows come by key, by key reversed, or by entry count with the key breaking ties.
The vista's filter names languages and the language is already given, so it is not applied here.

## `public IReadOnlyList<LFanqieRow> LEngineFanqieRead(LDiwei diwei)`

Every stored placement linked to one category, for the category page to group.

## `internal IReadOnlyList<long> LEngineDiweiScan(string language, IReadOnlyList<long> diweiIds)`

The entries sitting at the cell the given categories name together.

## `public IReadOnlyList<LVistaRow> LEngineXiaoyunFind(string language, IReadOnlyList<long> diweiIds, string query)`

The entries of one rime table cell as the yunjing panel lists them, ready to show.
The cell's entry ids are scanned once, then the entries among them matching the headword query are read once.
Only those ids are read, so a cell never costs the whole entry table.
Rows keep id order and are built by the vista row builder, twins numbered and epithets read in one scan.
No row is marked chosen, because the panel's shown entry is its own to mark.

## `internal void LEngineDiweiRebuild()`

Derives every category again on request, so an edited hypothesis file shows without a restart.

## `private void LEngineDiweiApply()`

Derives the categories of every language with rime books again, from the stored rows and the hypothesis.
Run when the engine binds a workspace, so an edited hypothesis file shows in the classes at the next start.
