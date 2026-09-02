# PStateConverter.cs

## `public sealed class PStateConverter`

Shows a stored value for reading, and names an unreadable one for writing.
A field showing nothing is one of two things.
Nothing was ever recorded, or something was and cannot be read back.
This is what tells a reader which.
The first shows nothing at all.
The second shows the mark the interface language gives for it.

## `public object Convert(object value, Type targetType, object parameter, CultureInfo culture)`

A value being read.
It shows the text it states, and nothing at all when nothing was recorded.
It shows the mark when what was recorded cannot be read back.

## `public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)`

The hint standing in an empty input field.
It is the mark when the field holds something unreadable.
Otherwise it is whatever the field would normally invite the user to write.
The first value says whether the field is unreadable and the second carries that normal invitation.
