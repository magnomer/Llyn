# LSheetCascade.cs
Hash: `5d9066015c0e4e88`

## `public static class LSheetCascade`

The appearance of an entry sheet, the HTML, PDF and print export of one entry.
Every value copies the view's XAML or the Deportment line that paints it.
Catalog sheets keep `LSheetStyle`.

## `public static string LSheetCascadeRead(LTheme theme)`

Writes the custom properties from `LSheetStyle.LSheetVariableFormat`, then every rule.
The icon rules follow, then `LSheetCascadePaper`.
Each embedded icon is written once per sheet, whatever the count of stars, cards and boxes.
The stars mask `star` over their colour, as `QGraspStar` draws it as an opacity mask.
The heart paints `favorite` or `favorite-marked`, the images `Theme.Favorite.Mark` swaps.
The card hinge, the box switch and the reflex fold paint `expand` with its own colour, as `QIcon` does.
The collocation row chevron paints it the same way.
Colours are always theme keys, so the sheet follows whichever theme the display shows.

## Inline notes

### `private static readonly (string, string)[] _lSheetCascadeRules =`

Each rule is a selector and its declarations, in source order so a later rule wins.

Body and frame follow `PDisplay.xaml:24-30` and `Theme.Panel.Surface`.
The ground is the surface colour, with no width cap and no centring.
The padding is the scroll viewer's margin plus its gutter, so 18 top, 10 right and 30 left.

The header is the three-column grid of `PDisplayHeader`, held to its 56 pixel minimum height.
The headword takes `Theme.Text.Headword`, the reading `Theme.Text.Reading` in the row below.
The language pill takes `Theme.Language.Chip` with its 9 by 4 inset.
An empty `::after` keeps the 12 pixel room of the hidden chevron.
The globe is a 12 pixel ring centred in the 19 by 14 flag box, as `PDisplayLanguageGlobe` sits.
The heart box is `Theme.Favorite.Mark`, 42 square with a 24 pixel icon.
The stars copy `QGraspStar`, 16 pixel stars 3 apart inside a 4 pixel slack.
Unlit stars are the accent at 55 percent, or the muted colour at 35 percent when unrated.
A half star paints its left half lit through a clipped gradient, as the view clips its fill.

The reading block copies `PThemePronunciation.xaml`, one 37 pixel row per reading line.
The lead cell carries the 11 pixel label with its 24 pixel gap, or the 19 by 14 flag alone.
Brackets and text rise 3 pixels, the view's negative top margin.
The text keeps the phonetic family, the 292 pixel cap and the ellipsis.

The speech line copies `Theme.Speech.Chip` and `Theme.Unit.Name`.
The frequency pill copies `Theme.Frequency.Chip` on the raised surface.
The tier name and lit pips take the tier colour of `QFrequencyLabel`, an unknown tier the muted one.
Dim pips are the muted colour at 22 percent.
Section margins copy `PDisplay.xaml:69,139,160,169,173`.

The reflex rows copy `Theme.Reflex.Display`, one 31 pixel row per reading.
The label and kind take `Theme.Pronunciation.Label`, right-aligned, with 24 and 6 pixel gaps.
The text takes `Theme.Reflex.Text`, raised 3 pixels and capped at 420 pixels inside its 3 pixel inset.
Romanization, meaning, note and anchor take `Theme.Reflex.Minor` with their own left gaps.
The fold copies `Theme.Reflex.Fold`, an 11 pixel muted label centred under the rows.
Its chevron turns half a turn while open, as the checked toggle does.

The paradigm box copies `Theme.Paradigm.Box`, its slot rows `Theme.Paradigm.Row`.
An empty part keeps no gap, as `QLookControl` collapses it.
A row text with a tip is muted, as `QParadigm` paints it.
The switch is `Theme.Paradigm.Switch`, pinned right above the tables, its labels `Theme.Paradigm.Fold`.
The view gives the checked fold no look of its own, so neither does the sheet.
A checked radio only hides the other table through `:checked ~`, as `QParadigmFoldRefine` toggles them.
Header, group, label and form cells copy their `Theme.Paradigm` styles, margins becoming padding.
The header rule and a close rule span every column, as `Theme.Paradigm.Close` does.
A plain rule skips the group column, as `QParadigmRuleDraw` starts it at column 1.
Marked runs take the warning colour, cut marks and tipped forms the muted one.

The fanqie and script boxes copy `Theme.Fanqie.Box` and `Theme.Script.Box`.
Their margins follow `PDisplay.xaml:171,240`.
The summary is the box head, a 12 pixel muted label beside the `Theme.Marker.Switch` chevron.
An open box turns the chevron and fills it with the soft accent, as the checked switch does.
The portrait band inside a box loses its band margin.

A card copies `Theme.Card.Shell` and its 55 pixel `Theme.Card.Header`.
A closed card rounds all corners and drops the header line, as `QCardFold` does.
Title, caption and peek keep the 62 pixel title margin.
An open card shows the caption, a closed one the peek, as `PLeaf` swaps them.
The hinge is a 26 pixel overlay at the crown's right, as the view spans it over both columns.
It is turned half a turn while the card is open, as `Theme.Card.Hinge` turns it when unchecked.

A collocation list is a bordered box, its `h3` count a muted 13 pixel head ruled off from the rows.
Each row is ruled from the one above and never splits across pages.
A collocation row's summary sets the phrase and the meaning on one baseline after an `expand` chevron.
The chevron points right while the row is closed and down while it is open.

The stamp copies `PDisplay.xaml:242-258` and `Theme.Stamp.Label` and `Theme.Stamp.Time`.

The chip, quote, plate, line, row and note rules are the catalog's, carried until those parts are restyled.
They are scoped to `section.band` or `div.note` where the entry markup reuses a class name.

Where WPF has no exact counterpart, the closest CSS stands in.
- WPF's natural line spacing becomes `line-height:normal` in the header, reading, speech and frequency blocks.
- Brush opacity becomes `color-mix` with transparent.
- A negative top margin on a centred element becomes `position:relative;top:-3px`.
- `Stretch="Uniform"` becomes `background-size:contain`.
- Table columns stand in for shared size groups.
  So the reflex and reading columns no longer align across the two tables.
- A table cell honours no reliable maximum width, so the reflex caps are best effort.
- The reflex fold sits above the folded rows, since a summary leads its `details`.
  Its label stays `Reflex.More` while open, as the sheet writes no `Reflex.Less`.
- The reflex block is as wide as its widest table, so the fold centres under that width.
- `Theme.Paradigm.Rule` draws a dash of 1 on and 2 off, so a 3 pixel gradient repeats it.
- The box label's 4 pixel bottom margin becomes summary padding, offset by the switch's negative margin.
- Margins on grid cells become padding on table cells.

### `private const string LSheetCascadePaper =`

The `@page` rule of `LSheetStyle`, written after every other rule.
Print runs through `LPressBrowser`, a hidden WebView2 page, and prints with backgrounds.
So print keeps the screen look, colours exact, cards and the switch as the file opens.
Print drops only the frame padding, since the 12 millimetre page margin replaces it.
The paper is the surface colour, white in the default theme.
