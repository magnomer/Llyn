# TAtelierRespelling.cs

## `public sealed class TAtelierRespelling`

Covers every notation rule the respelling gates own, over a fake phonology port.
A pronunciation's mark copies the switch and both brackets the engine answers for its language.
A schemed transcription prints the phonetic bare.
A reflex stands between slashes whenever its language is phonemic, and bare otherwise.
A reflex row answers ready to show, its text resolved and its fold read in the same engine call.
Each reflex row is marked as leading its run of one language.
The respelling prints only while shown and filled.
An accent row carries the resolved form, its audio and its variety's keys.

## `private static CReflexDraft TAtelierReflexCreate(long id, string language, string text, string respelling)`

Builds one draft reflex with one anchor and a tone, so a scan's copy of them shows.

## `private static CAtelier TAtelierRespellingCreate(LEngine engine, (bool, string, string) mark, List<string> asked)`

Builds the atelier whose phonology port answers `mark` for every language and notes each language asked.
