# LEngineUpdate.cs

## `public sealed partial class LEngine`

The two write paths that make an entry out of a draft, and the two updated marks.
Both writes are facades over the outcome clerk, which normalises, saves and starts the frequency fetch.

## `internal LEntry LEngineEntrySave(LEntryDraft draft)`

Saves the whole input form as one new entry through the outcome clerk.
A blank headword is refused by the entry clerk before anything is written.

## `internal LEntry LEngineEntryUpdate(long id, LEntryDraft draft)`

Applies `draft` to the entry `id` names through the outcome clerk and answers the stored entry.
An id no entry carries is refused before anything is written.

## `private void LEngineUpdatedSet(long entryId)`

Moves the entry's `updated_utc` to now, through the clerk.
Every seam that changes one part of an entry outside the draft path calls this.

## `private void LEngineUpdatedSet(long ownerId, bool collocation)`

The same, reached from a card rather than the entry, through the card clerk.
