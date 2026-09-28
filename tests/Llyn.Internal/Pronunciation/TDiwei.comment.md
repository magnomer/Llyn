# TDiwei.cs

## `public sealed class TDiwei`

The category's own rules, apart from any store.

## `public void DiweiRankRead_RimeDivisionOrder()`

Divisions rank in table order, an unlisted division after them, and a blank heading last.
Places rank as the hypothesis lists them, an unlisted place after them, and a blank heading last.
With no hypothesis every place is unlisted, and a negative line rank normalizes to last.

## `public void DiweiSectionScan_TallyUnderHeading_KeepsOnlyTheShownSet(bool respelled, string text, string character)`

A section's tally rows carry only the marks of the set the page shows.
A line with no mark in the respelled set is left out of a respelled section.
