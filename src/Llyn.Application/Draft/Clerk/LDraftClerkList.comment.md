# LDraftClerkList.cs

## `public static class LDraftClerkList`

The list arithmetic every row kind of a draft shares.
A sentence row, a chip, a media row and a credit are all lists of items addressed by id.
So one set of insert, remove, move and change routines serves all of them, told the id to read.
The concern classes hand these routines the list and the key and add nothing but the item to build.

## `public static IReadOnlyList<LDraftItem> LDraftListAdd<LDraftItem>(`

Places a new item at the position asked for, clamped to the list.

## `public static IReadOnlyList<LDraftItem> LDraftListInsert<LDraftItem>(`

Places an existing item at the position asked for, unless the list already holds its id.
A picked row is one thing however many times it is picked, so the second pick is nothing.

## `public static IReadOnlyList<LDraftItem> LDraftListChange<LDraftItem>(`

Places the item as an insert does, then drops the former item the field stood for.
A former item the list does not hold is nothing to drop.
An item that is its own former is left where it is, so an unchanged field sends nothing new.
The insert lands at the former item's place, so the removal after it leaves the new item there.

## `public static IReadOnlyList<LDraftItem> LDraftListRemove<LDraftItem>(`

Drops the item carrying the id, and refuses when none does.

## `public static IReadOnlyList<LDraftItem> LDraftListMove<LDraftItem>(`

Takes the item carrying the id out and puts it back at the position asked for.
The position is clamped after the removal, so the last place is reachable.

## `public static IReadOnlyList<LDraftItem>? LDraftListChange<LDraftItem>(`

Applies one change to the item carrying the id, or answers null when the list does not hold it.
Null rather than a refusal.
A shared row is looked for in every card, and most cards will not hold it.

## `public static int LDraftListFind<LDraftItem>(`

The place of the item carrying the id, or minus one.
An id of zero is refused here, so no list routine can be asked about nothing.

## `public static LEntryDraft LDraftRowChange(LEntryDraft content, Func<LCardDraft, LCardDraft?> change)`

Offers every card, nested ones included, one change and keeps the ones that took it.
A shared row is edited this way, since the same id may sit in any number of cards.
No card taking the change means the draft holds no such row, and that is refused.

## `public static IReadOnlyList<LDraftItem> LDraftListNormalize<LDraftItem>(IReadOnlyList<LDraftItem> items, Func<LDraftItem, bool> blank, Func<LDraftItem, LDraftItem> name)`

Drops every item `blank` marks and passes the rest through `name`, which issues an id to a new one.
The list comes back untouched when nothing was dropped or renamed, so a save can tell by reference.
