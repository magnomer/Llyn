# LPortraitReading.cs

## `public static class LPortraitReading`

Turns the reading rows of a draft into the labelled lines shown under the title.
A pronunciation shows its variety as the label and one reading as the text, bare.
The display's brackets travel as the line's open and close marks, so a writer draws them in its own style.
The reading is the respelling when the language is set to respell and the row has one, else the IPA.
That is the rule the display panel follows, so a page never prints one sound twice.
A phonemic language takes slashes, and any other takes square brackets, as the panel does.
A transcription shows its scheme as the label and its spelled reading as the text.
A reflex follows the same respelling rule under its own language, as the reflex rows of the panel do.
The label is empty when the row needs none, which a single unlabeled pronunciation never does.

## `public static IReadOnlyList<LPortraitLine> LPortraitReadingCreate(IReadOnlyList<LPronunciationDraft> pronunciations, bool respelled, bool phonemic)`

The pronunciations of a draft as lines, in draft order, skipping a row whose reading is blank.
`respelled` says whether the respelling stands in for the IPA, `phonemic` which brackets close it.

## `public static IReadOnlyList<LPortraitLine> LPortraitReadingCreate(IReadOnlyList<LTranscriptionDraft> transcriptions)`

The transcriptions of a draft as lines, in draft order, skipping a row whose text is blank.

## `public static IReadOnlyList<LPortraitLine> LPortraitReadingCreate(IReadOnlyList<LReflexDraft> reflexes, Func<string, bool> respelled, Func<string, bool> phonemic, IReadOnlyList<LFanqieRow> fanqie)`

The reflexes of a draft as lines, in draft order, skipping a row whose reading is blank.
Each reflex belongs to its own language, so the two questions are asked per language.
A phonemic reflex takes slashes around the reading alone and any other stands bare, as the reflex rows do.
The label is the language, the kind, the region and the main mark, so a page reads `Japanese Kan-on`.
The romanization, meaning, note and anchored rime rows follow the reading, as the panel's reflex row prints them.

## `public static string LPortraitAnchorRead(IReadOnlyList<LFanqieRow> fanqie, IReadOnlyList<long> anchors)`

The rime rows a reflex is tied to, each as its slashed reading or its book, joined by a dot.
Empty when the reflex anchors nothing.

## `private static void LPortraitReadingAdd(List<string> parts, string text)`

Adds a part, skipping an empty one.

## `private static string LPortraitReadingRead(string phonetic, string respelling, bool respelled)`

The one reading a row shows, trimmed.

## `private static (string, string) LPortraitMarkRead(bool phonemic, bool bracketed)`

The marks around a reading: slashes when phonemic, square brackets when a bracketed row, else none.
