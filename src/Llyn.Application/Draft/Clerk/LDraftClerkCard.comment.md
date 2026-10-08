# LDraftClerkCard.cs
Hash: `cee90cc963d79cf0`

## `public static class LDraftClerkCard`

The card arithmetic of a draft: insert, remove, move, change and find, wherever a card nests.
Every list request inside a card comes through `LCardChange`, so the card check is written once.
Only a card addition mints an id, through the issuer the clerk hands in.
Every other routine is pure and works on cards already named.

## `public static LEntryDraft? LCardApply(LEntryDraft content, LRequest request, LIdentity identity)`

Routes every card request to its handler, and answers null for any other request.
The clerk then hands that request on to its other handlers.
`identity` names the card an addition makes, and no other request uses it.

## `public static LEntryDraft LCardInsert(LEntryDraft content, LRequestCardAddition request, LIdentity identity)`

Mints the id of the new card here rather than leaving it to the normalize.
The answer must name the card, and a card named at birth cannot be confused with one named later.
The clerk also asks here when a request naming card zero finds the draft without a Meaning.

## `public static LEntryDraft LCardInsert(LEntryDraft content, LCardKind kind, long parentId, int position, LCardDraft card)`

A collocation takes no parent, because only a meaning names a parent in the store.
A parent the meanings do not hold is a missing card, and is refused as one.

## `private static IReadOnlyList<LCardDraft> LCardInsert(IReadOnlyList<LCardDraft> cards, LCardDraft card, int position)`

Puts the card into one list at the place given.
A place beyond either end of the list is read as that end.

## `public static LEntryDraft LCardRemove(LEntryDraft content, long id)`

Drops the card named from the meanings, their children or the collocations, and refuses when none carries it.

## `public static LEntryDraft LCardMove(LEntryDraft content, LRequestCardShift request)`

The card is taken out of wherever it sits, then put back under the parent named.
Its kind is remembered from where it was found, so a card never changes list by moving.
A parent inside the card being moved is gone by the time it is looked for.
So the move refuses rather than loops.

## `private static IReadOnlyList<LCardDraft>? LCardRemove(IReadOnlyList<LCardDraft> cards, long id, out LCardDraft? removed)`

Takes the card out of one list or the children below it, and answers null when none carries the id.
The card taken out is handed back, so a move can put it somewhere else.

## `public static LEntryDraft LCardChange(LEntryDraft content, long id, Func<LCardDraft, LCardDraft> change)`

Applies one change to the card named, wherever it nests, and refuses when no card carries the id.

## `private static IReadOnlyList<LCardDraft>? LCardChange(IReadOnlyList<LCardDraft> cards, long id, Func<LCardDraft, LCardDraft> change)`

Applies the change to the card in one list or below it, and answers null when none carries the id.
Only the lists on the way down to the changed card are copied, so the rest keep their reference.

## `public static int LCardEndRead(LEntryDraft content, LCardKind kind)`

The place a new card of `kind` takes: after every card its list already holds.
A card the user adds goes last, whoever asks for it.

## `public static bool LCardLoneCheck(LEntryDraft content, long id)`

Whether the card is the only one in the meanings or in the collocations.
Removing it would leave the list with nothing to type into, so a user edit keeps it.
The removal request itself stays free to empty a list, as a replay or an import needs.

## `private static bool LCardLoneCheck(IReadOnlyList<LCardDraft> cards, long id)`

Whether the list holds one card only, and that card carries the id.

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
It asks the match overload for the card carrying that id.

## `public static LCardDraft? LCardFind(LEntryDraft content, Func<LCardDraft, bool> match)`

The first card `match` accepts among the meanings, their children or the collocations, or null.
Each card is tried before its children, so a parent wins over a child that also matches.
The id find and the media finds share it, so no finder forgets the child cards.

## `public static (LOwner, int)? LCardOwnerFind(LEntryDraft draft, long id)`

Which top card list of `draft` holds the card `id`, and at which place, or null when neither does.
Only the meanings and the collocations are scanned, since a child card has no owner list of its own.

## `private static LCardDraft? LCardFind(IReadOnlyList<LCardDraft> cards, Func<LCardDraft, bool> match)`

The first card `match` accepts in one list or below it, or null.

## `private static IReadOnlyList<LCardDraft> LCardPositionUpdate(List<LCardDraft> cards)`

Renumbers one list from one, which every insert, removal and move leaves to do.
A card already carrying its number is kept as it is, so an untouched card stays the same reference.
