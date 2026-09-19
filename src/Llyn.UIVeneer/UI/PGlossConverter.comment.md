# PGlossConverter.cs

## `internal sealed class PGlossConverter`

Turns the Glosses a read-only card carries into rows it can show.
A card stores each Gloss as its language and text alone, and the display needs the flag beside it.
So each Gloss is wrapped in the same row the editor uses, which already resolves its own flag.

The display shares that wrapper rather than resolving flags a second way.
One resolver for both modes keeps a Gloss identical whichever mode shows it.

## Inline notes

### `private static readonly ObservableCollection<PLanguageItem> PGlossConverterCatalog = [];`

The row asks for a language catalog because the editor lets a Gloss change language.
The display never opens that picker, so every row shares one empty catalog.
