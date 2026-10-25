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

The prompt keeps two folders.
The sight is the folder the prompt is in, and typed text resolves against it.
The preview is the folder the list shows, read from the folder part of the typed text.
So `Archive\wa` lists `Archive` narrowed by `wa`, and the folder line shows `Archive`.
Enter on a row and Backspace in the list read against the preview.
Had the typed text resolved against the preview, `Archive\wa` would become `Archive\Archive\wa`.
The preview is replaced whole on every edit, so the list and its folder never disagree.
A typed folder that cannot be read lists only `..\`, narrowed like any other row.
It shows no alert, since the user is still typing.
A reload keeps the folder part of the text, so a format change keeps the typed folder in view.
Opening a folder clears a text that has a folder part.
Its last segment only filtered the rows, so it is no name.
A bare name with no separator stays, so a save keeps its name across folders.
With no folder part the preview holds no folder, and `BSPathfinderFolderPreview` reads the folder shown.

The field, the Format row and the list have a key table each.
The body swaps between them as one is current.
The box calls `BSParleyBodyResume` when Tab hands the focus to the body.
It lands on the field, or on the list when backwards on Shift.
The Format row is reached only by Tab inside the body, and it draws lit while its table is current.
In save mode Tab goes from the field to the Format row and then to the list.
Shift+Tab goes back the same way.
Without a format, Tab goes from the field straight to the list.
Down from the field goes to the list.
Left and Right change the format on the Format row only.
Enter on the Format row acts as Enter in the field.
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
A folder prompt with an empty field acts as Enter in the field too.
So it chooses the folder shown only if that folder can be read.
When no folder could be read at the start, the button chooses nothing.
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

## `BSPathfinderEntries`

Reads the rows of one folder through the listing seam.
That is `..\`, the folders, and the files that end in a wanted suffix.
The wanted suffix is the chosen format's in save mode, and the asked suffixes otherwise.
The read narrows its rows to those that start with the typed segment, ignoring case.
A navigation reads with an empty segment, so it keeps every row.
Narrowing is part of what the read answers, so it lives in the reader and not in the body.
A listing that throws answers null, and the caller decides what that means.
A navigation then shows the alert.
A typed folder then lists `..\`, narrowed by the same test as the read.
So a segment `x` under an unreadable folder lists nothing.

## `BSPathfinderMedium`

The formats a save may take, with one chosen.
It starts at the choice that is marked chosen, or at the first.
It needs at least one choice, or the constructor throws.
`BSPathfinderMediumMove` clamps at both ends, so Left on the first format and Right on the last do nothing.
`BSPathfinderMediumChoice` gives the suffix to the tail, the file filter and the name check.
It also gives the catalog key to the format line.

## `BSPathfinderPath`

Joins a typed path to the folder shown and cleans it.
A text with a drive and a separator, or a network path, ignores the folder.
A text that starts with one separator takes the root of the folder shown.
That root is its drive, or its server and share.
The disk would otherwise read it on the drive of the process.
A drive with a relative path reads against the folder shown when both are on one drive.
The folder shown stands in for the drive's current folder, as Windows keeps one per drive.
On another drive such a path reads from that drive's root, and a drive alone is always its root.
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

Commit: `8c403d1df768`
- `BSDrive.cs` `bbc1ca4283878f41`
- `BSPathfinder.cs` `445cf6f14d0f17f6`
- `BSPathfinderEntries.cs` `2dfd992b55afaeb9`
- `BSPathfinderMedium.cs` `589df3dc01859487`
- `BSPathfinderPath.cs` `981c839a70e016b1`
