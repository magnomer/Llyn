# PSettings.xaml

## `<Rectangle ... Style="{StaticResource Theme.Panel.SeamRow}" />`

The seams that part the setting catalog from the page of the chosen group.
The row seam runs under the search field and is bled past the panel margin, as every tab does it.
The column seam sits in the gutter and reaches the foot of the window.

## `<Style x:Key="Theme.Ledger.Row" ...>`

The catalog row of the panel, marked chosen through the item it is bound to.
It is the same accent edge the other catalogs paint, so the settings tab reads as one of them.

## `<TextBox x:Name="PWinnow" ...>`

The search field over the catalog, a bare field with no ordering or filter button.
Five groups need no ordering, and the text alone decides which rows stay.

## `<ItemsControl x:Name="PLedger">`

The catalog of setting groups, one row each, the title over a summary of the values the group holds.
Choosing a row swaps the page on the right and nothing else.

## `<Border Grid.Row="1" Grid.Column="1" ... Style="{StaticResource Theme.Panel.Surface}">`

The edit area, one page per group drawn in the same cell, only the chosen one visible.
A page opens with its heading and purpose, then its rows parted by hairlines.
Each row reads its label over its helper, with the control at the far end.
No box is drawn around a page, because the surface it stands on is already the region.
A page and its rules run the whole width of the surface, as a seam does.
Each row caps its label column instead, so the control stands close after the text in a wide window.
The filler column past the control takes the rest, which is why the rule can outrun the row.

## `<Button x:Name="PDialFolder" ...>`

The action that opens the workspace folder, set at the foot of the workspace page.
It stands with the setting it acts on rather than in a command rail over every page.
The negative margin lines its label up with the rows above, since the button pads its own text.

## `<Button x:Name="PDialWidth" ...>`

The action that drops the stored panel widths, set at the foot of the layout page for the same reason.

## `<Button x:Name="PWorkspaceDialog" ...>`

The folder picker beside the path field, an icon action rather than a labelled button.
The label survives as its tooltip.

## `<ToggleButton x:Name="PRespelling" ...>`

Whether readings are shown and edited in the pack's respelling convention instead of as they came.
Both forms are stored, so a flip redraws every open reading through a settings bulletin and refetches nothing.
The switch keeps the check box's handler and its checked reading, only the drawing changed.

## `<ToggleButton x:Name="PSettingsEpithet" ...>`

The switch of the listing page: whether every list prints an entry's epithet after its headword.
The epithet is the reading the language pack names, so a Han character lists as `弄 [희롱할 롱]`.

## `<ToggleButton x:Name="PFrequency" ...>`

Whether an entry's frequency is fetched from the pack's web source.
It stands above the inflection switch in the same card, since both govern what a lookup fetches.
A pack that declares no frequency source fetches nothing either way, so the switch costs nothing there.
