# PWing.xaml

## `<Border Grid.Row="0" Margin="0,0,0,18" Style="{StaticResource Theme.Search.Bar}">`

The shared search bar: ordering dropper, divider, query field, divider and language sieve.
Every other searching panel stands on this shape, so the wing stands on it too.
The bar has no name because nothing reaches for it after the markup is built.
The ordering dropdown therefore hangs from the dropper and is offset to the bar's own edge.

## `<TextBox x:Name="PWingQuery" ... />`

The query field of this side alone.
The wing subscribes a lost focus and a key press beside the text change.
So the list beneath can close when the reader leaves, and the arrow keys can walk it.

## `<Grid Grid.Row="1" Margin="0,14,0,0">`

The side owns two things beneath its bar.
Above are the matches the query found, below is the Entry chosen from them.
The matches collapse once a row is taken, so the Entry reads on the full height.

## `<Border x:Name="PWingTray" ...>`

The card follows the list it frames, and `PWingSeam` above it follows the same list.
Both start collapsed, and the wing copies the list's visibility onto them whenever it changes.
An empty card would otherwise sit above the Entry with nothing in it.
The empty text inside it shows while a typed query matches nothing.

## `<ItemsControl x:Name="PWingIndex" Visibility="Collapsed">`

The result list, hidden until a query is typed and hidden again on a pick, an Escape or a leave.
Its rows are buttons that take focus, so a lost focus bubbles here as it does from the field.
Each row takes `Theme.Catalog.Row`, and its named parts are filled as the library index fills them.

## `<local:PDisplay x:Name="PWingDisplay" />`

The read-only entry view is shared, so it is a control rather than markup written here.
Both sides read an Entry the same way the library panel does.
Comparison is what differs, not how an Entry reads.
