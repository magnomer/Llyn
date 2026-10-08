# PTaxonomy.xaml
Hash: `ce182c1f7a422bae`

## `<Rectangle ... Style="{StaticResource Theme.Panel.SeamRow}" />`

The seams that part the tag catalog, the entries under a tag, and the entry itself.
The row seam runs under the ordering bar and the command row.
It is bled past the panel margin so it meets the navigation's own edge.
There are two column seams here rather than one, because this panel reads across three columns and not two.
The column seam sits in the middle of the gutter and reaches the foot of the window.
Neither seam encloses anything.
That is the whole rule, since a line marks a division and a box would claim an object.

## `<Grid Margin="34,20,34,38">`

Three columns run from narrow to wide.
They hold the tag catalog, the entries under the chosen tag, then the reader.
The library panel asks for an entry by its headword.
This panel asks for it by a label somebody put on one of its cards.
So the catalog here is a tag, and the entry list beside it is what that tag holds.

## `<Border x:Name="PFunnel" ... Style="{StaticResource Theme.Search.Bar}">`

The sorting button and the search field are one control over the tag column.
Their shared edge is the same as the catalog below.
Both act on the tag catalog and nothing else.
The ordering picker is the shared `PChoiceOrder`, placed here as `PTaxonomyOrder`.

## `<Grid Grid.Row="1" Grid.Column="1" Style="{StaticResource Theme.Catalog.Middle}">`

The entry list is a bare catalog column, not a boxed surface.
Both lists on this panel are asked the same kind of question, so both are drawn the same way.
A surface behind one of them would read as an object rather than a list.
Each row carries the flag, the headword and the language, in the library catalog's order.

## `<TextBlock x:Name="PMembershipEmpty" ...>`

Shown when the chosen tag holds no entry.
It also stands while no tag is chosen and the workspace is empty.
Either way there is nothing to read, which is what the line says.

## `<Border x:Name="PLattice" ... Style="{StaticResource Theme.Search.Bar}">`

The search field and the language filter over the entry column.
`PScout` narrows the entries under the chosen tag by typed text.
The language filter is the shared `PChoiceFilter`, placed here as `PTaxonomyFilter`.
It opens the menu of loaded languages, and its mark shows while any is hidden.

## `<veneer:PPanelRail x:Name="PTaxonomyRail" Grid.Row="0" Grid.Column="2" Margin="0,0,0,18" />`

The same entry actions the library panel offers, over the same reader.
The command row is the shared `PPanelRail`, placed here as `PTaxonomyRail`.
Export and print act on the entry being read.

## Catalog spacing

The catalog uses the shared Theme.Catalog.Frame and Theme.Catalog.Scroll styles.
The scroll bar takes its lane only when the list overflows, and the rows then narrow to make room.
The shared Theme.Catalog.Row preserves the same rounded shape, internal padding and row spacing across browse panels.
The matching-entry column uses Theme.Catalog.Middle to measure its left gutter from the intervening seam.

## Hooks

The markup carries no hook.
The Deportment driver `QTaxonomy` sets commands, the search hint and the tag row fills.
`QMembership` drives the entry column and its search field.
The picker, the filter and the rail have their own drivers, which set their icons and popups.
The rail's driver folds the two button pairs by mode.
