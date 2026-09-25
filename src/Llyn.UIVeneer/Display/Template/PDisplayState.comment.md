# PDisplayState.xaml

## `ResourceDictionary`

The reading of a three-state card value, held alone so every card row can reach it.
A loose dictionary resolves a static reference only in itself, in what it merges, and in the application resources.
So a converter shared across row dictionaries has to be a dictionary of its own that each row merges.
