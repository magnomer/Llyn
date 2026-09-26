# PProspectTemplate.xaml

## `Theme.Prospect.Row`

One row of the dropdown offered when the typed word does not name one entry outright.
It reads as the other headword lists do, so the same word means the same thing everywhere.
The row is a plain surface rather than a button, so the list beneath it keeps its own selection.
The Deportment class of the same name loads this markup, and the editor's fill writes the parts.

## `PProspectMark`

A create row is marked with a leading plus, because it makes an entry rather than pointing at one.
The plus starts hidden, and the fill shows it on a create row.
