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
`BCanvasWidthDivide` breaks wide clusters between clusters, and a combining mark stays with its base.

## `TLoop`

Three queued items run before the batch, so the batch runs once.
The batch completes the task the run waits for, so the test ends without a timeout.

## Stamp

Commit: `66a4aa567097`
- `TCanvas.cs` `6890ce240ff799ea`
- `TCanvasWidth.cs` `e107870823fc4364`
- `TLoop.cs` `df3c6a7dbd056aa9`
