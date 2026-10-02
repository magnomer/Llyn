# PTheme.xaml
Hash: `062fced7cae9f1ae`

## `<ResourceDictionary.MergedDictionaries>`

The theme is one dictionary per role rather than one file for every role at once.
Each part names the styles of a single subject and merges only the parts it draws from.
A part therefore resolves its own StaticResource and BasedOn references without reaching sideways.
This file merges the parts in dependency order and holds no style of its own.
It names only veneer parts, since the deportment holds no markup at all.
App.xaml keeps merging this one file, so the theme has one address.
The Mention, Etymology, Gloss and Paradigm parts are merged late, on the palette and at most the input part.
The Script, Fanqie, Diwei and Xiesheng parts close the list, each on the palette and at most one more part.
