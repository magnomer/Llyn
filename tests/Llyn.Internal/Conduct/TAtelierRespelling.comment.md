# TAtelierRespelling.cs

## `public sealed class TAtelierRespelling`

Covers every notation rule the respelling gates own, over a fake phonology port.
A pronunciation stands between slashes only when respelled and phonemic, and in square brackets otherwise.
A schemed transcription prints the phonetic bare.
A reflex stands between slashes whenever its language is phonemic, and bare otherwise.
The respelling prints only while shown and filled.
An accent row carries the resolved form, its audio and its variety's keys.

## `private static CAtelier TAtelierRespellingCreate(LEngine engine, bool respelled, bool phonemic)`

Builds the atelier whose phonology port answers the two switches for every language.
