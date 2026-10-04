# LDraftClerkCard.cs
Hash: `e395d586324b6643`

## `public static class LDraftClerkCard`

The card arithmetic of a draft: insert, remove, move, change and find, wherever a card nests.
Every list request inside a card comes through `LCardChange`, so the card check is written once.
Nothing here mints an id, so the routines are pure and the clerk hands in a card already named.

## `public static LEntryDraft LCardInsert(LEntryDraft content, LCardKind kind, long parentId, int position, LCardDraft card)`

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

## `public static int LCardEndRead(LEntryDraft content, LCardKind kind)`

The place a new card of `kind` takes: after every card its list already holds.
A card the user adds goes last, whoever asks for it.

## `public static bool LCardLoneCheck(LEntryDraft content, long id)`

Whether the card is the only one in the meanings or in the collocations.
Removing it would leave the list with nothing to type into, so a user edit keeps it.
The removal request itself stays free to empty a list, as a replay or an import needs.

## `public static int? LCardShiftRead(LEntryDraft content, long id, int place)`

The place a card moves to in its own list, counting from zero, or null when nothing moves.
A card in neither list, a place outside its list, or the place it holds already answers null.
So a list of one never moves, and the caller sends nothing.

## `private static IReadOnlyList<LCardDraft> LCardListRead(LEntryDraft content, long id)`

The list that holds the card: the meanings when they carry it, and the collocations otherwise.

## `private static int LCardPlaceRead(IReadOnlyList<LCardDraft> cards, long id)`

The index of the card in one list, or minus one when the list does not hold it.

## `public static LCardDraft? LCardFind(LEntryDraft content, long id)`

The card `id` names among the meanings, their children or the collocations, or null.

## `private static IReadOnlyList<LCardDraft> LCardPositionUpdate(List<LCardDraft> cards)`

Renumbers one list from one, which every insert, removal and move leaves to do.
A card already carrying its number is kept as it is, so an untouched card stays the same reference.
