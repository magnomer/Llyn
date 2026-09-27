# LOccurrence.cs

## `public sealed class LOccurrence`

The deportment of the repertoire panel's occurrence list: the panel state over the occurrence vista and its rows.
The rows are the entries referencing the chosen Situation, so it keeps a handle on the situation vista too.
The occurrence side never deletes, so its delete seam always refuses.
It prints and exports the occurrence vista's chosen entry.

## `public string LOccurrenceEmptyRead(string? sortie)`

The wording key for an empty occurrence list, chosen by whether the search text is blank.
