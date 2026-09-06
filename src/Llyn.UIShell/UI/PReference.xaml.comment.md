# PReference.xaml.cs

## `public partial class PReference : UserControl`

The Source panel: the view of the shelf of bibliographic Sources itself.
A Source is independent data owned by nothing, so this panel is not a view of one citation of it.
The visible label is Source and the internal base is Reference, because Source already names a pronunciation source.
It holds the host and the engine the browsing side calls, and nothing else.
The browsing lives in `PReferenceBrowse.cs`, the editing in `PReferenceImprint.cs`, the credits in `PReferenceAuthor.cs`.

## `internal void PReferenceAttach(PWindow host, LEngine engine)`

Binds the panel to the window it asks for confirmations and panel switches through.

## `internal void PReferenceReset()`

Drops the selection and reads the shelf again, for when the workspace underneath changed.

## `internal bool PReferenceChangeCheck()`

Whether the edit area is open over modifications nothing has saved yet.
The window asks before anything can leave the panel.
Credits are not compared, because they are written when they are made rather than on save.

## `internal void PReferenceClose()`

Closes the popups the panel owns, so none outlives the window.
