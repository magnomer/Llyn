# LDiweiSection.cs

## `public sealed record LDiweiSection(`

One heading of a Diwei page: a division on an initial page, a place of articulation on a rime page.
It carries its label already formatted, the tally lines that have marks in the shown set, and its sorted lines.
The switch flags say whether a respelling exists for the language and which set the page shows.
A section is data once built, so the shells carry it without a projection.

**Parameters**
- `LDiweiSectionHeading`: the raw division or place name the rows were grouped under.
- `LDiweiSectionLabel`: the localized heading, or the raw heading when no text is found.
- `LDiweiSectionLines`: the lines in print order.
- `LDiweiSectionTallies`: the tally lines under this heading that carry marks in the shown set.
- `LDiweiSectionSwitched`: true when the language has a respelling to switch to.
- `LDiweiSectionRespelled`: true when the page shows the respelling set.

## `public static IReadOnlyList<LDiweiSection> LDiweiSectionScan(`

Groups the fanqie rows of one category under their headings and lines, sorted as the page prints them.
The localize seam answers null for a missing key, so the raw heading stands in.
Characters gather in a list the line record already holds, so the scan mutates nothing after it returns.

## `private static LDiweiLine LDiweiLineCreate(`

Builds the line record for the first row met under a key, over the character list the scan keeps filling.

## `private static void LDiweiCharacterAdd(List<string> characters, string character)`

Adds a placed character once, skipping blanks.

## `private static void LDiweiLineSort(List<LDiweiLine> lines)`

Orders the lines of a section by Hypothesis rank, unranked last, then by label.

## `private static IReadOnlyList<LTallyLine> LDiweiTallyScan(IReadOnlyList<LTally> tallies, string heading, bool respelled)`

The tally lines under one heading that carry marks in the shown set.

## `private static string LDiweiLabelFormat(string division, Func<string, string?> localize)`

The heading of a division: the localized pattern over the numeral and its roman form, or the blank label.

## `private static string LDiweiPlaceFormat(string place, Func<string, string?> localize)`

The heading of a place of articulation: its localized name, or the unplaced label.
