# LReflexFacade.cs
Hash: `788abb9b304e1b64`

## `internal sealed class LReflexFacade`

The engine's facade for reflex, wrapping the clerk's anchors and rules, the fetch the clerk holds, row guises and tones.

## `public LReflexFacade(LEngine engine)`

The facade bound to its engine and the engine's gate.

## `public IReadOnlyList<LAnchorRow> LEngineAnchorScan(long entryId, IReadOnlyList<long> anchors, string language, string reflex, string tone)`

The anchor rows of one stored entry, read from its own rime-book rows.

## `public bool LEngineAnchorCheck(long entryId, string headword)`

Whether the reflex rows of `headword` may carry anchors, judged on the entry's own rime-book rows.

## `public string LEngineAnchorFormat(long entryId, IReadOnlyList<long> anchors, string headword, string separator)`

The anchored readings of one stored entry, joined with `separator`.

## `public IReadOnlyList<LAnchorRow> LEngineAnchorScan(IReadOnlyList<LFanqieRow> rows, IReadOnlyList<long> anchors, string language, string reflex, string tone)`

The stored fanqie rows marked held and estimated for the anchor dropdown.
The tone rules of the entry `language` resolve the classes of the `reflex` language and `tone`.

## `public bool LEngineAnchorCheck(IReadOnlyList<LFanqieRow> rows, string headword)`

Whether the reflex rows of `headword` may carry anchors.

## `public string LEngineAnchorFormat(IReadOnlyList<LFanqieRow> rows, IReadOnlyList<long> anchors, string headword, string separator)`

The readings of the anchored rows joined with `separator`, or empty when the rows cannot be anchored.

## `public IReadOnlyList<LReflexRule> LEngineReflexRead(string language)`

The reflex rules of a language.

## `public IReadOnlyList<LReflexGuise> LEngineGuiseRead(string language, IReadOnlyList<string> reflexes)`

How each reflex row prints, one answer per language in `reflexes`, in order.
Each row language is trimmed before the settings and the fold set are asked.
The fold set is the one the pack of the entry's `language` declares.

## `private LReflexGuise LEngineGuiseBuild(string reflex, IReadOnlyList<string> folded)`

The respelling switch, the phonemic mark and the fold of one trimmed row language.

## `public void LEngineReflexStart(long entryId)`

Starts the reflex fetch of an entry that has none.

## `public void LEngineReflexRebuild(long entryId)`

Clears and fetches the reflexes of an entry again.

## `public bool LEngineReflexCheck(long entryId)`

Whether a reflex fetch is pending for the entry.

## `public IReadOnlyList<LDescent> LEngineDescentRead(string language)`

The tone classes the pack of a language declares, or none for a blank language.

## `private IReadOnlyList<LFanqieRow> LEngineAnchorRead(long entryId)`

The entry's rime-book rows as the clerk's flat list, after starting the fetch of missing characters.
The flat list runs character by character, each character's rows in the declared book order.
Flattening the book groups would put book before character instead.
