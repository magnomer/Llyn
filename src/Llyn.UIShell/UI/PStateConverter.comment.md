# PStateConverter.cs

## `public sealed class PStateConverter`

Shows a stored value for reading, and names an unknown one for writing.
A field showing nothing is one of two things.
Nothing was ever recorded, or the user marked it as not known.
This is what tells a reader which.
The first shows nothing at all.
The second shows the mark the interface language gives for it.

## `public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)`

Every caller passes the state first and the unknown mark second.
A stored value shows its own text.
It shows the mark when the value is not known, and nothing when nothing was recorded.
An input field passes a plain flag instead and adds the hint it invites the user with third.
The mark and the hint both arrive as bindings, so a language change reaches text that has already been drawn.
