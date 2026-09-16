# PDiweiItem.cs

## `internal sealed class PDiweiItem`

One section of a category page, heading its rows: a division on an initial's page, a place on a rime's.

## `private const string PDiweiItemKey = "Yunjing.Division";`

The localization key of a division heading.
`{0}` stands for the source division and `{1}` for its Roman numeral.

## `private const string PDiweiItemBlank = "Yunjing.DivisionNone";`

The localization key of the section for placements without a division.

## `private const string PDiweiItemPrefix = "Yunjing.Place";`

The prefix of a place heading's localization key, the place name capitalized after it, as `Yunjing.PlaceLabial`.

## `private const string PDiweiItemUnplaced = "Yunjing.PlaceNone";`

The localization key of the section for initials no place of the hypothesis lists.

## `private static readonly string[] PDiweiItemOrder = ["一", "二", "三", "四"];`

The divisions in table order, which is also the section order on an initial's page.

## `private static readonly string[] PDiweiItemRoman = ["I", "II", "III", "IV"];`

The Roman numeral of each division, for languages that name them so.

## `private readonly List<PDiweiLine> _pDiweiItemLines = [];`

The rows of the section, sorted as the line class orders them.

## `private PDiweiItem(string kind, string heading, IReadOnlyList<PTally> tallies, bool switched, bool respelled)`

Names the section from its heading by the page's kind.
Keeps its tally lines and the state its switch repeats.

## `public string PDiweiItemHeading`

The division as the source wrote it, or the place name as the pack lists it, or empty.

## `public string PDiweiItemLabel`

The heading in the interface language, such as `Division I` or `Labials`.

## `public IReadOnlyList<PDiweiLine> PDiweiItemLines`

The rows of the section.

## `public IReadOnlyList<PTally> PDiweiItemTallies { get; }`

The tally lines printed under the heading, one per borrowing language and kind, empty when none has a reading.

## `public bool PDiweiItemSwitched { get; }`

Whether the heading shows the IPA and respelling switch, true while respelling is on for the language.

## `public bool PDiweiItemRespelled { get; }`

Which half of the switch is lit and which set the tally lines print, true for respelling.

## `internal static IReadOnlyList<PDiweiItem> PDiweiItemScan(`

Groups the category's placements into sections and rows by the page's kind.
An initial's page sections by division and rows by rime and 開合.
A rime's page sections by the articulatory place of the initial and rows by initial.
Each section takes the tally of its heading, printed for the set `respelled` names, and repeats the switch state.
Divisions follow table order, places pack order, with unknown ones after and the blank one last.

## `private static int PDiweiRankRead(string kind, string heading, LHypothesis? hypothesis)`

The sorting rank of a section: its table or pack position, then unknown ones, then blank.

## `private static string PDiweiLabelFormat(string division)`

The division heading: the localized pattern filled with the division and its numeral, or the blank heading.

## `private static string PDiweiPlaceFormat(string place)`

The place heading: the localized text under the place's key, the raw name when none, or the blank heading.
