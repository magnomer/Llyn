# Canvas

The grid of cells the window draws on and the cell width of text.
It stands apart from `Shell` because no piece here knows the program, only the console.

## `BCanvasWidth`

The width side of the canvas: how many cells a text takes on the console.
It is its own object, so the canvas holds one and stays whole in its file.

## `_bCanvasWidthAmbiguous`

Whether ambiguous characters such as `ǐ`, `★` and box drawing take two cells.
The constructor takes it.
The wardrobe builds the window's width, narrow unless the user chose wide.
The strict `-find` print builds its own narrow one.
The setting holds for the object's life, so it has one writer, the constructor.

## `BCanvasWidthResolve`

Sums the cells of each grapheme cluster `StringInfo` cuts from the text, so one cluster gives its own width.
A cluster takes the widest of its characters, so a combining mark joins its base at no cost.
A lone combining mark, enclosing mark, format or control character takes 0.
A cluster holding U+FE0F asks for emoji presentation and takes 2.
Every other character takes its cells from `BCanvasRange`.
Categories come from the runtime that cuts the clusters, so both read one Unicode version.

## `BCanvasWidthClamp`

Cuts a text to at most the given cells and pads the rest with blanks.
So the result is exactly that wide.
It stops before the cluster that would overflow, so a wide character never splits and never leaves a half.

## `BCanvasWidthShorten`

Fits a text to the given cells and keeps its end.
A text that fits is only clamped.
Otherwise the ellipsis leads and the longest tail of whole clusters that fits after it follows.
If the ellipsis alone does not fit, the result is blanks.

## `BCanvasWidthDivide`

Cuts a text into lines of at most the given cells, never splitting a grapheme cluster.
A line breaks at a space, and spaces at a line start are dropped.
Two neighbouring wide clusters break between them, unless they are Hangul, which breaks only at spaces.
Given separators, a line breaks only after a separator character, and spaces are kept.
A path then wraps after `\` or `/` and not at the spaces inside a name.
A span of 0 or less gives no lines.
A line with no break point is cut where the next cluster would overflow.
A cluster wider than the span still takes a line of its own.

## `BCanvasCell`

One cell of the grid holds its text, its width and its role.
Width 1 is a normal cell and width 2 is the head of a wide cluster.
Width 0 is the empty tail after a head.
The role names the style the cell belongs to.

## `BCanvasText`

One placed text: its row, column, span in cells, value and role.
The frame is a list of these, so a driver never writes a cell itself.

## `BCanvas`

The grid of cells the window draws on, one drawn grid and one grid the terminal shows.
The grid is as wide and high as the console, and a negative size is refused.
The drawn grid starts blank and the shown grid starts unknown, so the first sync writes every cell.
It takes the theme, which turns each cell's role into its colour at sync.

## `BCanvasFrameDraw`

Rebuilds the drawn grid from blanks and the placed texts, in order, so a later text covers an earlier one.
A text is clipped to its span and the row, and a row or column off the grid is skipped.
A cluster of zero cells is skipped.
A wide cluster that does not fit takes a blank cell and ends its text.
Writing over half of a wide cluster blanks the other half, so no half character stays.

## `BCanvasSync`

Prints only the runs that differ from the shown grid, each after a cursor move to its row and column.
A run grows to cover a wide tail and any changed cell after it.
A change in a tail starts the run at its head.
A sync with nothing changed prints nothing.
The drawn grid becomes the shown one afterwards.
Each cell's colour comes from the theme by its role, and an escape is written only when it changes.
A wide tail writes nothing, since its head already covers it.
A frame that printed anything ends with a reset, so the terminal keeps no colour after it.

## `BCanvasRange`

The cell-width table, written by `scripts/UnicodeWidth.ps1` and never edited by hand.
Its Unicode version is pinned to the runtime's, 16.0.0 for .NET 10.
`BCanvasRangeFirst` partitions every code point into runs, each starting where its class changes.
`BCanvasRangeNarrow` and `BCanvasRangeWide` give each run's cells under each ambiguous setting.
The table holds the answer for both settings, so no class code needs decoding.
Wide and fullwidth characters and emoji presentation take 2, ambiguous ones 1 or 2, and the rest 1.
Reserved code points in the wide blocks take 2, as the Unicode data lists them.

## Stamp

Commit: `78fc0ebfff7c`
- `BCanvas.cs` `0f0b8d99716ec206`
- `BCanvasCell.cs` `cb0d077c14c60990`
- `BCanvasRange.cs` `07ec2bb6aa90b979`
- `BCanvasText.cs` `025bd90e380a2df3`
- `BCanvasWidth.cs` `5993d42a18ffa88e`
