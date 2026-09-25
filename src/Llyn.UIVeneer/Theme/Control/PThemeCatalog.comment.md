# PThemeCatalog.xaml

## `<Style x:Key="Theme.Catalog.Row" TargetType="Button">`

One row of a panel's catalog, drawn on nothing until it is pointed at.
The catalogs carry no card of their own, so a bordered row would stack a frame inside a frame.
A row shows its subject over a quieter line of what tells it apart.
The row the panel stands on is tagged as chosen by the panel.
The tag paints the soft accent ground and turns the subject accent, as the reading compass marks its current row.
One ground and one ink say chosen together, so no rail or edge repeats them.

## `<Style x:Key="Theme.Catalog.Epithet" TargetType="Run">`

The epithet run after a listed headword: the reading the language pack names, such as `희롱할 롱` after `弄`.
It sits on the headword's baseline, two steps smaller and muted, so the headword stays the row's one bold word.
An en space leads it, carried by the binding's format, so an empty epithet leaves no visible gap.
The row's title keeps its ellipsis, since both runs share one text block.

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
