# Pronunciation

This folder holds the tests of the sound records of an entry.
They cover pronunciations, transcriptions, reflexes and their order, and respelled drafts.

## `TPronunciation`

Covers the engine's stored-pronunciation seams, driven through the entry save and read through its load.
That is the ordered pronunciation rows an Entry keeps, the first being the primary one.
It is also the audio hanging from each, stored workspace-relative and handed back resolved.
It is also the single note an Entry keeps beside them.
It also covers the draft requests that edit the primary row alone or any row by id.
It also covers the variety label reaching the stored row and counting as a change on its own.

The primary field is seeded, so a reading typed and cleared again is no change and stores nothing.
A row the user added counts even blank and is stored empty, so it stands again next edit.
An addition past the primary onto an empty list lands beneath a blank primary the engine seeds.

A row keeps its id across a save.
The recording hanging from it therefore stays attached when the list is reordered.
The first row after the save is the one every summary shows.
The pronunciation dropped from the entry takes the audio row with it, and the file on disk is the workspace's.

Audio is stored relative, so a moved workspace keeps its audio.
It is handed back full, so the shell plays a path without knowing where the workspace is.
The test reads the path as it sits in the table.
Only the seam turns it into a playable path.

## `TReflex`

Covers the reflexes an Entry keeps, the readings of its characters in the languages that borrowed them.
That is their order, ids and main marks across a save, and the stale id the engine refuses on.
A pack without an order lists them by language name.
The declared pack order and the reordered-rows facts live in `TReflexOrder`.
Entries are built through the `TInterface.TReflexDraftCreate` relay that `TReflexOrder` shares.

A row blank in language, kind and text is dropped on save.
A row with a language alone is kept, and already counts as a change to the draft.
The draft requests add, fill, mark and drop a reflex row before a save.
Only the meaning request marks the row as user-owned.
A row is found in a draft by its id alone, and an id of zero is refused.

The `reflex` element of the markup round-trips the romanization, meaning, ownership, note, region and main mark.
A saved region and note read back through the entry, the store, the markup export and the portrait line.

## `TReflexOrder`

Covers the order an Entry's reflex rows read back in.
Rows read back in the order the pack declares, never in the order they were stored.
The order rule puts declared languages and kinds first and the rest after by name.
Within one language and kind, the main row leads and the rest follow by text.
The Classical Chinese pack declares its order apart from its fetch rules.
The entry view, the editor draft, Livery, the portrait and the markup export all list one declared order.
The editor draft opened in that order counts as unchanged.

Saving the same rows in another order writes nothing and logs no reflex revision.
The stored positions stay as saved, so the declared order never rewrites them.
Two drafts holding the same rows in another order match, while a changed row does not.

## `TRespellingDraft`

Covers the respelling stored beside every reading and the switch that picks which one is shown.
A reading request derives the row's respelling through the draft language's pack, whatever the switch says.
A respelling request overwrites that form alone, and the next reading or variety request derives it again.
A variety request derives again because the English groups are scoped by variety.
A language request derives every row again, blank under a pack without groups.
The two forms are stored in their own columns and read back into the draft.
A respelling-only edit counts as a change.

The check answers true only while the switch is on and the pack declares groups.
The phonemic flag is the pack's alone.
A reading stands between slashes only when respelled under a phonemic pack, and in square brackets otherwise.
Flipping the switch raises a settings bulletin so open readings redraw.

A reflex row's text derives a bare respelling through the row's own language, never wrapped in slashes.
A reflex respelling written by hand stands until the text or the language changes.
Rows saved without a respelling, pronunciation and reflex alike, have one derived and stored on the way in.
A Mandarin sandhi pair respells to its realized tone, in the row, the accent sheet and its contour labels.
With the switch off the sheet shows the whole pair, and the reading keeps it either way.
A respelling stored under older rules is derived again when the workspace opens, reading untouched.
A hand-written respelling and the reading beside it stay as they were.
A load reads the stored form back and derives nothing.

## `TTranscription`

Covers the transcriptions an Entry keeps, one row per scheme, beside its pronunciations.
That is their order and ids across a save, and the one-scheme rule the engine refuses on.
A row the user added is stored even blank, so it stands again the next time the entry is edited.
A seeded row, the one the form offers unasked, is no change and is dropped while blank.
The one-scheme rule the scheme dropdown asks ignores the row that asks.
The block's free scheme counts the glyph row too, and the add's place counts it in the whole list.
The draft requests add, fill, move and drop a transcription row before a save.
The lookup of one scheme goes through the sources its pack declares, kept literal.
A scheme without sources finishes at once with nothing found.

The pack lists its schemes under `transcription`, and the scheme read lists those alone.
English declares none, so its list is empty rather than a default.
Two rows trading scheme names in one save would collide on the unique scheme mid-write.
The archive shelves every surviving row first, so both ids survive and only the names move.

## Stamp

Commit: `aae5349dbb94`
- `TPronunciation.cs` `dd68d3d094c39615`
- `TReflex.cs` `7c6ce6a407088f9a`
- `TReflexOrder.cs` `8c81c82e194f8fb1`
- `TRespellingDraft.cs` `37a8ceeeb6b10da2`
- `TTranscription.cs` `9a7fc295dc68d7d6`
