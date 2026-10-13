# Settings

The settings panel is a catalog of setting groups beside the page of the chosen group.
It stands apart from the browse panels.
Its catalog has no ordering, and its right side holds controls, not a reader.
Its markup carries no hook.
The Deportment class `QSettings` and its choice drivers set icons, clicks, handlers, the language items and the row fills.
It also names the language box's value path, since that path is read by reflection.

## `PSettings`

The shell behind `PSettings.xaml`, placed by the main window.
Its constructor only calls `InitializeComponent()`.

## Seams

The seams part the setting catalog from the page of the chosen group.
The row seam runs under the search field and is bled past the panel margin, as every tab does it.
The column seam sits in the gutter and reaches the foot of the window.

## `PWinnow`

The search field over the catalog, a bare field with no ordering or filter button.
Eight groups need no ordering, and the text alone decides which rows stay.

## `PLedger`

The catalog of setting groups, one row each, the title beside a summary of the values the group holds.
Choosing a row swaps the page on the right and nothing else.
A row uses `Theme.Catalog.Row`, so the tab reads as one of the catalogs.

## Edit area

One page per group is drawn in the same cell, and only the chosen one is visible.
A page opens with its heading and purpose, then its rows parted by hairlines.
Each row reads its label over its helper, with the control at the far end.
No box is drawn around a page, because the surface it stands on is already the region.
A page and its rules run the whole width of the surface, as a seam does.
Each row caps its label column instead, so the control stands close after the text in a wide window.
The filler column past the control takes the rest, which is why the rule can outrun the row.

## `PWorkspaceDialog` and `PDialFolder`

`PWorkspaceDialog` is the folder picker beside the path field, an icon action whose label survives as its tooltip.
`PDialFolder` opens the workspace folder and stands at the foot of the workspace page.
It stands with the setting it acts on rather than in a command rail over every page.
The negative margin lines its label up with the rows above, since the button pads its own text.

## `PLocalization`

The interface language box, empty in markup.
The Deportment class fills one item per embedded catalog, so the markup names no language.

## `PRespelling`

Whether readings are shown and edited in the pack's respelling convention instead of as they came.
Both forms are stored, so a flip redraws every open reading through a settings bulletin and refetches nothing.
The switch keeps the check box's handler and its checked reading, and only the drawing changed.

## `PSettingsEpithet`

Whether every list prints an entry's epithet after its headword.
The epithet is the reading the language pack names, so a Han character lists as `弄 [희롱할 롱]`.

## `PSettingsNotebook`

The title of the top Joplin notebook a sync writes, on the Sync page.
It is a bare field the width of the language box, saved on Enter or when it loses focus.
The page is named `PDialOutpost`, the engine's name for the Joplin side, because `Sync` is a verb.

## `PSettingsAnalysis`

Whether the inflection box uses the pack's custom sheets.
With it on, letters that differ from the predicted form are marked.
Every form is analysed when it is stored, so a flip only redraws the box.

## `PFrequency` and `PMorphology`

`PFrequency` decides whether an entry's frequency is fetched from the pack's web source.
`PMorphology` decides whether a lookup fetches the headword's inflected forms from the web.
They stand together in one card, since both govern what a lookup fetches.
A pack that declares no frequency source fetches nothing either way, so that switch costs nothing there.
A click on the morphology switch saves the choice to the ledger at once, with no apply step.

## `PLayoutLinked` and `PDialWidth`

`PLayoutLinked` decides whether every tab keeps the same panel widths.
Turning it on copies the last dragged tab's widths to every other tab and saves them.
`PDialWidth` drops the stored panel widths and stands at the foot of the layout page like the folder action.

## Stamp

Commit: `c2f335c331fe`
- `PSettings.xaml` `e65cb88b1edbdfcc`
- `PSettings.xaml.cs` `2c910be1bc87bf9d`
