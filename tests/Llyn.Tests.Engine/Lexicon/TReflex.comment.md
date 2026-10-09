# TReflex.cs
Hash: `7c6ce6a407088f9a`

## `public sealed class TReflex`

Covers the reflexes an Entry keeps, the readings of its characters in the languages that borrowed them.
That is their order, ids and main marks across a save, and the stale id the engine refuses on.
A pack without an order lists them by language name.
The declared pack order and the reordered-rows facts live in `TReflexOrder`.
Entries are built through the `TInterface.TReflexDraftCreate` relay that `TReflexOrder` shares.

A row blank in language, kind and text is dropped on save.
A row with a language alone is kept.
It also covers the draft requests that add, fill, mark and drop a reflex row before a save.
Only the meaning request marks the row as user-owned.
A row is found in a draft by its id alone, and an id of zero is refused.
A row with a language alone already counts as a change to the draft.
It also covers the `reflex` element of the markup.
The romanization, meaning, ownership, note, region and main mark survive a round trip.
A saved region and note read back through the entry, the store, the markup export and the portrait line.
