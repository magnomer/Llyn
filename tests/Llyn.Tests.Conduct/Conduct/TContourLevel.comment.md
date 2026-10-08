# TContourLevel.cs
Hash: `a938f302f311162a`

## `public sealed class TContourLevel`

Covers the contour map `CContour.CContourRead` over a scale each fact chooses.
The cases reach the internal map through the interface relay, with no desk at all.

## `public void ContourRead_LevelsOffScale_DropsThemAndTheTone()`

Every level the given scale lacks is dropped, while a repeated level on the scale stays.
A syllable left with no level comes back untoned.

## `public void ContourRead_EmptyScale_LeavesEachUntoned()`

An empty scale keeps every syllable's text but drops every level and every tone.

## `public void ContourRead_NoSyllables_AnswersNone()`

No syllables give no contour, and nothing throws.
