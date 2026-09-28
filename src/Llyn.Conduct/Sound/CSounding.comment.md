# CSounding.cs

## `internal static class CSounding`

The maps from an entry draft's sound rows to the shapes the drivers show.
They came up from the deportment ahead of the rest of the sounding, because the card map needs them.

## `internal static IReadOnlyList<CPronunciationDraft> CSoundingPronunciationRead(`

The pronunciations of a draft, shaped for the accent rows and the lectern.

## `internal static CPronunciationDraft CSoundingPronunciationRead(LPronunciationDraft spoken)`

One pronunciation, shaped for the accent rows and the respelling resolve.

## `internal static IReadOnlyList<CTranscriptionDraft> CSoundingTranscriptionRead(`

The transcriptions of a draft, shaped for the transcription rows and the lectern.

## `internal static IReadOnlyList<CReflexDraft> CSoundingReflexRead(IReadOnlyList<LReflexDraft> reflexes)`

The reflexes of a draft, shaped for the reflex rows and the lectern.
The tone travels as text, so no driver reads an anatomy.
