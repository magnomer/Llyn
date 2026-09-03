# PProspectTemplate.xaml

## `Theme.Prospect.Row`

One row of the dropdown offered when the typed word does not name one entry outright.
It reads as the other headword lists do, so the same word means the same thing everywhere.
A create row is marked with a leading plus, because it makes an entry rather than pointing at one.
The row is a plain surface rather than a button, so the list beneath it keeps its own selection.

## `PProspectTemplate.xaml.cs`

The dictionary forwards the row's click to the editor that owns the popup.
The Category, Phonetic and Clip dictionaries do the same for their own rows.
