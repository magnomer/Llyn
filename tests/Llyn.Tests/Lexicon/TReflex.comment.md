# TReflex.cs

## `public sealed class TReflex`

Covers the reflexes an Entry keeps, the readings of its characters in the languages that borrowed them.
That is their order, ids and main marks across a save, and the stale id the engine refuses on.
A row blank in every text is dropped on save, and a row with any text is kept.
It also covers the draft requests that add, fill, mark and drop a reflex row before a save.
Only the meaning request marks the row as user-owned.
A row with a language alone already counts as a change to the draft.
It also covers the `reflex` element of the markup.
The romanization, meaning, ownership, note, region and main mark survive a round trip.
