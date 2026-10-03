# TYunjingDiwei.cs
Hash: `1cce8956ab4a2cc1`

## `public sealed class TYunjingDiwei`

Covers the page a yunjing cell shows, on a real workspace.
It builds its panel through `TYunjing.TYunjingPrepare`.
The page of an initial cell groups its lines by division, with the characters sorted and the switch flags copied.
The section label is localized from a shared catalog, so `TDiwei` checks it with a localizer handed in.
The page carries the headword and glyph fonts of its language, and the blank page carries none.
Under the bundled book language, a Korean reading under the cell reaches the page as one tally.
The tally switch is saved once, and a switch without a side changes nothing.
A glyph opens in the library tab only while a cell page shows, in the language of that page.
