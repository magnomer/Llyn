# TTranscription.cs

## `public sealed class TTranscription`

Covers the transcriptions an Entry keeps, one row per scheme, beside its pronunciations.
That is their order and ids across a save, the one-scheme rule the engine refuses on, and the list the language pack declares.
It also covers the draft requests that add, move, rename and drop a transcription row before a save.

## Inline notes

### `public void EntryUpdate_SwappedSchemes_KeepsEveryId()`

Two rows trading scheme names in one save would collide on the unique scheme mid-write.
The archive shelves every surviving row first, so both ids survive and only the names move.

### `public void SchemeRead_LanguagePack_ListsDeclaredSchemesOnly()`

The pack lists its schemes under `transcription`, and a pack that lists none turns the line off.
English declares none, so its list is empty rather than a default.
