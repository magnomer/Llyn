# LEntryQueryVault.cs
Hash: `ee319777555535de`

## `public interface LEntryQueryVault`

The read-only query port over entries.
It answers lists, scans and counts, and never changes a row.
It stands apart from `LEntryVault` so a reader holds no write it does not need.
`LEntryQueryArchive` in Infrastructure is its adapter over the workspace database.

## `IReadOnlyList<LEntry> LEntryFind(string query);`

Finds the entries whose headword matches `query`, every entry for an empty query.

## `IReadOnlyList<LEntry> LEntryHeadwordFind(string language, string headword);`

Finds the entries of `language` whose headword equals `headword`.

## `IReadOnlyList<LEntry> LEntryHeadwordScan(string language, string text);`

Finds the entries of `language` whose headword occurs in `text`.

## `IReadOnlyList<LEntry> LEntryScan(IReadOnlyList<long> ids, string query);`

Reads the entries among `ids` whose headword matches `query`.

## `IReadOnlyDictionary<long, string> LEntryEpithetScan(IReadOnlyList<long> ids);`

Reads the epithet of every entry among `ids` that has one.

## `IReadOnlyList<LEntry> LEntryTagFind(long tagId);`

Finds the entries a card of which carries the tag.

## `IReadOnlyList<LEntry> LEntryRegisterFind(long registerId);`

Finds the entries a card of which carries the register.

## `IReadOnlyList<LEntry> LEntrySituationFind(long situationId);`

Finds the entries a card of which carries the situation.

## `IReadOnlyList<LEntry> LEntryExampleFind(long exampleId);`

Finds the entries a sentence of which cites the example.

## `IReadOnlyList<LEntry> LEntryReferenceFind(long referenceId);`

Finds the entries that cite the reference.

## `long LEntryCountRead();`

Counts every entry in the workspace.
