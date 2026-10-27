# Pedigree

Windows tests for the etymology drivers and painters.

## `TEtymologyItem`

The etymology list shows each box as an item, and co-sources nest under the item they feed.
Two sources of one child sit in that child's item, and a root with none has an empty source list.
A second read keeps the item already paired with a box, so the view does not rebuild.
The kept item takes the new box's text.
A box missing from the next read loses its item.
Items pair by key and not by id.
The same id under a new key is a new item, since a borrowed row repeats its entry's id.
These cases need no thread, since the items are plain notifying objects.

## `TLecternEtymology`

The lectern paints one box's face from its row and its chosen relation.
A linked, borrowed box shows its language, its text as a link, its gloss and its relation name.
Its frame stays collapsed, and the dashed trace and the borrowed mark show.
The face is a small template with the named parts the painter looks up, so the fact needs no window.
It runs on its own STA thread, catches any failure there, and asserts that none was caught.

## Stamp

Commit: `9377e657b395`
- `TEtymologyItem.cs` `5fc53474746dc43d`
- `TLecternEtymology.cs` `aec7681891fbf191`
