# Marker

This folder holds the speech marker chip and the slate row of the editor, as markup alone.
The Deportment class of the same name loads each template and forwards clicks to the editor.

## `PMarkerTemplate.xaml`

`Theme.Marker.Chip` is one speech marker as a compact chip with the button that removes it.
The fill writes the name and sets the close icon.
`Theme.Unit.Row` is one lexical unit in the unit menu.
It is drawn as a category row is, with a check on the held one.
`Theme.Freedom.Row` is one freedom in the freedom menu, drawn the same way.
The fill names the unit or freedom through its key and wires the click.

## `PSlateTemplate.xaml`

### `Theme.Slate.Row`

It shows one offered tag as its text alone, since a tag carries no count of its own.
The row is a plain surface rather than a button, so the list beneath it keeps its own selection.
The editor's fill writes the runs.

### `PSlateRow`

The row answers the pointer going down rather than coming up.
A popup takes the window's activation the moment it is pressed, which ends the caret's focus.
Answering on the press writes the tag and empties the caret before that happens.
So the text the user was typing is not committed a second time as a new tag.

### `PSlateLead`

The pieces of the text are runs of one line rather than three blocks.
A run carries no spacing of its own, so the text reads as one word broken only by weight.

## Stamp

Commit: `c2f335c331fe`
- `PMarkerTemplate.xaml` `c329adf8082f603e`
- `PSlateTemplate.xaml` `d51a395a909b399b`
