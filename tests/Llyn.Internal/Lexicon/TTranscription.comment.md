# TTranscription.cs

## `public sealed class TTranscription`

Covers the transcriptions an Entry keeps, one row per scheme, beside its pronunciations.
That is their order and ids across a save, and the one-scheme rule the engine refuses on.
A row the user added is stored even blank, so it stands again the next time the entry is edited.
A seeded row, the one the form offers unasked, is no change and is dropped while blank.
It also covers the list the language pack declares.
It also covers the draft requests that add, move, rename and drop a transcription row before a save.
It also covers the lookup of one scheme through the sources its pack declares, kept literal.
A scheme without sources finishes at once with nothing found.

## Inline notes

### `public void EntryUpdate_SwappedSchemes_KeepsEveryId()`

Two rows trading scheme names in one save would collide on the unique scheme mid-write.
The archive shelves every surviving row first, so both ids survive and only the names move.

### `public void SchemeRead_LanguagePack_ListsDeclaredSchemesOnly()`

The pack lists its schemes under `transcription`, and a pack that lists none turns the line off.
English declares none, so its list is empty rather than a default.
