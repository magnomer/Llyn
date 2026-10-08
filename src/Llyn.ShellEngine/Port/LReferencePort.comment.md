# LReferencePort.cs
Hash: `516f5f4bb2486985`

## `public interface LReferencePort`

The slice of the engine a deportment sees when it lists Sources or reads their sheets and citations.
`LReferenceFacade` implements it.

## `IReadOnlyList<LCatalogReference> LEngineReferenceFind(LVista vista);`

The Sources the shelf's vista lists, with its query and order.

## `IReadOnlyList<LCatalogReference> LEngineReferenceFind();`

Every Source as the whole shelf lists it, ordered by author.

## `IReadOnlyDictionary<long, string> LEngineCitationRead(LEntryDraft shown);`

The citation line of every Source the shown entry cites, child cards included.
A Source that is gone reads as its bare id.

## `string LEngineCitationRead(LDraft? draft);`

The citation line of the Source the draft's own Example cites, and empty for no draft or no citation.

## `LColophon LEngineColophonRead(LDraft draft);`

The read sheet of a Source draft, with the usage tally of the Source it holds.
A draft that holds no Source is a caller mistake and throws.

## `LImprint LEngineImprintRead(LDraft? draft);`

The edit sheet of a Source draft, or of a blank Source with no draft.

## `IReadOnlyList<(string LReferenceKindTag, string LReferenceKindKey)> LEngineKindRead();`

The kind menu of the source editor, each option's tag and localization key, in menu order.
