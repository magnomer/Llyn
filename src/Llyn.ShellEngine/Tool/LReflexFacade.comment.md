# LReflexFacade.cs

## `internal sealed class LReflexFacade`

The engine's facade for reflex, wrapping the clerk's anchors, rules, rows, fetch and tones.

## `public LReflexFacade(LEngine engine)`

The facade bound to its engine and the engine's gate.

## `public bool LEngineAnchorMatch(IReadOnlyList<long> one, IReadOnlyList<long> other)`

Whether two anchor lists name the same rows.

## `public IReadOnlyList<LAnchorRow> LEngineAnchorScan(`

The stored fanqie rows marked held and estimated for the anchor dropdown.
The tone rules of the entry `language` resolve the classes of the `reflex` language and `tone`.

## `public bool LEngineAnchorCheck(IReadOnlyList<LFanqieRow> rows, string headword)`

Whether the reflex rows of `headword` may carry anchors.

## `public string LEngineAnchorFormat(`

The readings of the anchored rows joined with `separator`, or empty when the rows cannot be anchored.

## `public IReadOnlyList<LReflexRule> LEngineReflexRead(string language)`

The reflex rules of a language.

## `public void LEngineReflexStart(long entryId)`

Starts the reflex fetch of an entry that has none.

## `public void LEngineReflexRebuild(long entryId)`

Clears and fetches the reflexes of an entry again.

## `public bool LEngineReflexCheck(long entryId)`

Whether a reflex fetch is pending for the entry.

## `public IReadOnlyList<LAnatomyTone> LEngineToneRead(string language)`

The tone classes the pack of a language declares, or none for a blank language.
