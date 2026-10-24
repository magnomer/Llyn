# Pathfinder

The console's path prompt and its drive list, both bodies of the sublevel box.
They take every disk access as a seam, so the kit reads no file system of its own.

## `BSPathfinder`

One body that asks for a folder, a file to open or a file to save.
The mode follows the arguments.
No suffixes and no medium ask for a folder.
Suffixes ask for a file to open, and a medium asks for a file to save.
The box above it holds the buttons, and the envoy decides what the first button is called.

The body lists the folder through a seam that returns folders and files.
It checks a file through a presence seam and reports the choice through a callback.
A folder that cannot be read leaves the old list and shows the alert `Pathfinder.Unreadable`.

The constructor starts at the folder it was given and climbs to the parent until a folder can be read.
So a missing start still shows a list.
It stops at a root that has no parent, so the climb ends.

The list holds `..\` first, then the folders, then the files that end in a wanted suffix.
Each group is in case-insensitive order.
Every folder entry ends in `\`, and that ending is how the body tells a folder from a file.
Typing narrows the list to the entries that start with the last segment of the text.
The lit row returns to the first on every narrowing or reload.

The field and the list have a key table each, and `BSParleyBodyResume` picks one.
Tab and Down from the field resume in the list, and Shift+Tab from the list resumes in the field.
The box calls `BSParleyBodyResume` when Tab hands the focus to the body, backwards on Shift.
Left and Right change the format in save mode only.
Alt+F1 asks for a drive and opens the one chosen.
Backspace in the list climbs to the parent.

Enter in the field of a folder prompt opens the typed path.
It chooses the path only if the folder could be read.
Enter in the field of a file prompt chooses a file that exists.
A typed folder opens instead, and the field is cleared.
Anything else shows `Pathfinder.Missing`.
Enter on a list row opens a folder row.
It chooses a file row that exists, and shows `Pathfinder.Missing` for one that does not.
In save mode every file row is chosen, since the callback asks about the replacement.

`BSPathfinderAccept` is the first button.
With text in the field it acts as Enter in the field.
A folder prompt with an empty field chooses the folder shown.
A file prompt with an empty field chooses the lit file and ignores a folder row.

The field holds only the name.
The suffix comes from the chosen format.
It is drawn after the field as a tail, and it is added when Enter is pressed.
A format change then changes the tail and the list, and the typed name stays as it is.
Had the suffix lived in the text, every format change would have to rewrite the user's text.

The field begins with the offered name without its known suffix.
`BSPathfinderMediumRemove` strips the longest suffix among the formats, ignoring case.
A name typed with the suffix of the chosen format is kept.
The suffix is added only when the name does not already end in it.
The tail takes its cells from the field, never more than the path row has left.

A named file that exists is chosen through the callback, and the envoy asks there to confirm a replacement.
A refusal leaves the prompt open.
A folder typed in the field opens, as in the other modes.
A name that is no file and no readable folder is taken as a new file name.
It is then chosen.

Enter on an empty field does nothing in save mode.
There is no name to save, and the result would be the bare suffix, a file with no name.
The key is dropped and the prompt stays open.
The user can still type a name or pick a row.
This is held by `PathfinderSave_EmptyFieldEnter_AsksNothing`.

## `BSPathfinderMedium`

The formats a save may take, with one chosen.
It starts at the choice that is marked chosen, or at the first.
It needs at least one choice, or the constructor throws.
`BSPathfinderMediumMove` clamps at both ends, so Left on the first format and Right on the last do nothing.
`BSPathfinderMediumChoice` gives the suffix to the tail, the file filter and the name check.
It also gives the catalog key to the format line.

## `BSPathfinderPath`

Joins a typed path to the folder shown and cleans it.
A rooted text, one that starts with a separator or has a colon second, ignores the folder.
A `..` removes a segment and a `.` or an empty segment is dropped.
A `..` never removes the root, which is the drive, or the server and share of a network path.
A root keeps its trailing `\`.
Both separators are taken, and the result uses `\`.
The result is never read from the disk, so the function is pure.

## `BSDrive`

A body that lists the drives and answers the one chosen.
Up and Down move the lit row, clamped at both ends, and Enter answers it.
It holds no field, so a key outside its table is left to the box.
Its height is one row per drive, and one row when there are none.
The envoy asks for the drives through a seam.
If the seam throws, no list opens and no alert shows.
The drive list is a box of its own over the prompt.
Cancel closes it with no change of folder.

## Stamp

Commit: `78fc0ebfff7c`
- `BSDrive.cs` `3ecc448f2d525097`
- `BSPathfinder.cs` `bc2655a729c8fc70`
- `BSPathfinderMedium.cs` `589df3dc01859487`
- `BSPathfinderPath.cs` `8d844395befa6f45`
