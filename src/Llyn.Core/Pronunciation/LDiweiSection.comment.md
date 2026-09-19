# LDiweiSection.cs

## `public sealed class LDiweiSection`

One heading of a Diwei page: a division on an initial page, a place of articulation on a rime page.
It carries its label already formatted, the tally lines that have marks in the shown set, and its sorted lines.
The switch flags say whether a respelling exists for the language and which set the page shows.

## `public static IReadOnlyList<LDiweiSection> LDiweiSectionScan(`

Groups the fanqie rows of one category under their headings and lines, sorted as the page prints them.
The localize seam answers null for a missing key, so the raw heading stands in.

## `private static IReadOnlyList<LTallyLine> LDiweiTallyScan(IReadOnlyList<LTally> tallies, string heading, bool respelled)`

The tally lines under one heading that carry marks in the shown set.

## `private static string LDiweiLabelFormat(string division, Func<string, string?> localize)`

The heading of a division: the localized pattern over the numeral and its roman form, or the blank label.

## `private static string LDiweiPlaceFormat(string place, Func<string, string?> localize)`

The heading of a place of articulation: its localized name, or the unplaced label.
