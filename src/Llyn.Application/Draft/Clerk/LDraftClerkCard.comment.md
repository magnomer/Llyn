# LDraftClerkCard.cs

## `public static class LDraftClerkCard`

The card arithmetic of a draft: insert, remove, move, change and find, wherever a card nests.
Every list request inside a card comes through `LCardChange`, so the card check is written once.
Nothing here mints an id, so the routines are pure and the clerk hands in a card already named.

## `public static LEntryDraft LCardInsert(`

A collocation takes no parent, because only a meaning names a parent in the store.
A parent the meanings do not hold is a missing card, and is refused as one.

## `public static LEntryDraft LCardRemove(LEntryDraft content, long id)`

Drops the card named from the meanings, their children or the collocations, and refuses when none carries it.

## `public static LEntryDraft LCardMove(LEntryDraft content, LRequestCardShift request)`

The card is taken out of wherever it sits, then put back under the parent named.
Its kind is remembered from where it was found, so a card never changes list by moving.
A parent inside the card being moved is gone by the time it is looked for.
So the move refuses rather than loops.

## `public static LEntryDraft LCardChange(LEntryDraft content, long id, Func<LCardDraft, LCardDraft> change)`

Applies one change to the card named, wherever it nests, and refuses when no card carries the id.

## `public static LCardDraft? LCardFind(LEntryDraft content, long id)`

The card `id` names among the meanings, their children or the collocations, or null.

## `private static IReadOnlyList<LCardDraft> LCardPositionUpdate(List<LCardDraft> cards)`

Renumbers one list from one, which every insert, removal and move leaves to do.
A card already carrying its number is kept as it is, so an untouched card stays the same reference.
