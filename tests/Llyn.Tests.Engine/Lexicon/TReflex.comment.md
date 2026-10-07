# TReflex.cs
Hash: `833e8e72d9a88107`

## `public sealed class TReflex`

Covers the reflexes an Entry keeps, the readings of its characters in the languages that borrowed them.
That is their order, ids and main marks across a save, and the stale id the engine refuses on.

Rows read back in the order the pack declares, never in the order they were stored.
A pack without an order lists them by language name.
The order rule puts declared languages and kinds first and the rest after by name.
Within one language and kind, the main row leads and the rest follow by text.
The Classical Chinese pack declares its order apart from its fetch rules.
The entry view, the editor draft, Livery, the portrait and the markup export all list one declared order.
The editor draft opened in that order counts as unchanged.

Saving the same rows in another order writes nothing and logs no reflex revision.
The stored positions stay as saved, so the declared order never rewrites them.
Two drafts holding the same rows in another order match, while a changed row does not.

A row blank in every text is dropped on save, and a row with any text is kept.
It also covers the draft requests that add, fill, mark and drop a reflex row before a save.
Only the meaning request marks the row as user-owned.
A row is found in a draft by its id alone, and an id of zero is refused.
A row with a language alone already counts as a change to the draft.
It also covers the `reflex` element of the markup.
The romanization, meaning, ownership, note, region and main mark survive a round trip.
