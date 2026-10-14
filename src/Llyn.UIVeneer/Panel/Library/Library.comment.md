# Library

This folder holds the library panel, which browses entries, and the duplex panel, which compares two entries side by side.
The duplex is two peer wings, each a veneer page placed twice.
The C# shells only call `InitializeComponent()`.

## `PLibrary`

### Seams

The seams part the entry catalog from the entry it opens.
The row seam runs under the ordering bar and the command row.
It is bled past the panel margin to meet the navigation's edge.
The column seam sits in the middle of the gutter and reaches the foot of the window.
Neither seam encloses anything, because a line marks a division and a box would claim an object.

### `POrder`

The ordering button and the search field are one control rather than two boxes.
The bar owns the ground and the focus ring, so both children are drawn bare.
Its outline shows only under the pointer or on focus.
Its width is the index column's, so its edge is the same as the catalog below.
The viewer deliberately has no control in this row.
The ordering picker is the shared `PChoiceOrder`, placed as `PLibraryOrder`.
The language filter is the shared `PChoiceFilter`, placed as `PLibraryFilter`.
Their own drivers set their icons and popups, and the panel wires the search field.

### `PLibraryRail`

It holds the entry actions over the reader, and the reader and editor toggle.
The command row is the shared `PPanelRail`, placed bare like every other panel's.
So its mode toggle keeps the right end.
The import button is the rail's own `PPanelRailMarkup`, shown only here by `QLibrary`.

### `PIndex`

It is the entry catalog, one `Theme.Catalog.Row` per Entry.
The shared index fill paints the named row parts and marks the chosen row.

### `PDisplay`

The read-only entry view is shared with the phonology and taxonomy panels.
What differs between the panels is which entry is selected, not how it reads.

### Catalog spacing

The catalog uses the shared `Theme.Catalog.Frame` and `Theme.Catalog.Scroll` styles.
The scroll bar takes its lane only when the list overflows, and the rows then narrow to make room.
The shared `Theme.Catalog.Row` keeps the same rounded shape, padding and row spacing across browse panels.

## `PDuplex`

The duplex panel holds two wings, each one side of the comparison.
The two wings are peers and neither drives the other.
The duplex driver gives each place its own `QWing`.

### Seams

One seam parts the two search bars from the pair of entries below them.
It is bled past the panel margin to meet the navigation's edge.
Its row is fixed to the bar's height and gap, since the bars belong to the wings.
The column seam sits in the middle of the gutter and parts one side from the other.
Neither seam encloses anything, and the two entries read as two pages side by side.

### `PLeftWing`

A wing spans both rows, so its bar lines up with the seam the panel draws.
Only the gutter margin differs between the two.

## `PWing`

### Search bar

The shared search bar holds the ordering dropper, divider, query field, divider and language sieve.
Every other searching panel stands on this shape, so the wing stands on it too.
The bar has no name because nothing reaches for it after the markup is built.
The two dropdowns sit beside the lookup grid, not inside the bar.
So the grid's focus scope holds only the bar and the list.
Each is offset to the bar's own edge.

### `PWingQuery`

It is the query field of this side alone.
The wing subscribes a key press beside the text change.
So the arrow keys can walk the list beneath.

### `PWingLookup`

The grid holds the search bar and the result tray together.
Its keyboard focus within is what the wing watches, so the list closes only when focus leaves both.

### Body grid

The side owns two things beneath its bar.
The matches the query found stand above, and the Entry chosen from them below.
The matches collapse once a row is taken, so the Entry reads on the full height.

### `PWingTray`

The card follows the list it frames, and `PWingSeam`, drawn beneath the lookup row, follows the same list.
Both start collapsed, and the wing copies the list's visibility onto them whenever it changes.
An empty card would otherwise sit above the Entry with nothing in it.
The empty text inside it shows while a typed query matches nothing.

### `PWingIndex`

The result list is hidden until a query is typed.
It is hidden again on a pick, an Escape or a focus leave, as Conduct's rows verdict says.
Its rows are buttons that take focus, and they stand inside the lookup grid, so taking one is no leave.
Each row takes `Theme.Catalog.Row`, and its named parts are filled as the library index fills them.

### `PWingDisplay`

The read-only entry view is shared, so both sides read an Entry the way the library panel does.
Comparison is what differs, not how an Entry reads.

## Stamp

Commit: `c6e60141a172`
- `PDuplex.xaml` `53f7bb0809856f2e`
- `PDuplex.xaml.cs` `015ae5f24112fadb`
- `PLibrary.xaml` `fa075bb95d2d52e5`
- `PLibrary.xaml.cs` `29517ab6f4728d2d`
- `PWing.xaml` `4eb791266ac60e0f`
- `PWing.xaml.cs` `78f34d07f0471129`
