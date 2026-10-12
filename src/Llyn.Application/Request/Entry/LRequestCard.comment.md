# LRequestCard.cs
Hash: `7fc40afb2afaa6e8`

The field requests key `LRequestKey` by type and row id, so a deferred edit replaces only its own row.
Every one names its card by `LRequestCardId`, real or minted, except the addition, which has no card yet.
Addition, Removal and Shift are the three structural nouns every list uses, and plan06 reuses them.

## `public sealed record LRequestCardAddition(long LRequestDraftId, LCardKind LRequestKind, long LRequestParentId, int LRequestPosition)`

Asks for a new empty card in the list `LRequestKind` names.
`LRequestParentId` is zero for the top of that list, or the meaning card the new card nests under.
`LRequestPosition` is the zero-based place it lands in, clamped to the list.
The engine mints the card's id and answers with it in the returned draft and the bulletin.

## `public sealed record LRequestCardRemoval(long LRequestDraftId, long LRequestCardId)`

Drops one card, wherever it nests.
A removed meaning also drops every collocation link to it and its descendants.

## `public sealed record LRequestCardShift(long LRequestDraftId, long LRequestCardId, long LRequestParentId, int LRequestPosition)`

Moves one card to `LRequestPosition` under `LRequestParentId`, zero for the top of its own list.
A card keeps its kind, so a collocation never lands under a meaning.

## `public sealed record LRequestCardTitle(long LRequestDraftId, long LRequestCardId, LStateWritten LRequestValue)`

Replaces the card's Title.

## `public sealed record LRequestCardExpression(long LRequestDraftId, long LRequestCardId, LStateWritten LRequestValue)`

Replaces the card's Expression.
A meaning card refuses it, since a meaning carries no expression.

## `public sealed record LRequestCardMeaning(long LRequestDraftId, long LRequestCardId, LStateWritten LRequestValue)`

Replaces the card's Meaning, which the meaning template shows as its Definition.

## `public sealed record LRequestSenseAddition(long LRequestDraftId, long LRequestCardId, long LRequestSenseId)`

Links the collocation card to one meaning of the same draft, at the end of its links.
The first link a collocation holds is its home.

## `public sealed record LRequestSenseRemoval(long LRequestDraftId, long LRequestCardId, long LRequestSenseId)`

Drops one link of the collocation card and keeps the order of the rest.
