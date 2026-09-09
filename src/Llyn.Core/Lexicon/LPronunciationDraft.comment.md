# LPronunciationDraft.cs

## `public sealed record LPronunciationDraft(`

The whole pronunciation of an entry as one value the input form carries.

An entry holds at most one pronunciation, so the draft holds at most one of these.
It carries the recording beside the reading because both belong to the same stored pronunciation.

**Parameters**

- `LPronunciationDraftIpa` — Whole-word transcription as typed or filled by a lookup.
- `LPronunciationDraftLevel` — Level of detail the entry records, or null when none was chosen.
- `LPronunciationDraftSyllables` — Syllables in the order they are spoken.
- `LPronunciationDraftRepresentations` — Representations in other writing systems, in order.
- `LPronunciationDraftAudio` — Full path to the recording, or empty when none was chosen.
  The shell deals in full paths and the engine stores the path relative to the workspace.
- `LPronunciationDraftSource` — Label of the source the recording came from, when there is one.
- `LPronunciationDraftId` — Id of the stored pronunciation row, empty when none stands yet.

## `public static LPronunciationDraft? LPronunciationDraftCreate(string ipa, string audio, string? source)`

Builds the draft from a reading and a recording alone.
It gives back null when neither was given, because an empty pronunciation is no pronunciation.

## `public bool LPronunciationDraftEmpty`

True when the draft carries nothing worth storing.
