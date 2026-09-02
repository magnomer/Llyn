# PSound.xaml.cs

## `public partial class PSound : UserControl`

The sound panel as a control: what it is made of, and when it starts and stops.
Browsing itself lives in the file beside this one.
That is the search, the ordering, the inventory, the read-only display and the editor beside it.

## `internal void PSoundAttach(PWindow host, LEngine engine)`

Puts the panel to work on `engine`, the workspace the window opened.
Nothing is read yet.
The panel fills itself the first time it is shown.

## `internal void PSoundReset()`

Puts the panel back on the workspace open now.
Nothing is selected, the editor is closed, and the inventory is re-read.

## `internal bool PSoundChangeCheck()`

Whether the editor holds modifications that have not been stored.
That is what the window asks before the workspace changes or the program closes.

## `internal void PSoundClose()`

Stops the panel: the editor is shut down and the shared display releases its playback.

## Inline notes

### `PArticulation.PArticulationAttach(PEditor.PPronunciation);`

The aid is given the one editable pronunciation field this panel owns.
It is named once here rather than looked up whenever a character is chosen.
