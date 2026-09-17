# LRequestSentence.cs

The field requests key `LRequestKey` by type and row id, so a deferred edit replaces only its own row.
The sentence requests, one per structural change and one per field of a sentence row.
Every one names the card and the sentence by id, real or minted.
The addition alone names no sentence, because it has none yet.
The example a sentence shows is edited through the sentence request.
The form never sees an example apart from its row.

## `public sealed record LRequestSentenceAddition(long LRequestDraftId, long LRequestCardId, int LRequestPosition)`

Asks for a new sentence row at `LRequestPosition` in the card, clamped to the list.
The engine mints the row's id and gives it no example.
The example appears with the first text that arrives for the row.

## `public sealed record LRequestSentenceRemoval(long LRequestDraftId, long LRequestCardId, long LRequestSentenceId)`

Drops one sentence row.

## `public sealed record LRequestSentenceShift(`

Moves one sentence row to `LRequestPosition` inside its card.

## `public sealed record LRequestSentenceExample(`

Picks an existing example for the sentence.
The engine copies the row's content under its positive id, replacing whatever example the sentence had.

## `public sealed record LRequestSentenceText(`

Replaces the text of the sentence's example.
On a sentence with no example the first non-empty text mints one.
On an example with a positive id it edits the draft copy, and commit updates the row.

## `public sealed record LRequestSentenceParticle(`

Replaces the particle of the sentence frame.

## `public sealed record LRequestSentenceDependence(`

Replaces the dependence of the sentence frame.

## `public sealed record LRequestSentenceReference(`

Names the source the sentence's example cites, or zero to cite nothing.
On a sentence with no example a non-zero source mints one.
