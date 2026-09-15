# LEngineMarkupAppend.cs

## `public sealed partial class LEngine`

The Append merge of markup import.
A stored entry and a parsed one become one draft the update path writes, with the stored ids kept.
The import itself stands in `LEngineMarkup.cs`.

## `private static LEntryDraft LEngineMarkupAppend(LEntryDraft loaded, LEntryDraft parsed)`

Headword and language come from `loaded`, since Append never renames.
Meanings and collocations are the stored cards joined with the parsed ones, renumbered through the tree.
Pronunciations, transcriptions, reflexes, speeches, forms and inflections take each parsed row the stored ones lack.
A reflex row is known by its language, kind, text and note together.
The note is the stored note followed by the parsed one, or the stored note alone.

## Inline notes

### `private static IReadOnlyList<LEngineRow> LEngineMarkupAppend<LEngineRow>(IReadOnlyList<LEngineRow> loaded, IReadOnlyList<LEngineRow> parsed, Func<LEngineRow, string> key)`

The stored rows, then each parsed row whose key matches no earlier row after `LCatalog.LCatalogTextNormalize`.
The key is the ipa, text or name that tells one row of its kind from another.
A row with a blank key is always kept, since nothing tells it from another blank one.

### `private static string LEngineMarkupAppend(string loaded, string parsed)`

A blank parsed note or one equal to the stored note changes nothing.
A blank stored note gives way to the parsed one, and two notes are joined by one blank line.

### `private static IReadOnlyList<LCardDraft> LEngineMarkupAppend(IReadOnlyList<LCardDraft> loaded, IReadOnlyList<LCardDraft> parsed)`

The stored cards, each joined with the parsed card that names the same head.
The parsed cards that name a new head follow.
An exported file read back as Append therefore changes nothing, instead of doubling every card.

### `private static int LEngineCardFind(IReadOnlyList<LCardDraft> cards, LCardDraft card)`

The index of the card with the same head as `card`, or -1.
A card with no title, expression or definition matches nothing and is always appended.

### `private static string LEngineCardFormat(LCardDraft card)`

The head of a card, its title, expression and definition normalized and joined, or empty when all three are blank.

### `private static LCardDraft LEngineMarkupAppend(LCardDraft loaded, LCardDraft parsed)`

The stored card with each parsed row it lacks, sentences by example text, chips and media by their text.
Children are joined the same way, so a nested meaning the file adds lands under its stored parent.

### `private static IReadOnlyList<LCardDraft> LEngineMarkupPlace(IReadOnlyList<LCardDraft> cards)`

Cards numbered from 1 in list order, with each child list numbered the same way.
