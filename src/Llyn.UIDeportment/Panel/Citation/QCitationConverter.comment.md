# QCitationConverter.cs

## `internal sealed class QCitationConverter`

Turns the Source id a read-only example cites into the line it shows beside the sentence.
An example stores the id of its Source alone, and the display writes the line beside the sentence.
So the lines are read once for the whole entry and held here while it is shown.

The display cannot ask per example, because each sentence would then be its own query.
One lookup for the entry keeps the reading of an entry to a single question.

## `internal void QCitationConverterShow(IReadOnlyDictionary<long, string> lines)`

Takes the ready line of every Source the shown entry cites, by id.
The engine already wrote a Source that names itself nowhere as its id.
It is called before the cards are handed over, so the templates find them ready.

## Inline notes

### `public object Convert(object value, Type targetType, object parameter, CultureInfo culture)`

Only looks the id up.
An example citing nothing has no line, which the template collapses.
