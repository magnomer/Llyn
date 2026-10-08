# PFavorite.xaml
Hash: `3d018f7389f8ecf5`

## `<Rectangle ... Style="{StaticResource Theme.Panel.SeamRow}" />`

The seams that part the favourite catalog from the entry it opens.
The row seam runs under the ordering bar and the command row.
It is bled past the panel margin so it meets the navigation's own edge.
The column seam sits in the middle of the gutter and reaches the foot of the window.
Neither seam encloses anything, which is the whole rule.
A line marks a division, and a box would claim an object.

## `<Border x:Name="PSeries" ... Style="{StaticResource Theme.Search.Bar}">`

The sort control and the search field are one control over the roster column.
Its edge matches the catalog below, and the outline around both is drawn once.
The ordering picker is the shared `PChoiceOrder`, placed here as `PFavoriteOrder`.
The language filter is the shared `PChoiceFilter`, placed here as `PFavoriteFilter`.
The panel has no new-record button, because a new entry is written in the input tab and never here.

## `<veneer:PDisplay x:Name="PDisplay" />`

The read-only entry view is shared, so it is a control rather than markup written here.
The favorite entries read exactly as they do in the other entry-browsing panels.
What differs between the panels is which entry is selected, not how it reads.

## Catalog spacing

The catalog uses the shared Theme.Catalog.Frame and Theme.Catalog.Scroll styles.
The scroll bar takes its lane only when the list overflows, and the rows then narrow to make room.
The shared Theme.Catalog.Row preserves the same rounded shape, internal padding and row spacing across browse panels.

## Hooks

The markup carries no hook.
The Deportment driver `QFavorite` sets commands, the search hint and row fills.
The picker and the filter have their own drivers, which set their icons and popups.
The command row is the shared `PPanelRail`, placed here as `PFavoriteRail`.
Its own driver folds the two button pairs by mode.
