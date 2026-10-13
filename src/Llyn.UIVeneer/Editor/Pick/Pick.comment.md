# Pick

This folder holds the dropdown rows offered while a caret is typing: Situations and entry prospects.
The Deportment class of the same name loads each template, and the editor's fill writes the parts.
Each row is a plain surface rather than a button, so the list beneath it keeps its own selection.

## `PProfferTemplate.xaml`

### `Theme.Proffer.Row`

It shows one offered Situation: its wording, with its usage count against the right edge.

### `PProfferRow`

The row answers the pointer going down rather than coming up.
A popup takes the window's activation the moment it is pressed, which ends the caret's focus.
Answering on the press attaches the Situation and empties the caret before that happens.
So the wording the user was typing is not committed a second time as a new Situation.

### `PProfferLead`

The pieces of the wording are runs of one line rather than three blocks.
A run carries no spacing of its own, so the wording reads as one word broken only by weight.

## `PProspectTemplate.xaml`

### `Theme.Prospect.Row`

It is one row of the dropdown offered when the typed word does not name one entry outright.
It reads as the other headword lists do, so the same word means the same thing everywhere.

### `PProspectMark`

A create row is marked with a leading plus, because it makes an entry rather than pointing at one.
The plus starts hidden, and the fill shows it on a create row.

## Stamp

Commit: `c2f335c331fe`
- `PProfferTemplate.xaml` `3dd5481567401a74`
- `PProspectTemplate.xaml` `a31a1b4fa8e4528e`
