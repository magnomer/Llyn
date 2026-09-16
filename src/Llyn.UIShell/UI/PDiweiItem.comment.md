# PDiweiItem.cs

## `internal sealed class PDiweiItem`

One division section of a category page, heading its rows.

## `private const string PDiweiItemKey = "Yunjing.Division";`

The localization key of the section heading.
`{0}` stands for the source division and `{1}` for its Roman numeral.

## `private const string PDiweiItemBlank = "Yunjing.DivisionNone";`

The localization key of the section for placements without a division.

## `private static readonly string[] PDiweiItemOrder = ["一", "二", "三", "四"];`

The divisions in table order, which is also the section order.

## `private static readonly string[] PDiweiItemRoman = ["I", "II", "III", "IV"];`

The Roman numeral of each division, for languages that name them so.

## `private readonly List<PDiweiLine> _pDiweiItemLines = [];`

The rows of the section, sorted by rime.

## `private PDiweiItem(string division, IReadOnlyList<PTally> tallies, bool switched, bool respelled)`

Names the section from the source division and keeps its tally lines and the state its switch repeats.

## `public string PDiweiItemDivision`

The division as the source wrote it, or empty.

## `public string PDiweiItemLabel`

The heading in the interface language, such as `Division I`.

## `public IReadOnlyList<PDiweiLine> PDiweiItemLines`

The rows of the section.

## `public IReadOnlyList<PTally> PDiweiItemTallies { get; }`

The tally lines printed under the heading, one per borrowing language and kind, empty when none has a reading.

## `public bool PDiweiItemSwitched { get; }`

Whether the heading shows the IPA and respelling switch, true while respelling is on for the language.

## `public bool PDiweiItemRespelled { get; }`

Which half of the switch is lit and which set the tally lines print, true for respelling.

## `internal static IReadOnlyList<PDiweiItem> PDiweiItemScan(`

Groups the category's placements into sections by division and rows by rime and 開合.
Each section takes the tally of its division, printed for the set `respelled` names, and repeats the switch state.
Sections follow table order, with unknown divisions after and the blank one last.
Rows within a section are sorted by rime.

## `private static int PDiweiRankRead(string division)`

The sorting rank of a division: its table position, then unknown ones, then blank.

## `private static string PDiweiLabelFormat(string division)`

The heading: the localized pattern filled with the division and its numeral, or the blank heading.
