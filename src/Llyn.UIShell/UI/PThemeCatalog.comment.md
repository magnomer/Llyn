# PThemeCatalog.xaml

## `<Style x:Key="Theme.Catalog.Row" TargetType="Button">`

One row of a panel's catalog, drawn on nothing until it is pointed at.
The catalogs carry no card of their own, so a bordered row would stack a frame inside a frame.
A row shows its subject over a quieter line of what tells it apart.
The row the panel stands on is painted by the panel, which tags it as chosen.
The tag raises a straight accent rail inset along the leading side.
A left border would bend around the rounded corners and read as a curved tip.

## `<Style x:Key="Theme.Catalog.Pellet" TargetType="Border">`

The count at the far end of a catalog row, held in a quiet capsule.
It is muted rather than accented, because the accent is what marks the chosen row.

## `<Style x:Key="Theme.Catalog.Empty" TargetType="TextBlock">`

The line that says a catalog holds nothing, lying over the rows that are not there.
It never takes the pointer, so a click through it still reaches the catalog.

## Catalog frame and scrolling styles

Theme.Catalog.HeaderMargin brings each catalog's sort and search row to the same horizontal edges as its list rows.
It offsets the panel's 34-pixel inset, leaving the shared 6-pixel outer gutter.
It ends where the row surface ends before the scroll rail.
Theme.Catalog.Frame offsets the 34-pixel panel margin to leave a 6-pixel left gutter.
Theme.Catalog.Middle offsets the seam 16 pixels before an interior column to leave the same 6-pixel gutter.
Both end at the next seam, where Theme.Catalog.Scroll opens its lane once the rows outrun the panel.
Library, Sound, Tags (including matching entries), Situations, Examples, Sources and Favorites share these styles.
The document and editor scroll viewers hold the same lane at the same width.
