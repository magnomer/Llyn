# Fold

This folder holds the tests of the engine's card, reflex list and box fold seams.

## `TFold.cs`

It covers the engine's card fold seam for Meaning and Collocation cards.
A card is folded, read back per entry, and unfolded through `LEngine` relays alone.
Both card kinds are covered, since each kind keeps its marks in a table of its own.
A fold names its card by kind and id, and a read returns those pairs.
A meaning whose id equals a collocation's folds only the meaning, so the collocation table stays empty.
The per-entry read returns only that entry's folded cards, so two entries keep apart.
An entry update dropping a card removes its mark, while the kept card stays folded.
An id at or below zero, as an unsaved card carries, must write nothing and throw nothing.
The fold bulletin is raised on the entry, so other open views of it re-read their folds.
The "More readings" mark of an entry's reflex list is opened, closed and read back.
Closing it removes its row.
Two entries keep that mark apart, and an entry delete drops its row.
Accepted opening and closing writes raise the fold bulletin on their entry.
`TFoldEntryCreate` saves an entry with two Meanings and one Collocation, then loads it back.
The loaded draft carries the stored card ids the fold calls need.
`TFoldBox` saves its entries through it too, so both fold suites share one entry shape.

## `TFoldBox.cs`

It covers the engine's box fold seam for the Fanqie and Script boxes of the entry editor.
A box is opened, closed and read back per entry through `LEngine` relays alone.
Theories exercise both box kinds, which have separate tables.
Closing a box removes its row from that table.
Opening one box leaves the other box and the reflex fold closed, and opening the reflex fold opens neither box.
Two entries keep apart, so no entry shares a box state with another.
An entry delete drops the rows of both boxes.
An id at or below zero, as an unsaved entry carries, must write nothing and throw nothing.
The fold bulletin is raised on the entry, so other open views of it re-read their folds.

## Stamp

Commit: `9377e657b395`
- `TFold.cs` `ddd303528bc4fa47`
- `TFoldBox.cs` `c0b078c7f91c8dee`
