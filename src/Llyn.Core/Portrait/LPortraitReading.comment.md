# LPortraitReading.cs

## `public sealed record LPortraitReading(`

One reading line of the portrait: a label and the text shown beside it.
A pronunciation shows its variety as the label and its IPA as the text.
A transcription shows its scheme as the label and its spelled reading as the text.
The label is empty when the row needs none, which a single unlabeled pronunciation never does.

**Parameters**

- `LPortraitReadingLabel` - the variety or the scheme, empty when there is none.
- `LPortraitReadingText` - the IPA or the spelled reading, never empty.

## `public static IReadOnlyList<LPortraitReading> LPortraitReadingCreate(IReadOnlyList<LPronunciationDraft> pronunciations)`

The pronunciations of a draft as reading lines, in draft order, skipping a row whose IPA is blank.

## `public static IReadOnlyList<LPortraitReading> LPortraitReadingCreate(IReadOnlyList<LTranscriptionDraft> transcriptions)`

The transcriptions of a draft as reading lines, in draft order, skipping a row whose text is blank.
