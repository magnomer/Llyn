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

## Inline notes

### `public object Convert(object value, Type targetType, object parameter, CultureInfo culture)`

An example citing nothing yields an empty line, which the template collapses.
An id no byline answered is drawn as the id itself.
A citation is a fact worth showing even when its Source is gone.

### `public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)`

The same line for a row being written, looked up in the Sources the form offers.
The row binds its anchor and the shared catalog, and the name is found when the field is drawn.
The row keeps no copy of the name.
A reloaded catalog only needs the row to say its anchor again.
