# PTheme.xaml

## `<ResourceDictionary.MergedDictionaries>`

The theme is one dictionary per role rather than one file for every role at once.
Each part names the styles of a single subject and merges only the parts it draws from.
A part therefore resolves its own StaticResource and BasedOn references without reaching sideways.
This file merges the parts in dependency order and holds no style of its own.
App.xaml keeps merging this one file, so the theme has one address.
The Mention and Gloss parts are merged last, because they stand on the palette and the input part alone.
The Paradigm part follows them, standing on the palette alone.
