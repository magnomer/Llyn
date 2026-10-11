# LPressDigest.cs
Hash: `609fddabcf38bad6`

## `public sealed record LPressDigest(LPressPrinter? LPressDigestPrinter, IReadOnlyList<LPressPaper> LPressDigestPaper, int? LPressDigestChosen, IReadOnlyList<bool> LPressDigestIncluded, int LPressDigestSheets)`

The ready facts a print session shows, each derived from the setting, the printer list and the page count.
`LPressClerk.LPressClerkRead` answers it, so the session reads facts and computes none.
It is answered fresh on each read and never kept, since every edit changes it.

**Parameters**

- `LPressDigestPrinter`: the listed printer the setting names, or null to save as PDF.
- `LPressDigestPaper`: the sheets that printer offers.
  The standard sheets stand in when saving as PDF, or when the printer reported no sheet.
- `LPressDigestChosen`: the index of the setting's sheet among them, or null when none matches.
- `LPressDigestIncluded`: one flag per drawn page, true when the range includes it, empty before a draw.
- `LPressDigestSheets`: the sheets the job uses, halved for two-sided printing and multiplied by copies.
