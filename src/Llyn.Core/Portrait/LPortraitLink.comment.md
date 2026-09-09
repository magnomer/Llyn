# LPortraitLink.cs

## `public sealed record LPortraitLink`

One translation chip on a card.

**Parameters**

- `LPortraitLinkId` - the linked entry's id, kept so a writer may address it.
- `LPortraitLinkHeadword` - the linked entry's word, shown on the chip.
- `LPortraitLinkLanguage` - the linked entry's language, shown after the word.

## `public static void LPortraitLinkRead(IReadOnlyList<LCardDraft> cards, List<string> ids)`

Collects every translation id the given cards name, without repeats.
One collected list lets the engine read all targets in a single query.
