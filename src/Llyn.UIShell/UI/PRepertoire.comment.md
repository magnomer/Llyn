# PRepertoire.xaml

## `<Border x:Name="PTier" ... Style="{StaticResource Theme.Search.Bar}">`

The ordering button and the search field are one bordered control over the catalog column.
The action row over the broader column stands in the same top row, as the library panel arranges it.

## `<StackPanel Grid.Row="0" Grid.Column="1" ...>`

The action row of the panel.
`PRepertoireFresh` opens the editor on a Situation nothing references yet.
This panel is the only place such a Situation can arise.
Elsewhere one is written from the card that carries it.
Export and print are mock-up controls and are not wired.

## `<ItemsControl x:Name="PAtlas">`

The catalog of every Situation the workspace holds.
A row reads its title over its kind, with the number of places referencing it at the far end.
That number is shown here and not only in the display, because it decides which delete the panel offers.

## `<Grid x:Name="PVignette">`

The reading of one Situation, and the sides referencing it beneath.
`PVignette` here stands on a Situation rather than on an Entry.
What it shares with the entry panels is the read-and-edit mechanism and the `PRepertoireScribe` toggle, not the object.
The four fields stand as labeled rows for title, kind, description and source.
Each is drawn whether or not it holds anything.
A field is stored data, so a hidden row would hide the difference between never written and written-but-unreadable.
A row therefore reads its value, the unreadable mark, or the unrecorded mark in the muted colour.

## `<ItemsControl x:Name="POccurrence">`

Everything referencing the selected Situation, one row per referring side.
A row names the Meaning or the Collocation and the Entry it belongs to.
The relationship is never flattened into the Entry alone.
Choosing a row leaves for that Entry in the library panel.
The list is read-only: a reference is added or dropped on the card holding it, never here.

## `<Grid x:Name="PScenario" Visibility="Collapsed">`

The editable view of the selected Situation.
The three fields are three-state, so an empty field says which kind of empty it is.
`PCitation` is the single Source the Situation cites, a pointer that is cleared without touching the Source itself.

## `<Button x:Name="PScenarioRemoval" ...>`

Deletes the shown Situation.
It stands with discard and save because it acts on the record rather than on the browsing beside it.
It is disabled while the editor stands on a Situation nothing has stored yet.
