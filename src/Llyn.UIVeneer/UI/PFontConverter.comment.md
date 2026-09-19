# PFontConverter.cs

## `internal sealed class PFontConverter`

Drops a line of text until it stands on the baseline of the text it is set beside.
An example is read in the pack's own face and its frame in the interface face.
Two faces sit at two baselines.
Laying them side by side by their tops leaves one hanging above the other.

The reading view once drew the frame and the example in one line of text.
The framework baselined that line for it.
The writing view cannot, because the example is written into a field of its own.
So both views now set the two apart and drop the example here.
A reader sees the line stand still between them.

## `public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)`

Every caller passes the leading face and its size first.
Then comes the face of the text being dropped and its size.
The drop is the distance between the two baselines, and a text already sitting lower is left alone.
The faces are read from the elements themselves.
A pack declaring its own example face is followed without being named here.
