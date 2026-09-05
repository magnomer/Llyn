# PDuplex.xaml

## `<TextBox x:Name="PLeftQuery" ... />`

Each side carries its own query field above it.
The two fields are peers and neither drives the other.

## `<Grid x:Name="PLeft" Grid.Row="1" Grid.Column="0" Margin="0,0,10,0">`

The comparison side owns two things.
Above are the matches the query found, below is the Entry chosen from them.
The matches collapse once a row is taken, so the Entry reads on the full height.

## `<Border Grid.Row="0" ... Visibility="{Binding Visibility, ElementName=PLeftIndex}">`

The card follows the list it frames rather than carrying a name of its own.
An empty card would otherwise sit above the Entry with nothing in it.

## `<local:PDisplay x:Name="PLeftDisplay" />`

The read-only entry view is shared, so it is a control rather than markup written here.
Both sides read an Entry the same way the library panel does.
Comparison is what differs, not how an Entry reads.

## `<ItemsControl x:Name="PRightIndex" Visibility="Collapsed">`

The right side repeats the left side's markup with its own names.
The two sides are independent, so neither can be a template of the other at runtime.
