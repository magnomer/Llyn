# PSettings.xaml

## `<Rectangle ... Style="{StaticResource Theme.Panel.SeamRow}" />`

The seams that part the setting catalog from the card of the chosen group.
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
Choosing a row swaps the card on the right and nothing else.

## `<Border Grid.Row="1" Grid.Column="1" ... Style="{StaticResource Theme.Panel.Surface}">`

The edit area, one card per group drawn in the same cell, only the chosen one visible.
A card opens with its title and purpose, then its rows parted by one-pixel lines.
Each row reads its label over its helper, with the control at the far end.

## `<Button x:Name="PWorkspaceDialog" ...>`

The folder picker beside the path field, an icon action rather than a labelled button.
The label survives as its tooltip.

## `<ToggleButton x:Name="PRespelling" ...>`

Whether looked-up transcriptions are recast into the pack's symbol convention.
The next lookup reads it, so nothing on screen changes when it flips.
The switch keeps the check box's handler and its checked reading, only the drawing changed.

## `<ToggleButton x:Name="PFrequency" ...>`

Whether an entry's frequency is fetched from the pack's web source.
It stands above the inflection switch in the same card, since both govern what a lookup fetches.
A pack that declares no frequency source fetches nothing either way, so the switch costs nothing there.
