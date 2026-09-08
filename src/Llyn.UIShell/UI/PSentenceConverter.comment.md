# PSentenceConverter.cs

## `internal sealed class PSentenceConverter`

Writes the frame a card's example is read under, ahead of the example itself.
The editor gives the frame two fields side by side, and the reader has one line to read.
So the marker and the role are drawn as a parenthesised head, as `(+of Something) This is an example of ...`.

The order of the two fields belongs to the language, not to the row.
The display holds one converter for the shown entry, so the order is read once for the whole entry.

## `internal void PSentenceConverterApply(LSentenceOrder order)`

Takes the order the entry's language writes its frame in.
It is called before the cards are handed over, so the templates find it ready.

## `public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)`

Every caller passes the marker first, the role second, the example third and the unreadable mark last.
A field showing nothing is left out of the head rather than drawn as an empty place.
The head is dropped whole when neither field says anything, because a frame around nothing frames nothing.
A field that cannot be read back shows the mark the interface language gives for it, as elsewhere in the card.
The parameter says which half of the line is wanted, the head or the example.
The reading view asks for each half apart, so it can draw the head in its own face and colour.
A caller passing nothing is given the whole line, head and example together.
The head is handed over without a space after it, because both views now keep it apart from the example.
The room between them is given as room, which neither view can drop the way a trailing space is dropped.
A caller taking the whole line still gets the two written as one sentence, with a space between them.
