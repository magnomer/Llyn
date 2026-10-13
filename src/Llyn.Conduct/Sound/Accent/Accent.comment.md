# Accent

This folder holds accent row shapes and tone contours mapped from the engine.
Contour brush keys belong here so surfaces receive drawable levels without naming Core roles.

## `CAccent`

`CAccentRead` maps one engine accent row, reading no rule.
The editor's sheet and the reading view's block share it.

## `CAccentTyped`

The answer to one typed accent row.
A driver writes its row from this answer alone, never from what the user typed.
The text is the typed text when the gate took it, else the text the draft holds.

## `CContour`, `CContourInk`

One syllable of a tone contour, ready to draw.
The engine parsed the reading, so the drawing only places what it is handed.
Conduct drops every level the scale lacks, since the drawing has no guide line for it.
A toned syllable always keeps at least one level, so its pitch line always has a point.
A toned syllable left with no level is marked untoned.

`CContourRead` takes the scale as a parameter, so a test can feed levels the engine's scale never meets.
The editor's contour and the reading view's block share it.
`CContourInkRead` maps a Core colour role to its `Theme.Contour` brush key by name.
An unknown role throws, so a new Core role cannot pass undrawn.

## Stamp

Commit: `c2f335c331fe`
- `CAccent.cs` `39170fc0b7e94529`
- `CAccentTyped.cs` `f661bedb14d5b68d`
- `CContour.cs` `92673f2280cc51c7`
- `CContourInk.cs` `3998d9c0c905ba2f`
