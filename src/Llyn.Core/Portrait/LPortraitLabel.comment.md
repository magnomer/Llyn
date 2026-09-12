# LPortraitLabel.cs

## `public sealed record LPortraitLabel`

The localized words an export needs, resolved by the caller.
Localization lives in the shell, and the engine holds none.
Passing the words in keeps export out of the interface layer without moving translation into logic.

**Parameters**

- `LPortraitLabelUnknown` - what stands in for a field that could not be read.
- `LPortraitLabelMeaning` - the singular word, used as a card kind.
- `LPortraitLabelMeanings` - the plural word, used as a section heading.
- `LPortraitLabelCollocation` - the singular word, used as a card kind.
- `LPortraitLabelCollocations` - the plural word, used as a section heading.
- `LPortraitLabelIncoming` - the heading over the entries that link here.
- `LPortraitLabelNote` - the heading over the entry note.

## `public static LPortraitLabel LPortraitLabelDefault`

English words for a caller that has no localization at hand.
Tests take these rather than pass empty strings.
