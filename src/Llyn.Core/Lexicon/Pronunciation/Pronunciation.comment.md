# Pronunciation

Pronunciation holds how an entry sounds and how that sound is written.
That covers IPA readings with recordings, syllables, and reflexes in neighbouring languages.
The engine answers the sheets ready, with the pack's verdicts beside the rows, so no caller asks the pack again.

## `LAccentRow`

One pronunciation of the shown entry as the reading view prints it.
The engine answers it with the reading already resolved under the respelling switch.

## `LAccentSheet`

The pronunciation block of the shown entry, answered ready by the engine.
`LAccentSheetSpoken` is true when the primary pronunciation has a reading to show.
`LAccentSheetVarieties` lists the variety labels whose flags the block draws, the primary's included.
A pronunciation without a label draws no flag, so it is left out.

## `LPronunciation`

One way the entry's reading sounds, written in IPA.
An entry owns an ordered list of these, and the first is the primary one every summary shows.
The variety says why this row stands beside the others, such as a region or a register.
A row may carry no IPA yet, because a recording is often fetched before the reading is typed.
It owns its ordered syllables.

## `LPronunciationAudio`

The recorded audio a pronunciation owns, one downloaded file per pronunciation.
It is stored as a path relative to the workspace folder, so moving or copying a workspace keeps its audio.
The owning pronunciation id is the identity, so there is at most one audio row per pronunciation.
Saving replaces whatever file the pronunciation had.

## `LPronunciationDraft`

One pronunciation of an entry as one value the input form carries, the first being the primary one.
It carries the recording beside the reading because both belong to the same stored pronunciation.
The variety label is kept even when the reading is blank, so a row being filled keeps its name.
`LPronunciationDraftNotated` is true for an IPA reading only, unlike `LPronunciationDraftEmpty`.
`LPronunciationDraftRead` gives the respelling when the switch is on and one exists, else the original.
`LPronunciationDraftEmpty` counts a respelling alone, so a hand-written respelling is not dropped.
A variety label alone does not make a row worth storing.

## `LReflex`

One reading of a Han-character entry in a neighbouring language that borrowed the character.
Korean 음훈, Mandarin IPA and Japanese on'yomi are reflexes of one Middle Chinese reading.
An entry owns an ordered list of these, grouped by language, each row one kind of reading.
It is not a Transcription, which spells the entry's own reading in one scheme.
The language and kind are stored as text, so a row stays readable after its pack changes.

## `LReflexDraft`

One reflex of an entry as one value the input form carries.
A row with a language but no text is a row still being filled.
It is kept while the draft lives.
On commit a row is stored whenever any of its seven texts is filled.
`LReflexDraftWritten` is true when the row carries a reading, and the entry view lists only written rows.
`LReflexDraftEmpty` is true when the row carries no language, kind, text, romanization, meaning, note or region.

## `LSyllable`

One syllable of a pronunciation, ordered within it.
It records how the syllable sounds and nothing of how it is written.
Identity is the parent pronunciation and the position, and reordering changes the position only.
Every field but the nucleus is optional.
An absent field is stored as NULL, which is distinct from an empty string.

## Stamp

Commit: `c2f335c331fe`
- `LAccentRow.cs` `f807d8eedeac364a`
- `LAccentSheet.cs` `e0f80fe78410f836`
- `LPronunciation.cs` `210bd223f3940858`
- `LPronunciationAudio.cs` `6c2538bc7d21c103`
- `LPronunciationDraft.cs` `183f22249d504061`
- `LReflex.cs` `feea4782d7bf1916`
- `LReflexDraft.cs` `e793b093b22f50f0`
- `LSyllable.cs` `4f983f6adeb0191f`
