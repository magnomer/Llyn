# LRequestCard.cs

The card requests.
Every one names the card by its draft id, real or minted, except the addition, which has no card yet.
Addition, Removal and Shift are the three structural nouns every list uses, and plan06 reuses them.

## `public sealed record LRequestCardAddition(`

Asks for a new empty card in the list `LRequestKind` names.
`LRequestParentId` is zero for the top of that list, or the meaning card the new card nests under.
`LRequestPosition` is the zero-based place it lands in, clamped to the list.
The engine mints the card's id and answers with it in the returned draft and the bulletin.

## `public sealed record LRequestCardRemoval(long LRequestDraftId, long LRequestCardId)`

Drops one card, wherever it nests.

## `public sealed record LRequestCardShift(`

Moves one card to `LRequestPosition` under `LRequestParentId`, zero for the top of its own list.
A card keeps its kind, so a collocation never lands under a meaning.

## `public sealed record LRequestCardTitle(long LRequestDraftId, long LRequestCardId, LStateValue LRequestValue)`

Replaces the card's Title.

## `public sealed record LRequestCardExpression(long LRequestDraftId, long LRequestCardId, LStateValue LRequestValue)`

Replaces the card's Expression.

## `public sealed record LRequestCardMeaning(long LRequestDraftId, long LRequestCardId, LStateValue LRequestValue)`

Replaces the card's Meaning, which the meaning template shows as its Definition.
