# PFanqieLine.cs

## `public sealed class PFanqieLine`

One placement drawn as one row of the fanqie block, its parts in shared columns.
The columns read as a 音韻地位: reading, tone class, initial, rime, 開合, division, tone and 反切.

## `private const string PFanqieLineUnrounded = "開";`

The medial column's word for an unrounded placement.

## `private const string PFanqieLineMark = "合";`

The medial column's word for a rounded placement.
It is drawn as a chip so it stands out as the site's colour did.

## `private const string PFanqieLineSuffix = "等";`

The suffix the division column carries, so 三 reads as 三等.

## `private const string PFanqieLineKey = "Display.FanqieTone";`

The localization key of the tone class pattern, `{0}` standing for the class key, such as `{0}성`.

## `public string PFanqieLineReading`

The stored reading between slashes, such as `/ngoʔ/`, or empty when the placement stored none.

## `public string PFanqieLineLabel`

The stored tone class in the interface language, such as `4성S`, or empty when the placement stored none.

## `public string PFanqieLineInitial`

The initial, such as 疑.

## `public string PFanqieLineRime`

The rime with its rime heading in brackets when the source gives one, such as `模[模]` or `模`.

## `public string PFanqieLineYunmu`

The rime category the line points to, as the diwei store keys it.
It drops the heading and the 重紐 letter, such as 寒.

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

## `internal static PFanqieLine PFanqieLineCreate(LFanqieRow row)`

Builds the columns from a stored row, the reading and tone class taken as the row stores them.
No hypothesis runs here, so drawing an entry derives nothing.
A row without initial and rime, from a book read without part patterns, keeps only its text.

## `private static string PFanqieLabelFormat(string toneClass)`

The class label: the localized pattern filled with the class key, or the bare key when the pattern is missing.
Empty with a blank class.
