# PImprint.xaml

## `<UserControl x:Class="Llyn.UIShell.PImprint">`

The edit area of the Source panel, the selected Source as the panel writes it.
It is a control of its own because the markup and the writing side outgrew one file.
Each stated text field carries a `?` toggle beside it that records the value as unknown.
The kind is chosen from a list that carries its own unknown entry, so it needs no toggle.
The three states are distinct facts, so clearing a value is not the same as recording it unknown.
The mode toggle is not in here, because it sits in the panel's action row.

## `<Style x:Key="Imprint.Unknown" ...>`

The `?` toggle every stated field carries.
It stays in this file rather than the merged dictionary, because a style built on another must be resolved where that other is reachable.

## `<Grid x:Name="PAuthor" Margin="0,7,0,0">`

The ordered credits and the workspace's authors offered for crediting.
`PAuthorSwitch` stands disabled until the Source exists, because a credit is an association row on a stored id.
`PAuthorUnknown` records that the authorship is unknown, which is not the same as no credit yet.

## `<ItemsControl x:Name="PAuthorCredit" Button.Click="PAuthorCreditHandle" ...>`

The credit rows, drawn from a template the merged dictionary holds.
The clicks of all four row buttons are taken here, because a template in a dictionary can name no handler.

## `<StackPanel Grid.Row="1" Margin="24,10,24,20" ...>`

The usage figure stays visible while editing, because one correction reaches every place citing the Source.
`PImprintRemoval` offers a plain delete at zero and a detach-and-delete otherwise.
