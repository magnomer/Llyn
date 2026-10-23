# Kit

Tests of the console framework pieces that know no program, only the console.

## `TCanvasWidth`

Holds one sample per width class, as the plan's width tests list them.
`水` takes 2, `a` 1, a lone combining mark 0 and an emoji 2.
`ǐ` takes 1 under the narrow setting and 2 under the wide one.
A mixed text sums its clusters, so a combining mark joins its base and U+FE0F asks for 2.
Three divide samples pin Hangul breaking at spaces, Han and kana between characters, and long Hangul between clusters.
A fourth pins a path breaking after `\` and `/` when separators are given, with its inner space kept.

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
A fault in a queued item goes to the handler the run was given, and not to the loop's own.

## `TBlueprint`

Loads the real `ABootstrap` and expects `AWindow`, `ANotice` and `ASParley` with the glyph sets `double`, `single` and `ascii`.
A blueprint embedded but not listed is skipped, as `TBlueprintFake` shows.
A `BWindow` is built with a loop, an envoy and a theme loader to prove every contract name is found.
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
A broken theme at start falls back to the spare palette and shows the reason in a box.
The box shows the fixed sentence and the reason, and nothing reaches stderr.
The fallback frame still carries the `console-blue` colour, and the run exits 0.

## `TField`

Typing three Hangul syllables into a field of 6 cells shows the tail `나다` padded with blanks.
The field keeps the whole text and reports it to the callback.
The field draws with a width measure the test passes in.

## Stamp

Commit: `c1396871c007`
- `TBlueprint.cs` `efb5286b148096f1`
- `TBlueprintFake.cs` `000e50a0d9cf0b07`
- `TBlueprintFake.xaml` `0d000834862bab6c`
- `TBlueprintListed.xaml` `5b0e022ddbcd8a05`
- `TBlueprintUnlisted.xaml` `c90b948e5b5fd904`
- `TCanvas.cs` `f15ccf3edfc35e06`
- `TCanvasWidth.cs` `70f3ebae89174d36`
- `TField.cs` `d60f7d6d52c31c2d`
- `TLoop.cs` `d33aa16620dbc226`
- `TTheme.cs` `a65066d14995ec46`
