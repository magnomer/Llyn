# TErrandNotationRoll.cs
Hash: `839f0a57d04fd60d`

## `public sealed class TErrandNotationRoll`

Covers the rows the notation popup lists as its search answers, on a real editor.
It builds its editors through `TErrandNotation.TErrandNotationPrepare`.
Its missing and ordering tests take their pack from `TErrandNotation.TErrandNotationPack`.
A found reading lists its text and mark as the search's settings answer them.
A reading shows a flag only when it names a variety.
A schemed search lists the phonetic bare, whatever the respelling switch shows.
A source without a reading says missing or broken, and an empty answer after a reading keeps it.
Rows keep the pack's order once each, and the end of a schemed search reads the transcription notice.
