# CPortraitLegend.cs

## `public sealed record CPortraitLegend(`

The localized words a catalog print needs, read by the driver for one realm.
Localization lives in the driver, and the panel controller turns the words into the engine's legend.

**Parameters**

- `CPortraitLegendUnknown`: what stands in for a field that could not be read.
- `CPortraitLegendUntitled`: what stands in for a missing title.
- `CPortraitLegendUnwritten`: what stands in for an example with no text.
- `CPortraitLegendUnused`: the tally of a row no card uses.
- `CPortraitLegendOnce`: the tally of a row one card uses.
- `CPortraitLegendUses`: the word after the count of a row many cards use.
- `CPortraitLegendTranslation`: the heading over the translations.
- `CPortraitLegendSource`: the heading over the cited source.
- `CPortraitLegendAuthor`: the heading over the authors.
- `CPortraitLegendYear`: the heading over the year.
- `CPortraitLegendUrl`: the heading over the web address.
- `CPortraitLegendNote`: the heading over the note.
- `CPortraitLegendDescription`: the heading over the description.
- `CPortraitLegendKind`: the word for each source kind, keyed by that kind's localization key.
