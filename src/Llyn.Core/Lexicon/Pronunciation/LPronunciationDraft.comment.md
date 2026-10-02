# LPronunciationDraft.cs
Hash: `183f22249d504061`

## `public sealed record LPronunciationDraft(`

One pronunciation of an entry as one value the input form carries.

An entry holds an ordered list of these, the first being the primary one.
It carries the recording beside the reading because both belong to the same stored pronunciation.
The variety label is kept even when the reading is blank, so a row being filled keeps its name.

**Parameters**

- `LPronunciationDraftIpa` — Whole-word phonetic transcription as typed or filled by a lookup.
- `LPronunciationDraftSyllables` — Syllables in the order they are spoken.
- `LPronunciationDraftAudio` — Full path to the recording, or empty when none was chosen.
  The shell deals in full paths and the engine stores the path relative to the workspace.
- `LPronunciationDraftSource` — Label of the source the recording came from, when there is one.
- `LPronunciationDraftId` — Id of the stored pronunciation row, empty when none stands yet.
- `LPronunciationDraftVariety` — Label telling this pronunciation from the entry's others, empty when unneeded.
- `LPronunciationDraftSeeded` — True when the engine made the row itself to hold a first edit on an entry with none.
  A seeded row still empty at save is dropped, while a row the user added is kept.
- `LPronunciationDraftRespelling` — The reading recast through the pack's respelling groups, empty when the pack has none.
  It is stored beside the original so the respelling switch only picks which of the two is shown.
  The engine derives it whenever the reading or its variety changes, and the user may overwrite it by hand.

## `public bool LPronunciationDraftNotated`

Whether the draft carries an IPA reading.
A respelling or a recording alone does not count, unlike `LPronunciationDraftEmpty`.

## `public string LPronunciationDraftRead(bool respelled)`

The reading as the field shows it.
It is the respelling when the switch is on and one exists, else the original.

## `public bool LPronunciationDraftEmpty`

True when the draft carries nothing worth storing.
A respelling alone counts, so a hand-written respelling is not dropped.
A variety label alone does not make a row worth storing.
