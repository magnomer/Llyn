# LEngineRequestList.cs

## `public sealed partial class LEngine`

The list requests inside a card, and the list arithmetic every one of them shares.
A sentence row, a chip, a media row and a credit are all lists of items addressed by id.
So one set of insert, remove, move and change routines serves all of them, told the id to read.
The per-kind files hand these routines the list and the key and add nothing but the item to build.

## `private LEntryDraft LEngineListApply(LEntryDraft content, LRequest request)`

The switch over every list kind inside a card.
A kind it does not know is a programming error, not a refusal, since no form can send one.

## `private LEntryDraft LEngineCardResolve(LEntryDraft content, Func<long, LRequest> retarget)`

Applies a request that named card zero to the first Meaning card, made when the draft has none.
A browsing panel planting a Tag, Register, Situation, Example or Source into a fresh entry asks this way.
The panel need not read the draft back to learn which card the reset made.

## `private LEntryDraft LEngineSentenceResolve(LEntryDraft content, long cardId, Func<long, LRequest> retarget)`

Applies a request that named sentence zero to the card's first sentence row, made when the card has none.
A card the draft does not hold is refused, as any card request is.

## `private static LCardDraft? LEngineCardFind(LEntryDraft content, long id)`

The card `id` names among the meanings, their children or the collocations, or null.

## `private static LCardDraft? LEngineCardFind(IReadOnlyList<LCardDraft> cards, long id)`

The card `id` names in `cards` or nested under one of them, or null.

## `private static IReadOnlyList<LEngineItem> LEngineListAdd<LEngineItem>(`

Places a new item at the position asked for, clamped to the list.

## `private static IReadOnlyList<LEngineItem> LEngineListInsert<LEngineItem>(`

Places an existing item at the position asked for, unless the list already holds its id.
A picked row is one thing however many times it is picked, so the second pick is nothing.

## `private static IReadOnlyList<LEngineItem> LEngineListChange<LEngineItem>(`

Places the item as an insert does, then drops the former item the field stood for.
A former item the list does not hold is nothing to drop.
An item that is its own former is left where it is, so an unchanged field sends nothing new.
The insert lands at the former item's place, so the removal after it leaves the new item there.

## `private static IReadOnlyList<LEngineItem> LEngineListRemove<LEngineItem>(`

Drops the item carrying the id, and refuses when none does.

## `private static IReadOnlyList<LEngineItem> LEngineListMove<LEngineItem>(`

Takes the item carrying the id out and puts it back at the position asked for.
The position is clamped after the removal, so the last place is reachable.

## `private static IReadOnlyList<LEngineItem>? LEngineListChange<LEngineItem>(`

Applies one change to the item carrying the id, or answers null when the list does not hold it.
Null rather than a refusal.
A shared row is looked for in every card, and most cards will not hold it.

## `private static int LEngineListFind<LEngineItem>(`

The place of the item carrying the id, or minus one.
An id of zero is refused here, so no list routine can be asked about nothing.

## `private static LEntryDraft LEngineRowChange(LEntryDraft content, Func<LCardDraft, LCardDraft?> change)`

Offers every card, nested ones included, one change and keeps the ones that took it.
A shared row is edited this way, since the same id may sit in any number of cards.
No card taking the change means the draft holds no such row, and that is refused.
