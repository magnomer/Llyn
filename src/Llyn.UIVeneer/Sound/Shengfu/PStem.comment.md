# PStem.xaml
Hash: `d984654ec6ca540a`

## `<ScrollViewer`

The page of one phonetic series, shown in the reader column while a series is chosen and no entry is.
It prints the series key as a headword, the series chip and the language, then the members.
The members stack, one under another, each with its reading and its reflex rows.

## `<TextBlock x:Name="PStemEmpty"`

Stands in while the series holds no character to print.

## `<ItemsControl x:Name="PStemList"`

One row per member, its character opening the entry written with it.
The list is the shared-size scope, so the reflex columns line up across members as on the entry page.
The rows are filled and the chip command bound by `QStem`, since the markup holds no binding.
