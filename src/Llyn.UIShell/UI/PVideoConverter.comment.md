# PVideoConverter.cs

## `internal sealed class PVideoConverter`

Turns the video locations a read-only card carries into videos it can play.
A card stores locations alone, exactly as it does for pictures.
So each location is wrapped in the same row the editor uses, which already resolves its own address.

The display shares that wrapper rather than resolving addresses a second way.
One resolver for both modes keeps a video identical whichever mode shows it.

## Inline notes

### `public object Convert(object value, Type targetType, object parameter, CultureInfo culture)`

A location left unspecified is dropped rather than shown as an empty frame.
That is a video never chosen, which the reading side has nothing to play.

A read row starts stopped, exactly as a written one does.
Nothing here has to say so, because a row is stopped until something asks it to play.
