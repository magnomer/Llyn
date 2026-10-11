# LPressMargin.cs
Hash: `26f8f571b5c2d9a4`

## `public sealed record LPressMargin(double LPressMarginTop, double LPressMarginBottom, double LPressMarginLeft, double LPressMarginRight, string? LPressMarginName)`

The four page margins in inches, the unit the browser engine prints in.
A named preset carries its name, and a margin typed by hand carries none.
So a choice list can show which preset is chosen without comparing numbers itself.

**Parameters**

- `LPressMarginTop`: the top margin in inches.
- `LPressMarginBottom`: the bottom margin in inches.
- `LPressMarginLeft`: the left margin in inches.
- `LPressMarginRight`: the right margin in inches.
- `LPressMarginName`: the preset name, or null for a custom margin.

## `private const double LPressMarginTolerance`

How close typed values must come to a preset to count as that preset.
Converting millimetres to inches leaves tiny remainders that exact equality would reject.

## `public static LPressMargin LPressMarginDefault`

Twelve millimetres on every side, the margin the page's own print rule declares.
It is the margin a print used before the preview window existed, so output does not shift.

## `public static LPressMargin LPressMarginNone`

No margin, for a sheet that should reach as close to the edge as the printer allows.

## `public static LPressMargin LPressMarginNarrow`

A quarter inch on every side.

## `public static LPressMargin LPressMarginWide`

One inch on every side.

## `public static IReadOnlyList<LPressMargin> LPressMarginPreset`

The four presets in the order a choice list offers them.
`LPressMarginResolve` matches typed values against it, and a galley finds a preset in it by name.

## `public static LPressMargin LPressMarginResolve(double top, double bottom, double left, double right, bool metric)`

Turns four typed values into a margin, read as millimetres when `metric` is set and inches otherwise.
A negative or non-numeric value becomes zero, since the browser rejects a negative margin outright.
Values that equal a preset answer that preset, so typing its numbers selects it.
