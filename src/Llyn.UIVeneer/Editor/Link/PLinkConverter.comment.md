# PLinkConverter.cs

## `internal sealed class PLinkConverter`

Turns the ids a read-only card carries into the links it shows.
A card stores ids alone, and the display draws headwords with their flags.
So the headwords are read once for the whole entry and held here while it is shown.

The display cannot ask per card, because each card would then be its own query.
One lookup for the entry keeps the reading of an entry to a single question.

## `internal void PLinkConverterShow(IReadOnlyList<LTranslationTarget> targets)`

Takes the headwords the entry's links resolved to.
It is called before the cards are handed over, so the templates find them ready.

## Inline notes

### `public object Convert(object value, Type targetType, object parameter, CultureInfo culture)`

An id no headword answered is left out rather than drawn as itself.
That is an entry deleted since the link was stored, which is nothing to read aloud.
