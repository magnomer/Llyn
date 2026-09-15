# LEngineDiwei.cs

## `public sealed partial class LEngine`

The diwei side of the engine: the 音韻地位 categories the stored placements of a language fall into.
The categories are derived, never edited, so the engine only reads them and keeps them in step.

## `public IReadOnlyList<LDiwei> LEngineDiweiRead(string language, string kind)`

The categories of one kind with their entry counts.

## `public LDiwei? LEngineDiweiFind(string language, string kind, string key)`

The category a link in the reading view names, or `null` when nothing is placed there.

## `public IReadOnlyList<long> LEngineDiweiScan(string language, IReadOnlyList<long> diweiIds)`

The entries sitting at the cell the given categories name together.

## `public void LEngineDiweiRebuild()`

Derives every category again on request, so an edited hypothesis file shows without a restart.

## `private void LEngineDiweiApply()`

Derives the categories of every language with rime books again, from the stored rows and the hypothesis.
Run when the engine binds a workspace, so an edited hypothesis file shows in the classes at the next start.
