# Kit

Tests of the console framework pieces that know no program, only the console.

## `TCanvasWidth`

Holds one sample per width class, as the plan's width tests list them.
`水` takes 2, `a` 1, a lone combining mark 0 and an emoji 2.
`ǐ` takes 1 under the narrow setting and 2 under the wide one.
A mixed text sums its clusters, so a combining mark joins its base and U+FE0F asks for 2.
Three divide samples pin Hangul breaking at spaces, Han and kana between characters, and long Hangul between clusters.

## `TCanvas`

Holds the canvas rules a user would see as broken characters.
A wide character that does not fit at the edge leaves one blank cell.
Writing over half of a wide character blanks its other half.
A second sync after one changed cell writes that run only, and a third writes nothing.
Each run is coloured by its role under the spare theme, and the frame ends with a reset.
`BCanvasWidthDivide` breaks wide clusters between clusters, and a combining mark stays with its base.

## `TLoop`

Three queued items run before the batch, so the batch runs once.
The batch completes the task the run waits for, so the test ends without a timeout.

## `TBlueprint`

Loads the real `ABootstrap` and expects `AWindow` and `ANotice` with the glyph sets `double`, `single` and `ascii`.
A blueprint embedded but not listed is skipped, as `TBlueprintFake` shows.
A `BWindow` is built to prove every contract name is found.
An ASCII frame is drawn on a 3 by 3 area to prove the glyph set picks the marks.
A glyph set missing a mark, an unknown element and an unknown attribute each throw `InvalidDataException`.
A `Frame` without `Glyphs` throws `InvalidDataException` with the message `a Frame takes Glyphs`.

## `TBlueprintFake`

A shell over the three test files, so the loader is tested apart from the console's own blueprints.
`TBlueprintFake.xaml` lists `TBlueprintListed.xaml`, and `TBlueprintUnlisted.xaml` stays out of the list.

## `TTheme`

Both console theme files must hold every role `BTheme` knows.
`console-blue` must equal the spare palette in `BTheme`, role by role.
An unknown theme name throws `FileNotFoundException`.
A missing role draws as `text`, and a `#RRGGBB` adds exact codes after the basic ones.
A malformed value and a missing `text` each throw.
A broken theme at start falls back to the spare palette and holds a notice with the reason.
The fallback frame still carries the `console-blue` colour, and the run exits 0.

## Stamp

Commit: `92c4426f8506`
- `TBlueprint.cs` `deeb6cb372233992`
- `TBlueprintFake.cs` `000e50a0d9cf0b07`
- `TBlueprintFake.xaml` `0d000834862bab6c`
- `TBlueprintListed.xaml` `5b0e022ddbcd8a05`
- `TBlueprintUnlisted.xaml` `c90b948e5b5fd904`
- `TCanvas.cs` `f15ccf3edfc35e06`
- `TCanvasWidth.cs` `e107870823fc4364`
- `TLoop.cs` `df3c6a7dbd056aa9`
- `TTheme.cs` `a75840ef51ec0733`
