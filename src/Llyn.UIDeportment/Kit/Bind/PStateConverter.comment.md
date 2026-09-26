# PStateConverter.cs

## `public sealed class PStateConverter`

Shows a stored value for reading, and names an unknown one for writing.
A field showing nothing is one of two things.
Nothing was ever recorded, or the user marked it as not known.
This is what tells a reader which.
The first shows nothing at all, or the hint a field invites with.
The second shows the mark the interface language gives for it.
It is the one place in the shell that reads the three states apart.
A panel that must know whether a value is unknown asks the value's own verdict instead.

## `public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)`

Every caller passes the stored value first and the unknown mark second.
A reader passes those two alone, and a stored value shows its own text.
It shows the mark when the value is not known, and nothing when nothing was recorded.
An input field adds the hint it invites the user with third, and then the result is its placeholder.
A placeholder reads the mark for an unknown value and the hint for any other.
The field's own text covers the placeholder while a value stands, so the value is never drawn twice.
The mark and the hint both arrive as bindings, so a language change reaches text that has already been drawn.

## `public object Convert(object value, Type targetType, object parameter, CultureInfo culture)`

The text of a stored value alone, for a field the user types into.
An unknown value types as nothing, and the mark beside it says why.
Nothing comes back the other way, because what is typed leaves as a request and never as a state.
