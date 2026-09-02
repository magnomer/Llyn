# PExample.xaml.cs

## `public partial class PExample : UserControl`

The Examples panel: the view of the shared stock of sentences itself.
An Example is independent data owned by nothing, so this panel is not a view of one Entry's sentences.
It holds the host and the engine the browsing side calls, and nothing else.
The browsing behavior lives in `PExampleBrowse.cs` and the editing in `PExampleEditor.cs`, one file per responsibility.

## `internal void PExampleAttach(PWindow host, LEngine engine)`

Binds the panel to the window it asks for confirmations and panel switches through.

## `internal void PExampleReset()`

Drops the selection and reads the catalog again, for when the workspace underneath changed.

## `internal bool PExampleChangeCheck()`

Whether the editor is open over modifications nothing has saved yet.
The window asks before anything can leave the panel.

## `internal void PExampleClose()`

Closes the popups the panel owns, so none outlives the window.
