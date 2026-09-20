# LEngineReflex.cs

## `public sealed partial class LEngine`

The reflex facades over the reflex clerk: anchors, rules, rows, the fetch and the tones.

## `public IReadOnlyList<long> LEngineAnchorToggle(IReadOnlyList<long> anchors, long fanqieId, bool anchored)`

The anchor list with `fanqieId` added or removed.

## `public bool LEngineAnchorMatch(IReadOnlyList<long> one, IReadOnlyList<long> other)`

Whether two anchor lists name the same rows.

## `public IReadOnlyList<LReflexRule> LEngineReflexRead(string language)`

The reflex rules of a language.

## `public IReadOnlyList<LReflex> LEngineReflexRead(long entryId)`

The stored reflex rows of an entry.

## `internal IReadOnlyList<LReflex> LEngineReflexSet(long entryId, IReadOnlyList<LReflex> reflexes)`

The rows of an entry replaced, the epithet rewritten with them.

## `public void LEngineReflexStart(long entryId)`

Starts the reflex fetch of an entry that has none.

## `public void LEngineReflexRebuild(long entryId)`

Clears and fetches the reflexes of an entry again.

## `public bool LEngineReflexCheck(long entryId)`

Whether a reflex fetch is pending for the entry.

## `internal Task<IReadOnlyList<LReflexDraft>> LEngineReflexFind(string headword, string language, CancellationToken cancellation)`

The reflexes of `headword` fetched now.

## `public IReadOnlyList<LAnatomyTone> LEngineToneRead(string language)`

The tone classes the pack of a language declares, or none for a blank language.
