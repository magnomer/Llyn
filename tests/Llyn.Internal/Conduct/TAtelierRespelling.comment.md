# TAtelierRespelling.cs
Hash: `a3920a39bdc64baa`

## `public sealed class TAtelierRespelling`

Covers the reflex rules the respelling helpers own, over a fake phonology port.
A reflex stands between slashes whenever its language is phonemic, and bare otherwise.
A reflex row answers ready to show, its text resolved and its fold read in the same engine call.
Each reflex row is marked as leading its run of one language.
The respelling prints only while shown and filled.

## `private static CReflexDraft TAtelierReflexCreate(long id, string language, string text, string respelling)`

Builds one draft reflex with one anchor, so a scan's copy of it shows.
