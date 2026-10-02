# PGlossConverter.cs
Hash: `c162fd15de6b5804`

## `internal sealed class PGlossConverter : IValueConverter`

Turns the Conduct Glosses a read-only card carries into rows it can show.
A card stores each Gloss as its language and text alone, and the display needs the flag beside it.
So each Gloss is wrapped in the same row the editor uses, which already resolves its own flag.

The display shares that wrapper rather than resolving flags a second way.
One resolver for both modes keeps a Gloss identical whichever mode shows it.

## `public object Convert(object value, Type targetType, object parameter, CultureInfo culture)`

The binding's face of the typed builder below, so a value that is not a Gloss list shows no rows.

## `internal static IReadOnlyList<PGloss> PGlossConverterCreate(IEnumerable<CGlossDraft>? rows)`

One shown row per Conduct Gloss, in order, typed so a driver needs no cast on the answer.
The reading view builds its line Glosses here, as the binding does through `Convert`.

## Inline notes

### `private static readonly ObservableCollection<PLanguageItem> PGlossConverterCatalog = [];`

The row asks for a language catalog because the editor lets a Gloss change language.
The display never opens that picker, so every row shares one empty catalog.
