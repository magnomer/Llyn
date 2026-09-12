# LPronunciationDraft.cs

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

## `public static LPronunciationDraft? LPronunciationDraftCreate(string ipa, string audio, string? source)`

Builds the draft from a reading and a recording alone.
It gives back null when neither was given, because an empty pronunciation is no pronunciation.

## `public bool LPronunciationDraftEmpty`

True when the draft carries nothing worth storing.
A variety label alone does not make a row worth storing.
