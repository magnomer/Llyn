# Pathfinder

Tests of the console path prompt, mirroring `src/Llyn.UIDemeanor/Kit/Pathfinder/`.

## `TPathfinder`

Runs the path prompt inside a window with the fake console and a seam over a few entries.
A folder prompt chooses the folder shown on Enter, and Escape or Cancel answers null.
Tab, Tab and Enter press the first button, and Shift+Tab from the field reaches the buttons.
Shift+Tab then Tab lands in the field again.
Enter on a folder row enters it, Enter on `..\` and Backspace in the list climb.
A file prompt lists the folders and only the wanted suffixes, and typing narrows the list.
A missing file shows an alert and the prompt stays open.
A folder that cannot be read at the start shows its parent with the alert.
When no folder can be read, Choose answers nothing.
An entry that cannot be read keeps the folder shown and shows the alert.
An empty folder lists only `..\`.
A folder name too long for the row is clipped on the left.
A wide file name is clipped by cells.
The help lists the path keys, among them the parent folder.
With no folder given, the prompt starts in the workspace, for the markup file and the folder question both.

`TPathfinderRun` sends each key through `TConsoleFakeSettle`, so a key that draws no frame does not wait.
It takes the loop and the window from the prime callback.

`TPathfinderRun` ends every key script with up to four F10 presses, each sent only while the run is still going.
A key left over after the answer may open a box in the window.
One F10 would then close that box, and the run would wait for a key forever.

## `TPathfinderMedium`

The save prompt, with the formats Llyn Markup and HTML.
Enter in the field saves the offered name with the suffix of the chosen format.
Tab goes from the field to the Format row and then to the list.
The Format row draws `lit` while it holds the focus.
Tab twice reaches the list, where Down and Enter pick a row.
Shift+Tab goes back the same way.
From the list it passes the Format row on its way to the field.
Right in the field leaves the format as it is.
Right on the Format row changes the format and the suffix, and the list shows only that format's files.
With a folder typed in the field, Right on the Format row lists that folder in the new format.
A format change keeps the typed folder in view.
Opening a folder row clears a typed text with a folder part.
It keeps a bare name, so the save lands in that folder.
A name typed after Right stays at the end of the name, and Right then Left keeps the typed name.
A suffix typed by the user is kept as typed, and a name without one gets the suffix.
Enter on an empty field asks nothing and saves nothing, and the prompt stays open for a name.
An existing file asks `Replace it?`, Yes saves, and No reopens the prompt.
Escape answers null and still reports the format that was chosen.
`BSPathfinderMediumRemove` strips a known suffix in any case and leaves other text.
`BSPathfinderMediumMove` clamps at both ends.

## `TPathfinderPath`

How a typed path is joined to the folder shown, through `BSPathfinderPath` and through the prompt.
Enter in the field accepts a path relative to the folder shown, and a typed path with the open button.
The list follows the folder part of the typed text, and Enter on its row reads against that folder.
A typed folder that cannot be read lists only `..\` and shows no alert.
A segment after it narrows that `..\` away like any other row.
A path that starts with one separator takes the drive, or the server and share, of the folder shown.
A drive with a relative path reads against the folder shown on the same drive.
On another drive it reads from that drive's root.
A drive alone is its root.
A `..` stops at a drive root and at a network share, and a `.` is dropped.

## `TDrive`

Alt+F1 in the path prompt opens the drive list over it.
Enter on a drive opens that drive's root in the prompt.
Escape keeps the folder.
A drive seam that throws shows no list and no alert.
The help shows the drive legend.

## `TCapsulePathfinder`

The path prompt as it reads and writes the console Capsule.
It sits apart from `TCapsule` because it drives the prompt, not the wardrobe.
A prompt with no given folder starts in the stored folder.
Accepting it there keeps the stored theme, so each save rewrites only its own field.
A prompt given a folder stores the folder of its accepted answer.
An accepted file stores its parent folder, and the next prompt starts there.

## Stamp

Commit: `c37ca1837ded`
- `TCapsulePathfinder.cs` `3dea711812f94ba1`
- `TDrive.cs` `e779c2a630a70602`
- `TPathfinder.cs` `9ca54c28e8a5109e`
- `TPathfinderMedium.cs` `7c4bccc9a851f992`
- `TPathfinderPath.cs` `5d29d55b63342f9c`
