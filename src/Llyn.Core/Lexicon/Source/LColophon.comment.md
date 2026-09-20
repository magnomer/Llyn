# LColophon.cs

## `public sealed record LColophon(`

The read sheet of one Source: every text the colophon page prints and every look it takes, already decided.
The page writes each value into one control and branches on nothing.
A field's text is its value, the unknown mark when it is recorded unknown, or empty when never written.
A faint field is dressed as the placeholder the edit side shows, so the two sides read alike.
A shown field has a heading to stand over, so a blank line never stands for two facts.
The title alone is never empty: a Source without one reads as untitled.

## `public static LColophon LColophonCreate(`

Composes the sheet from the Source, its credited Authors and the tally sentence the panel composed.
The texts a sheet needs localized come through `localize`, because this record knows no language file.
The kind reads inside its chip when one was recorded, an unknown kind included.
The authorship reads the credited names in order, or the unknown mark when recorded unknown with no credit.
