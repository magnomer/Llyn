# PReference.xaml.cs

## `public partial class PReference : UserControl`

The Source panel: the view of the shelf of bibliographic Sources itself.
A Source is independent data owned by nothing, so this panel is not a view of one citation of it.
The visible label is Source and the internal base is Reference, because Source already names a pronunciation source.
It holds the host and the engine the browsing side calls, and nothing else.
The browsing lives in `PReferenceBrowse.cs`, the editing in `PReferenceImprint.cs`, the credits in `PReferenceAuthor.cs`.

## `internal void PReferenceAttach(PWindow host, LEngine engine)`

Binds the panel to the window it asks for confirmations and panel switches through.
It binds its lists and subscribes to the engine, and reads nothing yet.
The window fills the source shelf when it restores the stored ordering.
Every change after that arrives as an announcement, so the panel is current whether or not its tab is in front.

## `internal void PReferenceReset()`

Drops the selection and reads the shelf again, for when the workspace underneath changed.

## `internal bool PReferenceChangeCheck()`

Whether the panel holds work nothing has saved yet.
The window asks before anything can leave the panel.
The engine answers it, so the panel decides nothing about what counts as a change.
Credits are not compared, because they are written when they are made rather than on save.

## `internal void PReferenceClose()`

Detaches the panel from the engine, so a closed panel is never announced to.
Closes the popups the panel owns, so none outlives the window.
