# PStem.xaml

## `<ScrollViewer`

The page of one phonetic series, shown in the reader column while a series is chosen and no entry is.
It prints the series key as a headword, the series chip and the language, then the member characters.
The characters wrap, since a wide series is a long line and not a wide one.

## `<TextBlock x:Name="PStemEmpty"`

Stands in while the series holds no character to print.

## `<ItemsControl x:Name="PStemList"`

One chip per member character, each opening the entry written with it.
