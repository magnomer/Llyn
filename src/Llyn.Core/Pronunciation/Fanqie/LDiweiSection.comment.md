# LDiweiSection.cs
Hash: `2ed919a6c81057e0`

## `public sealed record LDiweiSection(string LDiweiSectionLabel, IReadOnlyList<LDiweiLine> LDiweiSectionLines, IReadOnlyList<LTallyRow> LDiweiSectionTallies, bool LDiweiSectionSwitched, bool LDiweiSectionRespelled)`

One heading of a Diwei page.
It is a division on an initial page and a place of articulation on a rime page.
It carries its label already formatted, the tally rows of the shown set, and its sorted lines.
The switch flags say whether a respelling exists for the language and which set the page shows.
A section is data once built, so the shells carry it without a projection.

**Parameters**

- `LDiweiSectionLabel`: the localized heading, or the raw heading when no text is found.
- `LDiweiSectionLines`: the lines in print order.
- `LDiweiSectionTallies`: the tally rows under this heading, each holding its marks in the shown set.
- `LDiweiSectionSwitched`: true when the language has a respelling to switch to.
- `LDiweiSectionRespelled`: true when the page shows the respelling set.

## `public static IReadOnlyList<LDiweiSection> LDiweiSectionScan(string kind, IReadOnlyList<LFanqieRow> rows, LHypothesis? hypothesis, IReadOnlyList<LTally> tallies, bool switched, bool respelled, Func<string, string?> localize)`

Groups the fanqie rows of one category under their headings and lines, sorted as the page prints them.
Headings follow the rank `LDiwei.LDiweiRankRead` gives them, the lowest first.
Unknown divisions and unplaced rimes share one rank, so the raw heading breaks the tie.
It compares by code point through `LGlyphOrder`, then ordinally as a final stable key.
The sort is stable, so the order the rows were stored in never reaches the page.
The localize seam answers null for a missing key, so the raw heading stands in.
Rows are walked by character in code point order through `LGlyphOrder`, the stored id last.
So each line lists its characters by that declared rule, never by fetch order.
Characters gather in a list the line record already holds, so the scan mutates nothing after it returns.

## `private static LDiweiLine LDiweiLineCreate(bool rime, LFanqieRow row, LHypothesis? hypothesis, List<string> characters)`

Builds the line record for the first row met under a key, over the character list the scan keeps filling.

## `private static void LDiweiCharacterAdd(List<string> characters, string character)`

Adds a placed character once, skipping blanks.
The scan feeds it rows in code point order, so appending keeps the line in that order.

## `private static IReadOnlyList<LDiweiLine> LDiweiLineSort(List<LDiweiLine> lines)`

Orders the lines of a section by Hypothesis rank, unranked last, then by label in code point order.
Labels are rime or initial characters, so `LGlyphOrder` compares them, never UTF-16 order.
An unrounded line precedes a rounded line of the same rime, as rhyme tables print open before closed mouth.
The first character of the line is the last key, and the sort is stable.
Rank, label and rounding already identify a line, so no tie is left to storage order.

## `private static IReadOnlyList<LTallyRow> LDiweiTallyScan(IReadOnlyList<LTally> tallies, string heading, bool respelled)`

The tally lines under one heading that carry marks in the shown set, each holding only those marks.

## `private static string LDiweiLabelFormat(string division, Func<string, string?> localize)`

The heading of a division.
It is the localized pattern over the numeral and its roman form, or the blank label.

## `private static string LDiweiPlaceFormat(string place, Func<string, string?> localize)`

The heading of a place of articulation.
It is its localized name, or the unplaced label.
