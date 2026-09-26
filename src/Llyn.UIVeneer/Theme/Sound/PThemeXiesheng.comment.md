# PThemeXiesheng.xaml

## `<ResourceDictionary`

The look of the series page of the xiesheng panel.
The columns of the panel are drawn with the catalog styles every panel shares.

## `<Style x:Key="Theme.Stem.Empty"`

The muted line a series with no character prints.

## `<Style x:Key="Theme.Stem.Chip"`

One member character as a pressable chip: accent-colored, underlined under the pointer.
Its command, parameter, character and underline are `PLook` rows copied from the item it stands for.

## `<DataTemplate x:Key="Theme.Stem.Glyph"`

Draws one character of the series page as a chip.
