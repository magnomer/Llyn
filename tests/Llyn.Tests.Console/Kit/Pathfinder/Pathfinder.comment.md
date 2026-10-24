# Pathfinder

Tests of the console path prompt, mirroring `src/Llyn.UIDemeanor/Kit/Pathfinder/`.

## `TPathfinder`

Runs the path prompt inside a window with the fake console and a seam over a few entries.
A folder prompt chooses the folder shown on Enter, and Escape or Cancel answers null.
Tab, Tab and Enter press the first button, and Shift+Tab from the field reaches the buttons.
Shift+Tab then Tab lands in the field again.
Enter on a folder row enters it, Enter on `..\` and Backspace in the list climb.
A file prompt lists the folders and only the wanted suffixes, and typing narrows the list.
Enter in the field accepts a path relative to the folder shown.
A missing file shows an alert and the prompt stays open.
A folder that cannot be read at the start shows its parent with the alert.
An entry that cannot be read keeps the folder shown and shows the alert.
An empty folder lists only `..\`.
A folder name too long for the row is clipped on the left.
A wide file name is clipped by cells.
The help lists the path keys, among them the parent folder.
With no folder given, the prompt starts in the workspace, for the markup file and the folder question both.

## `TPathfinderMedium`

The save prompt, with the formats Llyn Markup and HTML.
Enter in the field saves the offered name with the suffix of the chosen format.
Right changes the format and the suffix, and the list shows only that format's files.
A name typed after Right stays at the end of the name, and Right then Left keeps the typed name.
A suffix typed by the user is kept as typed, and a name without one gets the suffix.
Enter on an empty field asks nothing and saves nothing, and the prompt stays open for a name.
An existing file asks `Replace it?`, Yes saves, and No reopens the prompt.
Escape answers null and still reports the format that was chosen.
`BSPathfinderMediumRemove` strips a known suffix in any case and leaves other text.
`BSPathfinderMediumMove` clamps at both ends.

## `TDrive`

Alt+F1 in the path prompt opens the drive list over it.
Enter on a drive opens that drive's root in the prompt.
Escape keeps the folder.
A drive seam that throws shows no list and no alert.
The help shows the drive legend.

## Stamp

Commit: `78fc0ebfff7c`
- `TDrive.cs` `24a2ced50fe8bc01`
- `TPathfinder.cs` `5fea131179bbb3c3`
- `TPathfinderMedium.cs` `203702d0be1b9c65`
