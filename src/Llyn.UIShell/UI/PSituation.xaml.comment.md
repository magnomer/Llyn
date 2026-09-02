# PSituation.xaml.cs

## `public partial class PSituation : UserControl`

The Situations panel: the view of the shared stock of usage contexts itself.
A Situation is independent data owned by nothing, so this panel is not a view of one Entry's contexts.
It holds the host and the engine the browsing side calls, and nothing else.
The browsing behavior lives in `PSituationBrowse.cs`, one file per responsibility.

## `internal void PSituationAttach(PWindow host, LEngine engine)`

Binds the panel to the window it asks for confirmations and panel switches through.

## `internal void PSituationReset()`

Drops the selection and reads the catalog again, for when the workspace underneath changed.

## `internal bool PSituationChangeCheck()`

Whether the editor is open over modifications nothing has saved yet.
The window asks before anything can leave the panel.

## `internal void PSituationClose()`

Closes the popups the panel owns, so neither outlives the window.
