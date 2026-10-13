# Transcription

This folder holds an entry's readings spelled in named schemes, as stored, drafted and shown.

## `LSchemeRow`

One scheme a transcription row may be switched to, marked for that row's dropdown.

## `LTranscription`

The entry's reading spelled in one named scheme, such as Jyutping or Pinyin.
It is not a pronunciation, which is IPA and says how the word sounds.
An entry owns an ordered list of these, one row per scheme, because a scheme spells one reading one way.
The scheme name is stored on the row, so a row stays readable after its language pack changes.

## `LTranscriptionDraft`

One transcription of an entry as one value the input form carries, one per scheme the form shows.
A row with a scheme but no text is a row still being filled.
It is kept in the draft, and stored empty on commit when the user asked for it.
A seeded row is the one the form offers before anything was asked for, and it is dropped while empty.

## `LTranscriptionRow`

One transcription row outside the glyph scheme, with the schemes its dropdown offers.

## `LTranscriptionSheet`

The transcription block of the held draft, answered ready by the engine.
The glyph rows are left out, since the glyph block shows them.
`LTranscriptionSheetFree` says whether a new row can still be added, since some scheme is free.

## Stamp

Commit: `c2f335c331fe`
- `LSchemeRow.cs` `a93789d3acd56059`
- `LTranscription.cs` `fb0d929274331d18`
- `LTranscriptionDraft.cs` `26688400b68e77bc`
- `LTranscriptionRow.cs` `c4e44a0a8772aae4`
- `LTranscriptionSheet.cs` `968b1aed56c83ea4`
