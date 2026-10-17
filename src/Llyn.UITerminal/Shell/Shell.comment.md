# Shell

The blueprints of the console's start screens.
Each file holds one `Blueprint` with a name that `BWindow` finds by contract.
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

## Stamp

Commit: `92c4426f8506`
- `ANotice.xaml` `30a72b1b0e5d1943`
- `AWindow.xaml` `987c9213804d9ba1`
