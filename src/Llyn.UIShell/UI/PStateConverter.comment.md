# PStateConverter.cs

## `public sealed class PStateConverter`

Shows a stored value for reading, and names an unreadable one for writing. A field showing nothing is one of two things — nothing was ever recorded, or something was and cannot be read back — and this is what tells a reader which: the first shows nothing at all, the second the mark the interface language gives for it.

## `public object Convert(object value, Type targetType, object parameter, CultureInfo culture)`

A value being read: the text it states, nothing at all when nothing was recorded, and the mark when what was recorded cannot be read back.

## `public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)`

The hint standing in an empty input field: the mark when the field holds something unreadable, and otherwise whatever the field would normally invite the user to write. The first value says whether the field is unreadable and the second carries that normal invitation.
