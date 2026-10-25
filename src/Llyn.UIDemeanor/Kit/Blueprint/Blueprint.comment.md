# Blueprint

The loader that reads a shell's blueprints, places their parts and draws their frames.
It stands apart from `Shell` because no piece here knows the program, only the console.

## `BBlueprint`

The loader of a shell's blueprints, written in XAML and embedded in the shell's assembly.
A UI assembly derives from it and calls `BBlueprintLoad` with itself, so Demeanor reads files it does not own.
It keeps the loaded blueprints by name and the glyph sets by key.
The vocabulary is closed to `Blueprint`, `Split`, `Frame`, `Text`, `Legend` and `Glyphs`, each with its own attributes.

## `BBlueprintLoad`

Reads the one embedded file named after the derived type, such as `ABootstrap.xaml`, and refuses its absence.
Its root must take an `x:Class` equal to the derived type's full name.
That file lists blueprints by `Source` and holds the glyph sets, and a blueprint not listed is never loaded.
A source is found in the assembly by its path, with `/` read as `.`.
A listed blueprint that lists blueprints or glyphs or takes an `x:Class` is refused, since only the shell's file does.
A name or a glyph key listed twice is refused.
Every `Frame` must name a glyph set that exists, or the load fails.
Every refusal throws `InvalidDataException`, so a broken blueprint stops the start before any screen.

## `BBlueprintRead`

Parses one blueprint file, with DTDs prohibited and comments and whitespace ignored.
Text inside a part, an unknown element and an unknown attribute are refused, each with its line number.
A name used twice in one file is refused.
A `Blueprint` takes exactly one of `x:Name`, `x:Class` or `Source`, and only the root takes an `x:Class`.
A `Split` takes `Rows` or `Columns`.
A `Frame` must name its `Glyphs`.
A part holds no more parts than it has room for.
  A `Blueprint` has room for one, a `Split` or `Frame` for one per track, and the rest for none.
Only a `Blueprint` may hold glyph sets or sources.
A `Glyphs` takes a key and every mark, with six frame marks and three rule marks.
The other marks include `Grasp`, `Fold`, `Media` and `Tree`, so a glyph set that omits one is refused.
A margin is one, two or four cell counts, and a part without a theme draws as `text`.

## `BBlueprintPlace`

Sets the row, column, width and height of a part and all below it from the given area.
A margin of one count is all four sides, and two counts are columns then rows.
Four counts are left, top, right and bottom.
A `Frame` takes one cell for its border and one more between tracks as a rule.
A `Split` shares its area between tracks with no rule.
A part without tracks gives its whole area to every child.
A track without a child still keeps its space.
The exception is a folded child, whose track shrinks to zero cells so its space goes to the others.
The running program sets the fold, not the file, so one blueprint serves both layouts.
An area that runs out gives zero, never a negative size.

## `BBlueprintTrack`

One size of a `Rows` or `Columns` list, either a count of cells or a weight written with `*`.
A bare `*` weighs 1, and a weight of 0 or any other text throws `InvalidDataException`.
Division gives fixed tracks their cells first, clipped to what remains.
The rest is shared by weight with a floor, and the last weighted track takes what is left over.

## `BBlueprintPart`

One parsed element with its attributes, children and the area `BBlueprintPlace` last gave it.
Only the loader sets the parsed fields, and the area is set at each placing.
`BBlueprintPartFolded` is the one field the program sets after loading, to give a part no room.
A folded part still draws nothing of its own, since its area is empty.
`BBlueprintPartRules` holds the rows of the rules a `Frame` draws between its tracks.

## `BBlueprintDraw`

Turns a placed part into the texts of its frames, then those of its children.
Only a `Frame` draws.
It writes a top border, bodies with a rule on each rule row, and a bottom border.
Its marks come from the glyph set the frame names, so one blueprint draws in double, single or ASCII lines.
A frame with no width or no height draws nothing.

## Stamp

Commit: `36e31eb71c98`
- `BBlueprint.cs` `376bfb153703ee6e`
- `BBlueprintPart.cs` `ccb55927f343148e`
- `BBlueprintTrack.cs` `a5d6633e1bd337a1`
