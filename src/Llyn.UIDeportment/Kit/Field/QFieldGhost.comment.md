# QFieldGhost.cs

## `internal sealed class QFieldGhost : IMultiValueConverter`

The text a measuring ghost shows: the field's own text, else its hint.
A ghost sizes its field, so an empty field still takes the width of its hint.
The choice sits in a converter, so the driver never branches on what the user typed.

## `public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)`

`values` holds the field's text first and its hint second.
A missing hint reads as empty.

## `public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)`

A ghost only shows, so nothing is ever written back.
