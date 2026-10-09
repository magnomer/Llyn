# LInflection.cs
Hash: `9717db5220e1d767`
Hash: `899802cd6c6f9178`

## `public sealed record LInflection(long LInflectionId, long LInflectionEntryId, int LInflectionPosition, string LInflectionText, string? LInflectionLocal, long? LInflectionSpeechId, IReadOnlyList<long> LInflectionMorphology, bool LInflectionRegular = false, string? LInflectionPrediction = null, IReadOnlyList<LInflectionMark>? LInflectionMarks = null, string? LInflectionStamp = null)`

One inflected form of an entry, ordered within it.
The inflection has its own row id so its morphology links can name it directly.
It is subordinate to its `LInflectionEntryId` parent, and reordering changes `LInflectionPosition` only.
It carries the morphology values it shows, in order (`LInflectionMorphology`).
It links its part of speech by row id (`LInflectionSpeechId`), never by name.

**Parameters**

- `LInflectionId`: Row id, `0` before the row is stored.
- `LInflectionEntryId`: Parent entry id.
- `LInflectionPosition`: Order within the parent entry.
- `LInflectionText`: The inflected form.
- `LInflectionLocal`: Optional local representation.
- `LInflectionSpeechId`: Linked `speech_value` row, or `null` when unspecified.
- `LInflectionMorphology`: Ordered `morphology_value` row ids the inflection carries.
  Each value knows its feature, so the feature is not repeated here.
- `LInflectionRegular`: Whether the form is the one the paradigm's regular pattern predicts from the headword.
  It is derived when the form or its entry is stored, so the reading view never runs the pattern.
  Covered nonempty forms are regular when the book marks nothing.
  Uncovered forms fall back to the paradigm's regular patterns.
- `LInflectionPrediction`: The form the pack's rule book predicts for the cell, or `null` when no rule covers it.
  It is `null` for every pack without a rule book.
- `LInflectionMarks`: The ranges of `LInflectionText` that depart from the prediction, as [LInflectionMark](../Inflection/LInflectionMark.comment.md) records.
  An empty list means covered with nothing marked, and `null` means uncovered.
  The display reads them as stored and never runs the rule book.
- `LInflectionStamp`: The stamp of the rule book that made the analysis, including uncovered cells.
  A stamp unlike the current book's says the analysis is stale and is made again.
