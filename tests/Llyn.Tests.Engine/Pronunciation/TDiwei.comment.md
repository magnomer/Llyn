# TDiwei.cs
Hash: `a4edda17d7808789`

## `public sealed class TDiwei`

The category's own rules, apart from any store.

## `public void DiweiRankRead_RimeDivisionOrder()`

Divisions rank in table order, an unlisted division after them, and a blank heading last.
Places rank as the hypothesis lists them, an unlisted place after them, and a blank heading last.
With no hypothesis every place is unlisted, and a negative line rank normalizes to last.

## `public void DiweiSectionScan_TallyUnderHeading_KeepsOnlyTheShownSet(bool respelled, string text, string character)`

A section's tally rows carry only the marks of the set the page shows.
A line with no mark in the respelled set is left out of a respelled section.

## `public void DiweiSectionScan_CharactersStoredOutOfCodepointOrder_ListsLineInCodepointOrder()`

A line lists its characters in code point order, whatever order the rows arrive in.
An ideograph beyond the basic plane sorts after the compatibility block, unlike UTF-16 order.
A character met twice is listed once.

## `public void DiweiSectionScan_HeadingsTiedOnRank_ListsByHeadingCodepoint()`

Two divisions off the table share one rank, so their headings follow by code point.
A heading beyond the basic plane sorts after the compatibility block, never by UTF-16 order.
The blank heading still comes last.

## `public void DiweiSectionScan_LinesTiedOnRank_ListsByLabelCodepointThenUnroundedFirst()`

Lines of equal rank list by label in code point order, a supplementary ideograph after the compatibility block.
Two lines sharing a label list unrounded first, though the rounded row arrives first.

## `public void TallyMarkScan_CharactersStoredOutOfCodepointOrder_ListsMarkInCodepointOrder()`

A tally mark lists its characters in code point order, whatever order they were gathered in.

## `public void DiweiSectionScan_DivisionHeading_LabelsItByTheLocalizedPattern(string? pattern, string label)`

A division section is headed by the localized pattern, filled with the division and its Roman numeral.
Without a pattern the raw division heads the section.
The localizer is handed in, so the label never depends on the catalog another test loaded.
