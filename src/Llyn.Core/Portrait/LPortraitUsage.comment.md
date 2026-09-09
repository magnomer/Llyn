# LPortraitUsage.cs

## `public sealed record LPortraitUsage`

One row of the entries that link here.

**Parameters**

- `LPortraitUsageHeadword` - the linking entry's word.
- `LPortraitUsageTitle` - the linking card's title, or the mark when it is unreadable.
- `LPortraitUsageOwner` - the localized word for the kind of card that links.
- `LPortraitUsageLanguage` - the linking entry's language.

## `public static IReadOnlyList<LPortraitUsage> LPortraitUsageCreate(IReadOnlyList<LUsage> incoming, LPortraitLabel label)`

Turns stored incoming links into the rows the display draws.
The owner word comes from the label rather than the enum.
So an export reads in the language the reader chose.
