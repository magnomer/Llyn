# TRelation.cs

## `public sealed class TRelation`

Covers the engine's relation seam: a Meaning's relations and a Collocation's synonyms created, read, updated, moved and deleted through `LEngine`, the lookup a caller resolves typed text with before it writes, and the refusal that meets a target which resolved to nothing.

## Inline notes

### `LEntry resolved = Assert.Single(engine.LEngineEntryFind("term"));`

The caller resolves first: typed text becomes the entry the user meant, and its id is what reaches the engine. Nothing about the text is handed over.

### `LSense chosen = Assert.Single(engine.LEngineSenseFind("term"));`

The meaning-level twin of the entry lookup: text names the Meanings it could mean, and the caller picks one of them.

### `Assert.Empty(engine.LEngineEntryFind("nothingatall"));`

Nothing in the workspace is spelled this way, so the lookup finds nothing to resolve to.

### `LRelation unresolved = new(`

An id nobody carries, both targets at once, and neither target are one failure: the caller did not hand over a resolved target, and all three are refused with the same stated reason.

### `Assert.Empty(engine.LEngineRelationRead(senseId));`

A refused write leaves no row behind: the relation is not written half-resolved.

### `Assert.NotNull(engine.LEngineEntryRead(target.LEntryId));`

The link went; what it linked to did not.

### `LSense chosen = Assert.Single(engine.LEngineSenseFind("term"));`

Re-pointing resolves the new target on the same terms the create did.

### `Assert.Equal("a summary", Assert.Single(engine.LEngineSenseFind("résumé")).LSenseDefinition);`

Folded over the whole of Unicode, the same as the entry lookup, so an accented headword is found typed in either case.

### `Assert.Equal(`

An empty query is every Meaning, ordered by the headword each one hangs from.

### `private static LEntry TRelationEntryCreate(LEngine engine, string headword, string definition)`

One saved entry with a single Meaning card and a single Collocation card, which is the shape every test here needs an origin and a target to have.

### `private static string TRelationSenseRead(TWorkspace workspace, string entryId)`

The id of the entry's only Meaning.
