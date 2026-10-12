# LMarkupClerkUnion.cs
Hash: `efae95958ebf2bc7`

## `public static class LMarkupClerkUnion`

The merge mode of an import.
The parsed draft is folded into the stored one without repeating a row.
Rows are matched on normalized text, so a retyped row with the same words is one row.

## `public static LEntryDraft LMarkupUnionRead(LEntryDraft loaded, LEntryDraft parsed)`

The loaded draft with every parsed list appended and the cards re-placed.
The stored etymology stands, and the file's is taken only when the entry has none.
The stored lexical unit and freedom each stand the same way.
Transcriptions are matched on their scheme, because the save allows one row per scheme.
The stored row of a scheme wins, since a merge never removes.
A pronunciation without ipa is matched on its audio location, so a re-merged recording is one row.
With neither ipa nor audio there is nothing to match, so such a row is always appended.

## `private static IReadOnlyList<LMarkupRow> LMarkupRowAppend<LMarkupRow>(IReadOnlyList<LMarkupRow> loaded, IReadOnlyList<LMarkupRow> parsed, Func<LMarkupRow, string> key)`

Parsed rows whose key is new, or empty, appended after the loaded ones.
Keys are compared in the folded form of `LCatalogTextNormalize`.

## `private static IReadOnlyList<LMarkupRow> LMarkupRowAppend<LMarkupRow>(IReadOnlyList<LMarkupRow> loaded, IReadOnlyList<LMarkupRow> parsed, Func<LMarkupRow, string> key, Func<string, string> fold)`

The same append with the key compare chosen by the caller.
A transcription scheme is case sensitive in the save, so its key is only trimmed.

## `private static string LMarkupStateFormat(LStateValue value)`

The match key of a stated value, its folded text with whether the value is unknown.
Registers, situation titles and media locations all key on it.
Two unknown values are one row, and never match a row named by text.
A value with no text and no unknown mark keeps an empty key, so it is never matched.

## `private static string LMarkupNoteAppend(string loaded, string parsed)`

A parsed note the stored note does not already hold is appended as a new paragraph.
The parsed note is appended as written, since the save normalizes it.

## `private static bool LMarkupNoteMatch(string loaded, string parsed)`

Whether the parsed note's blocks appear in a row among the stored note's blocks.
Only whole blocks count, so a sentence inside a longer paragraph is no match.

## `private static string[] LMarkupNoteParse(string note)`

The blocks of a note, normalized as the save does, split at blank lines.
Normalizing both sides means trailing spaces and line ends never make a note look new.

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
