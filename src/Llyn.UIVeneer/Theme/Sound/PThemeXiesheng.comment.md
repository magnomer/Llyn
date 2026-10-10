# PThemeXiesheng.xaml
Hash: `74a20843436d7345`

## `<ResourceDictionary`

The look of the series page of the xiesheng panel.
The columns of the panel are drawn with the catalog styles every panel shares.

## `<Style x:Key="Theme.Stem.Empty"`

The muted line a series with no character prints.

## `<ResourceDictionary.MergedDictionaries>`

Merges the palette, input and reflex sheets, so the styles find their brushes.
The member template finds the reading-line style and the reflex row template there too.

## `<Style x:Key="Theme.Stem.Chip"`

One member character as a pressable link: accent-colored, underlined under the pointer.
Its command, parameter, character and underline are `QLook` rows copied from the item it stands for.

## `<Style x:Key="Theme.Stem.Fold"`

The member hinge: the entry page fold toggle with its own text keys.
Folded it reads `Xiesheng.ReadingsShow`, and its checked `QLook` row swaps in `Xiesheng.ReadingsHide`.
The look comes from `Theme.Reflex.Fold`, so only the content, left alignment and bottom gap are restated.

## `<DataTemplate x:Key="Theme.Stem.Member"`

Draws one member of the series page: the character link with its reading line beside it, then its reflex rows.
The reading line wears `Theme.Text.Reading`, the look of the reading under the entry page headword.
The reflex rows reuse `Theme.Reflex.Display`, the entry page table in view mode, inside the same `QReflexFrame`.
The frame sets the plain text font back, so the glyph font the list carries reaches only the character.
The hinge `PStemMemberHinge` sits above the rows and wears `Theme.Stem.Fold`, which shows or hides every reflex row.
The rows and hinge share a stack no wider than the rows, so the hinge stays left.
The hinge keeps its place whether folded or opened, because the rows unfold below it.
