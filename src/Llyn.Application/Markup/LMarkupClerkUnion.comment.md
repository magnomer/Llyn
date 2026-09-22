# LMarkupClerkUnion.cs

## `public static class LMarkupClerkUnion`

The merge mode of an import: the parsed draft folded into the stored one without repeating a row.
Rows are matched on normalized text, so a retyped row with the same words is one row.

## `public static LEntryDraft LMarkupUnionRead(LEntryDraft loaded, LEntryDraft parsed)`

The loaded draft with every parsed list appended and the cards re-placed.
The stored etymology stands, and the file's is taken only when the entry has none.

## `private static IReadOnlyList<LMarkupRow> LMarkupRowAppend<LMarkupRow>(IReadOnlyList<LMarkupRow> loaded, IReadOnlyList<LMarkupRow> parsed, Func<LMarkupRow, string> key)`

Parsed rows whose key is new, or empty, appended after the loaded ones.

## `private static string LMarkupNoteAppend(string loaded, string parsed)`

A parsed note that differs is appended as a new paragraph.

## `private static IReadOnlyList<LCardDraft> LMarkupCardAppend(IReadOnlyList<LCardDraft> loaded, IReadOnlyList<LCardDraft> parsed)`

A parsed card that matches a loaded one by title, expression and meaning merges into it, else it is appended.

## `private static int LMarkupCardFind(IReadOnlyList<LCardDraft> cards, LCardDraft card)`

The index of the card `card` matches, or minus one.
A card with no text never matches.

## `private static string LMarkupCardFormat(LCardDraft card)`

The match key of a card, empty when it has no text.

## `private static LCardDraft LMarkupCardAppend(LCardDraft loaded, LCardDraft parsed)`

Every list of the card appended, children merged in turn.

## `private static IReadOnlyList<LCardDraft> LMarkupCardPlace(IReadOnlyList<LCardDraft> cards)`

The cards renumbered from one, children in turn.
