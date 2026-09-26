# PFanqieLine.cs

## `public sealed class PFanqieLine`

One placement drawn as one row of the fanqie block, its parts in shared columns.
The columns read as a 音韻地位: reading, tone class, initial, rime, 重紐, 開合, division, tone and 反切.

## `public int PFanqieLineRank`

The line's place among the character's representative readings, one first, zero when unmarked.

## `public bool PFanqieLineMarked`

Whether the line is a representative reading at all, so the template fills the star.

## `public bool PFanqieLinePrimary`

Whether the line is the first representative reading, the one drawn at full strength.

## `public string PFanqieLineOrder`

The rank as the badge prints it beside the star, empty when the line is unmarked.

## `public string PFanqieLineReading`

The stored reading between slashes, such as `/ngoʔ/`, or empty when the placement stored none.

## `public string PFanqieLineLabel`

The stored tone class in the interface language, such as `4성S`, or empty when the placement stored none.

## `public string PFanqieLineInitial`

The initial, such as 疑.

## `public string PFanqieLineYunmu`

The rime category the line points to, as the diwei store keys it, drawn as the link itself.
It drops the heading and the 重紐 letter and adds the 開合 mark and the division, such as `寒W I`.
The link carries the key it opens, so what the chip reads is what the rime table shows.

## `public string PFanqieLineHeading`

The rime heading in brackets when the source gives one, such as `[桓]`, drawn after the link and outside it.

## `public string PFanqieLineKnot`

The 重紐 letter in its own bracket, such as `(重紐:X)`, or empty when the rime carries none.
It stands apart from the link because it names the placement rather than the rime category.

## `public string PFanqieLineMedial`

開 or 合, or empty when the row carries no parts at all.

## `public bool PFanqieLineRounded`

Whether the medial is 合, so the template can draw the chip.

## `public string PFanqieLineDivision`

The division with its suffix, such as 一等, or empty.

## `public string PFanqieLineTone`

The tone, such as 平.

## `public string PFanqieLineSpelling`

The 反切 the source printed, such as 五乎, or empty.

## `public string PFanqieLineText`

The placement text as fetched, shown alone when the row carries no parts, and empty otherwise.

## `internal static void PFanqieRowApply(FrameworkElement container, object item, string? _)`

Fills a line of `Theme.Fanqie.Line` from the line's values.
The representative star reads `Marked` or `Faded` from its tag, and its content is the order.
An initial or rime link with no text folds away through its empty state.
A rounded medial is drawn in the warning colour and semibold.

## `private static void PFanqieWordApply(FrameworkElement container, string name, string word)`

Sets the text of one initial or rime link.

## `private static void PFanqieTextApply(FrameworkElement container, string name, string text)`

Sets one named text of a line.

## `internal static PFanqieLine PFanqieLineCreate(LFanqieRow row)`

Copies the columns from a stored row the engine already formatted.
No hypothesis runs here, so drawing an entry derives nothing.
