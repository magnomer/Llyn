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
The first frame is `AIndex`, where the library list draws.
The second frame holds `ADisplay`, the reading area, and `AVoyage`, the history row at its foot.
The window finds `ARoof`, `AEstablishment`, `ALegend` and `AVoyage` by name, so renaming one breaks the window.
`BLibrary` finds `AIndex` and `ADisplay` by name when it draws, so renaming either breaks the Library tab.

## `ANotice.xaml`

The framed notice a console below 80 by 24 cells shows, when it is at least 44 by 8.
A smaller console gets the bare text instead.
One `double` frame in the `dialog` theme holds `ANoticeText`, the notice text.
The text has a margin of two columns and one row.
`BNotice` reads the text part's size to wrap the notice and to size the frame.

## `ASParley.xaml`

The sublevel box the console draws over the window.
One `double` frame in the `dialog` theme holds a split of a weighted row and a row of 1.
The split has a margin of two columns and one row.
The first row is `ASParleyText`, with a bottom margin of one row, and the last is `ASParleyButton`.
`BSParley` finds both parts by name, so renaming either breaks its construction.
The frame names its glyph set, which the box draws and the envoy reads for the arrow.

## `ASPathfinder.xaml`

The path prompt, the body of the sublevel box for a folder or file.
One `double` frame in the `dialog` theme holds a split of five rows.
The rows are 1, a weighted share, 1, 1 and 1.
They are `APathfinderPath`, `APathfinderList`, `APathfinderMedium`, `APathfinderAlert` and `APathfinderFolder`.
The alert row has the theme `alert` and the others `dialog`.
`BSPathfinder` finds the frame at construction and each row part when it draws.
Renaming one breaks it there.

## `ASNavigation.xaml`

The tab groups of the console, in the order the tab menu and the tab chooser list them.
One `double` frame named `ANavigation` in the `dialog` theme holds five splits of rows.
The groups hold 3, 7, 3, 2 and 2 rows, one `Text` per tab, each named `ANavigation` and the tab.
`BNavigation` reads only the groups and the part names, so no code places or draws this frame.
It maps each part name to a tab key and a catalog label.
A name it does not know throws at construction.
Renaming a part, or adding one, needs a mapping in `BNavigation`.

## `ASDrive.xaml`

The drive list shown over the path prompt.
One `double` frame in the `dialog` theme holds a split with one weighted row, `ADriveList`.
`BSDrive` finds the frame and the list by name.

## Stamp

Commit: `fe7e11daae88`
- `ANotice.xaml` `30a72b1b0e5d1943`
- `ASDrive.xaml` `543d46f27c3a4ffe`
- `ASNavigation.xaml` `140013786dd77412`
- `ASParley.xaml` `9c08cc18227e74bb`
- `ASPathfinder.xaml` `45f2634d1372f979`
- `AWindow.xaml` `4ee13fd4901c1e66`
