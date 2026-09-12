# TRelation.cs

## `public sealed class TRelation`

Covers the engine's relation seam.
A Meaning's relations and a Collocation's synonyms are created, read, updated, moved and deleted.
All of it goes through `LEngine`.
It also covers the lookup a caller resolves typed text with before it writes.
It covers the refusal that meets a target which resolved to nothing.

## `public void EntryUpdate_DraftCarryingRelations_StoresAndReconcilesThem()`

A draft card carries its relations, so a commit must write them as it writes every other row the card holds.
A relation the draft names by id is rewritten in place and keeps its id, one it stopped naming goes, and the rest are renumbered.
Before this, the draft carried them, the unsaved check compared them, and the commit dropped them on the floor.

## Inline notes

### `LEntry resolved = Assert.Single(engine.LEngineEntryFind("term"));`

The caller resolves first: typed text becomes the entry the user meant, and its id is what reaches the engine.
Nothing about the text is handed over.

### `LMeaning chosen = Assert.Single(engine.LEngineMeaningFind("term"));`

The meaning-level twin of the entry lookup.
Text names the Meanings it could mean, and the caller picks one of them.

### `Assert.Empty(engine.LEngineEntryFind("nothingatall"));`

Nothing in the workspace is spelled this way, so the lookup finds nothing to resolve to.

### `LRelation unresolved = new(`

An id nobody carries, both targets at once, and neither target are one failure.
The caller did not hand over a resolved target.
All three are refused with the same stated reason.

### `Assert.Empty(engine.LEngineRelationRead(meaningId));`

A refused write leaves no row behind: the relation is not written half-resolved.

### `Assert.NotNull(engine.LEngineEntryRead(target.LEntryId));`

The link went.
What it linked to did not.

### `LMeaning chosen = Assert.Single(engine.LEngineMeaningFind("term"));`

Re-pointing resolves the new target on the same terms the create did.

### `Assert.Equal("a summary", Assert.Single(engine.LEngineMeaningFind("résumé")).LMeaningDefinition);`

Folded over the whole of Unicode, the same as the entry lookup.
So an accented headword is found typed in either case.

### `Assert.Equal(`

An empty query is every Meaning, ordered by the headword each one hangs from.

### `private static LEntry TRelationEntryCreate(LEngine engine, string headword, string definition)`

One saved entry with a single Meaning card and a single Collocation card.
That is the shape every test here needs an origin and a target to have.

### `private static string TRelationMeaningRead(TWorkspace workspace, string entryId)`

The id of the entry's only Meaning.
