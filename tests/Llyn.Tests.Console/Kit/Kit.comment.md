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
The first frame also hides the cursor, since no caret was named.
A `caret` text shows the cursor after the frame at its cell.
A caret that does not move prints nothing, and a moved one prints only the cursor move.
A printed frame hides the cursor first and shows it again at the caret.
A frame without a caret hides the cursor once, and a caret off the grid counts as none.
`BCanvasWidthDivide` breaks wide clusters between clusters, and a combining mark stays with its base.

## `TBlueprint`

Loads the real `ABootstrap` and expects its six blueprints in order.
They are `AWindow`, `ANotice`, `ASParley`, `ASNavigation`, `ASPathfinder` and `ASDrive`.
The glyph sets are `double`, `single` and `ascii`.
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
Clearing a field given a starting text empties the text and calls no edit callback.
Arrows, Home, End and Delete edit at the caret, and Control steps by word.
Shift grows a selection that a typed character or Delete replaces, and the selection draws in the `chosen` role.
A text longer than the width scrolls with `<` and `>` marks.
A wide character cut by an edge shows as a blank.
An empty field with a hint draws the hint in the `muted` role.
Delete at the end, Backspace at the start and the arrows of an empty field reach the owner's callbacks.
Enter and Escape are not taken.
A focused field draws a zero-width `caret` text at the cell the caret is on.
An unfocused one draws none.
Two halves of a surrogate pair typed apart insert one character and report once.

## Stamp

Commit: `c37ca1837ded`
- `TBlueprint.cs` `1429a675f738f38a`
- `TBlueprintFake.cs` `000e50a0d9cf0b07`
- `TBlueprintFake.xaml` `0d000834862bab6c`
- `TBlueprintListed.xaml` `5b0e022ddbcd8a05`
- `TBlueprintUnlisted.xaml` `c90b948e5b5fd904`
- `TCanvas.cs` `a1a559a9fb2846eb`
- `TCanvasWidth.cs` `20114b19fd1e54cc`
- `TField.cs` `fb6494e05476d8cb`
- `TTheme.cs` `a65066d14995ec46`
