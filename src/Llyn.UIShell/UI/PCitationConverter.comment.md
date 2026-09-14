# PCitationConverter.cs

## `internal sealed class PCitationConverter`

Turns the source anchor a read-only example carries into the `Author (Year)` line it shows.
An example stores the id of its Source alone, and the display writes the byline beside the sentence.
So the bylines are read once for the whole entry and held here while it is shown.

The display cannot ask per example, because each sentence would then be its own query.
One lookup for the entry keeps the reading of an entry to a single question.

## `internal void PCitationConverterShow(IReadOnlyDictionary<long, string> lines)`

Takes the byline of every stored Source, by id.
It is called before the cards are handed over, so the templates find them ready.

## `internal void PCitationConverterClear()`

Forgets the bylines of the entry that was shown.

## Inline notes

### `public object Convert(object value, Type targetType, object parameter, CultureInfo culture)`

An example citing nothing yields an empty line, which the template collapses.
An id no byline answered is drawn as the id itself.
A citation is a fact worth showing even when its Source is gone.
