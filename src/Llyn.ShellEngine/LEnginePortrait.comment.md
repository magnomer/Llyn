# LEnginePortrait.cs

## `public sealed partial class LEngine`

Assembles the likeness of one entry from what the workspace holds.
The panel reads the same records through the same engine, so both see one entry the same way.
The section builders live in `LEnginePortraitCrest.cs` and `LEnginePortraitCard.cs`.

## `internal LPortraitPage LEnginePortraitRead(long entryId, LPortraitLabel label)`

Everything the display gathers is gathered here, in the display's own order.
The reading rows go under the title: pronunciations, transcriptions, then reflexes.
A pronunciation prints its respelling in place of its IPA only where the display would, per language setting.
The speech names are the page chips, drawn in the crest as the panel draws them.
The sections follow: glyph, frequency, forms, paradigm, rime books, meanings, collocations, links here, note, character forms.
The rime rows are read once and serve both the rime band and the anchors on the reflex lines.
A section that has nothing to show is left out, as the panel collapses it.
The value ids the entry is filed under stay in the draft.
A writer that needs them can find them there.
A missing entry is an error, because a caller asked to portray one that no longer stands.
Translations, sources, incoming links, the favourite mark and the bracket choice are read defensively.
None of them is the entry itself, and none is worth refusing an export over.

## `private IReadOnlyDictionary<long, LPortraitLink> LEngineTargetScan(IReadOnlyList<long> ids)`

The linked entries by id, read in one query.
A failed read yields no links rather than no export.

## `private IReadOnlyDictionary<long, string> LEngineSourceScan()`

Every source by id, each as the byline the display cites under an example.
A failed read yields no source lines rather than no export.

## `private IReadOnlyList<LFanqieRow> LEngineFanqieScan(long entryId, string language)`

The rime rows of the entry, empty for a language without a rime book or when the read fails.

## `private LSentenceOrder LEngineFrameRead(string language)`

An unknown or unknown language falls back to the default order, exactly as the panel does.
