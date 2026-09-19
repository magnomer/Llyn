# PImageConverter.cs

## `internal sealed class PImageConverter`

Turns the picture locations a read-only card carries into pictures it can show.
A card stores locations alone, and the display draws the pictures themselves.
So each location is wrapped in the same row the editor uses, which already loads its own preview.

The display shares that wrapper rather than loading pictures a second way.
One loader for both modes keeps a picture identical whichever mode shows it.

## Inline notes

### `public object Convert(object value, Type targetType, object parameter, CultureInfo culture)`

A location left unspecified is dropped rather than drawn as an empty frame.
That is a picture never chosen, which the reading side has nothing to show for.
