# LEngineDiwei.cs

## `public sealed partial class LEngine`

The diwei side of the engine: the 音韻地位 categories the stored placements of a language fall into.
The categories are derived, never edited, so the engine only reads them and keeps them in step.

## `internal IReadOnlyList<LDiwei> LEngineDiweiRead(string language, string kind)`

The categories of one kind with their entry counts.

## `public LDiwei? LEngineDiweiRead(long? id)`

One category by its id, or null for no id and for an id no longer stored.

## `public LDiweiPage LEngineDiweiResolve(long? id, Func<string, string?> localize)`

The page of one category: its fanqie rows grouped and sorted, with the tally of the set the settings choose.
The respelling set is shown only where the language has a respelling and the setting asks for it.
The blank page stands for no category, so the caller never branches on null.

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

## `public IReadOnlyList<LVistaRow> LEngineXiaoyunFind(string language, LVista onset, LVista rime, LVista vista)`

The entries at the cell the two column vistas stand on, narrowed by the entry vista's query.
No chosen cell lists nothing, since a cell is what the list is about.

## `internal void LEngineDiweiRebuild()`

Derives every category again on request, so an edited hypothesis file shows without a restart.

## `private void LEngineDiweiApply()`

Derives the categories of every language with rime books again, from the stored rows and the hypothesis.
Run when the engine binds a workspace, so an edited hypothesis file shows in the classes at the next start.
