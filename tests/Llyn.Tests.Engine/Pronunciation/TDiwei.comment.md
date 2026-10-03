# TDiwei.cs
Hash: `570f2bb09479f5ab`

## `public sealed class TDiwei`

The category's own rules, apart from any store.

## `public void DiweiRankRead_RimeDivisionOrder()`

Divisions rank in table order, an unlisted division after them, and a blank heading last.
Places rank as the hypothesis lists them, an unlisted place after them, and a blank heading last.
With no hypothesis every place is unlisted, and a negative line rank normalizes to last.

## `public void DiweiSectionScan_TallyUnderHeading_KeepsOnlyTheShownSet(bool respelled, string text, string character)`

A section's tally rows carry only the marks of the set the page shows.
A line with no mark in the respelled set is left out of a respelled section.

## `public void DiweiSectionScan_DivisionHeading_LabelsItByTheLocalizedPattern(string? pattern, string label)`

A division section is headed by the localized pattern, filled with the division and its Roman numeral.
Without a pattern the raw division heads the section.
The localizer is handed in, so the label never depends on the catalog another test loaded.
