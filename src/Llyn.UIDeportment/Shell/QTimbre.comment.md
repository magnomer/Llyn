# QTimbre.cs

## `public sealed class QTimbre`

The sound facts of the entry an editor holds, and the sound-sheet reads and writes over its stored id.
The pack facts come from `LEditorLanguage`, and the waiting sections from the editor's display.
The editor builds it over itself, its display and the Conduct sound sheet, so it keeps no copy.
Its sheet reads and writes only forward to `CSounding` until their callers call the sheet directly.

## `public bool QTimbreRespelled`

Whether the held draft's pack respells, so the reading field shows a respelling.

## `private long? QTimbreEntry`

The stored entry the held draft stands on, or null for a fresh draft or an empty desk.

## `public IReadOnlyList<CFanqieGroup> QTimbreFanqieRead()`

The held entry's rime-book groups, read off the sound sheet.

## `public string QTimbreReadingRead(string headword)`

The headword's representative reading, formed by the engine from the same groups the box draws.
The editor prints it under the headword box, as the reading view prints it under the headword.

## `public bool QTimbreAnchorCheck(string headword)`

Whether the held entry's fanqie rows let a reflex anchor at all.
The rows stay in the engine, so the gate reads them itself.

## `public string QTimbreAnchorFormat(IReadOnlyList<long> anchors, string headword, string separator)`

The anchors of one reflex written as its row shows them.

## `public IReadOnlyList<CAnchorRow> QTimbreAnchorScan(IReadOnlyList<long> anchors, string reflex, string tone)`

The readings one reflex may anchor to, as the anchor menu offers them.

## `public void QTimbreFanqieRebuild()`

Fetches the rime-book rows again for the held entry, announcing the change or the refusal.

## `public void QTimbreFanqieSet(long fanqieId, int rank)`

Ranks one rime-book row of the held entry among its representative readings.

## `public IReadOnlyList<CScriptGroup> QTimbreScriptRead()`

The held entry's script rows, read off the sound sheet.

## `public IReadOnlyList<CParadigmSlot> QTimbreParadigmRead()`

The held entry's paradigm slots, read off the sound sheet.
