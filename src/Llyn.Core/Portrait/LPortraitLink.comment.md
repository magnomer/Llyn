# LPortraitLink.cs

## `public sealed record LPortraitLink`

One linked entry on a section, such as a translation chip on a card.

**Parameters**

- `LPortraitLinkId` - the linked entry's id, kept so a writer may address it.
- `LPortraitLinkHeadword` - the linked entry's word, shown on the chip.
- `LPortraitLinkLanguage` - the linked entry's language, shown after the word.

## `public static void LPortraitLinkRead(IReadOnlyList<LCardDraft> cards, List<long> ids)`

Collects every entry id the given cards name, without repeats.
The ids come from translations and from the mentions of each example.
Child cards are walked too, so a nested example is found.
One collected list lets the engine read all targets in a single query.

## `private static void LPortraitLinkAdd(List<long> ids, long id)`

Adds an id once, and never a zero, which marks an unset link.
