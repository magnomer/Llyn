# LRequestAuthor.cs

The author requests of the sources panel.
The credits of a source are an ordered list held on the draft, so they take the four structural nouns.
An author is a shared row, so a credit is added by name or picked by id, never edited here.

## `public sealed record LRequestAuthorAddition(long LRequestDraftId, string LRequestText, int LRequestPosition)`

Credits a new author with the typed name at `LRequestPosition`.
The engine mints the id, and commit creates the row before attaching it.

## `public sealed record LRequestAuthorPick(long LRequestDraftId, long LRequestAuthorId, int LRequestPosition)`

Credits an existing author at `LRequestPosition`.
An author already credited is left as it is.

## `public sealed record LRequestAuthorRemoval(long LRequestDraftId, long LRequestAuthorId)`

Drops one credit.

## `public sealed record LRequestAuthorShift(long LRequestDraftId, long LRequestAuthorId, int LRequestPosition)`

Moves one credit to `LRequestPosition`.

## `public sealed record LRequestAuthorState(long LRequestDraftId, LState LRequestState)`

Sets whether the authorship is known, unknown, or unrecorded.
Crediting an author marks it known on its own, so this is sent for the other two.
