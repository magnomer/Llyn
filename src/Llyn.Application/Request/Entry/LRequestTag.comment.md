# LRequestTag.cs

The tag requests, shaped like the situation requests.
A tag carries plain text rather than a state value, because a tag is never unknown.

## `public sealed record LRequestTagAddition(`

Adds a new tag with the typed text at `LRequestPosition`.

## `public sealed record LRequestTagPick(long LRequestDraftId, long LRequestCardId, long LRequestTagId, int LRequestPosition)`

Links an existing tag to the card at `LRequestPosition`, copying the stored row under its positive id.

## `public sealed record LRequestTagRemoval(long LRequestDraftId, long LRequestCardId, long LRequestTagId)`

Unlinks one tag from the card.

## `public sealed record LRequestTagShift(long LRequestDraftId, long LRequestCardId, long LRequestTagId, int LRequestPosition)`

Moves one tag chip to `LRequestPosition` inside its card.

## `public sealed record LRequestTagText(long LRequestDraftId, long LRequestTagId, string LRequestText)`

Replaces the text of the tag, wherever the draft holds it.
