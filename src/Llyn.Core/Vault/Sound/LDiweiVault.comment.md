# LDiweiVault.cs

## `public interface LDiweiVault`

The persistence port for the Diwei rows the engine reads and writes.
It lists exactly what the engine asks of diwei storage, and nothing about how rows are kept.
`LDiweiArchive` in Infrastructure is its adapter over the workspace database.

## `void LDiweiApply(string language, string character, LHypothesis? hypothesis);`

Rebuilds the links of one character after its fanqie rows were stored.
Categories left without a link in the language are dropped afterwards.

## `void LDiweiRebuild(string language, LHypothesis? hypothesis);`

Drops every category of the language and derives them again from every stored character.
Run when the engine binds a workspace, since the tone classes depend on the hypothesis file as it now reads.

## `IReadOnlyList<LDiwei> LDiweiRead(string language, string kind);`

The categories of one kind with their entry counts, in the order they were first made.

## `LDiwei? LDiweiRead(long diweiId);`

One category by its id, with its entry count, or null when none is stored under it.

## `LDiwei? LDiweiFind(string language, string kind, string key);`

The category with this key, with its count, or `null` when no placement carries it.

## `IReadOnlyList<long> LDiweiEntryScan(string language, IReadOnlyList<long> diweiIds);`

The entries owning a reflex row anchored to one fanqie row linked to every given category at once.
So an initial and a rime together name one cell of the rime table, not two separate sets.
Empty ids give nothing.

## `IReadOnlyList<LFanqieRow> LDiweiFanqieRead(long diweiId);`

The fanqie rows linked to one category that some reflex row is anchored to, in storage order.
Each carries its character and id.
A placement nobody anchored is left out, so the page lists no character without an anchored reading.
