# PWing.xaml

## `<Style x:Key="Theme.Index.Row" TargetType="Button" BasedOn="{StaticResource Theme.Catalog.Row}">`

The result row, which paints the chosen accent when its item says it is the chosen one.
The library index paints its rows the same way, so a picked entry reads alike in both panels.

## `<Border Grid.Row="0" Margin="0,0,0,18" Style="{StaticResource Theme.Search.Bar}">`

The shared search bar: ordering dropper, divider, query field, divider and language sieve.
Every other searching panel stands on this shape, so the wing stands on it too.
The bar has no name because nothing reaches for it after the markup is built.
The ordering dropdown therefore hangs from the dropper and is offset to the bar's own edge.

## `<TextBox x:Name="PWingQuery" ... />`

The query field of this side alone.
A lost focus and a key press reach the code beside the text change.
So the list beneath can close when the reader leaves, and the arrow keys can walk it.

## `<Grid Grid.Row="1" Margin="0,14,0,0">`

The side owns two things beneath its bar.
Above are the matches the query found, below is the Entry chosen from them.
The matches collapse once a row is taken, so the Entry reads on the full height.

## `<Border Grid.Row="0" Margin="0,0,0,12" ... Visibility="{Binding Visibility, ElementName=PWingIndex}">`

The card follows the list it frames rather than carrying a name of its own.
The seam above it follows the same list.
An empty card would otherwise sit above the Entry with nothing in it.
The empty text inside it shows while a typed query matches nothing.

## `<ItemsControl x:Name="PWingIndex" LostKeyboardFocus="PWingLeaveHandle" Visibility="Collapsed">`

The result list, hidden until a query is typed and hidden again on a pick, an Escape or a leave.
Its rows are buttons that take focus, so a lost focus bubbles here as it does from the field.

## `<local:PDisplay x:Name="PWingDisplay" />`

The read-only entry view is shared, so it is a control rather than markup written here.
Both sides read an Entry the same way the library panel does.
Comparison is what differs, not how an Entry reads.
