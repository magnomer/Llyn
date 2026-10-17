# Shell

The blueprints of the console's start screens.
Each file holds one `Blueprint` with a name that the window, the envoy or the box finds by contract.
Draw order and area come from the file, and the words come from the localization catalog.

## `AWindow.xaml`

The main window in four rows of 1, a weighted share, 1 and 1.
The first row is the roof, `ARoof`, which stays blank.
The share holds two equal columns, each a `double` frame in the `side` theme.
Each frame has a rule above its last inner row.
The third row is `AEstablishment`, the opening line, with a left margin of one cell.
The last row is `ALegend`, whose theme `legend` gives the roles `legend-number` and `legend-label`.
The window finds `AEstablishment` and `ALegend` by name, so renaming either breaks its construction.

## `ANotice.xaml`

The framed notice a console below 80 by 24 cells shows.
One `double` frame in the `dialog` theme holds `ANoticeText`, the notice text.
The text has a margin of two columns and one row.
The window reads the text part's size to wrap the notice and to size the frame.

## `ASParley.xaml`

The sublevel box the console draws over the window.
One `double` frame in the `dialog` theme holds a split of a weighted row and a row of 1.
The split has a margin of two columns and one row.
The first row is `ASParleyText`, with a bottom margin of one row, and the last is `ASParleyButton`.
`BSParley` finds both parts by name, so renaming either breaks its construction.
The frame names its glyph set, which the box draws and the envoy reads for the arrow.

## Stamp

Commit: `9a85a3ab82db`
- `ANotice.xaml` `30a72b1b0e5d1943`
- `ASParley.xaml` `9c08cc18227e74bb`
- `AWindow.xaml` `987c9213804d9ba1`
