# LCardEquality.cs
Hash: `503148a69bd1e326`

## `public static class LCardEquality`

Whether two card lists, or the fields inside a card, say the same thing.
Every call here is static and answers by content, because records compare lists by reference.
`LDraftClerkEquality` asks it for an entry's cards.
`LSituationClerk` asks it for a Situation's own pictures and clips.
Every comparer is one overload of `LCardEqualityMatch`, told apart by the row type it compares.

## `public static bool LCardEqualityMatch(IReadOnlyList<LCardDraft> one, IReadOnlyList<LCardDraft> other)`

Whether two card lists carry the same cards in the same order.
Order counts, because the order cards are in is the order they are stored in.
Position is compared with it, so a reorder is caught by what it changed rather than by chance.

## `public static bool LCardEqualityMatch(IReadOnlyList<LVideoDraft> one, IReadOnlyList<LVideoDraft> other)`

Whether two video lists carry the same clips in the same order.
Blank rows are left out first, as they are for sentences.
A Situation compares its own clips through it.

## `public static bool LCardEqualityMatch(IReadOnlyList<LImageDraft> one, IReadOnlyList<LImageDraft> other)`

Whether two image lists carry the same pictures in the same order.
Blank rows are left out first, as they are for sentences.
A Situation compares its own pictures through it.

## `private static IReadOnlyList<LCardDraft> LCardEqualityScan(IReadOnlyList<LCardDraft> cards)`

The cards that carry something, in their original order.

## `private static bool LCardEqualityCheck(LCardDraft card)`

Whether a card holds nothing at all.
An open form always shows one such card, and offering it is not an edit.
A blank sentence, image or video row counts as nothing, since the form offers those too.

## `private static bool LCardEqualityMatch(IReadOnlyList<LSentenceDraft> one, IReadOnlyList<LSentenceDraft> other)`

Whether two row lists say the same thing in the same order.
Blank rows are left out first, so a row the form added and never filled is not a change.
The frame counts, so writing a marker or a role and nothing else is a change and is saved.
The stored row each names counts too, because a row that changed id is a different row.

## `private static bool LCardEqualityMatch(LExampleDraft? one, LExampleDraft? other)`

Whether two rows quote the same Example on the same terms.
A row quoting none matches only another quoting none.
The citation counts, so retagging a sentence is a change.
The Mentions count too, so a word linked or unlinked marks the draft dirty.

## `private static bool LCardEqualityMatch(IReadOnlyList<LSituationDraft> one, IReadOnlyList<LSituationDraft> other)`

Whether two situation lists say the same thing in the same order.
Title, description, kind and id all count, not only the title the card shows.

## `private static bool LCardEqualityMatch(IReadOnlyList<long> one, IReadOnlyList<long> other)`

Whether two translation lists name the same entries in the same order.

## `private static bool LCardEqualityMatch(IReadOnlyList<LTagDraft> one, IReadOnlyList<LTagDraft> other)`

Whether two tag lists name the same tags in the same order, by id and by text.
