# PRepertoire.xaml.cs

## `public partial class PRepertoire : UserControl`

The Repertoire panel: the view of the shared stock of usage contexts itself.
A Situation is independent data owned by nothing, so this panel is not a view of one Entry's contexts.
It holds the host and the engine the browsing side calls, and nothing else.
The browsing behavior lives in `PRepertoireBrowse.cs`, one file per responsibility.

## `internal void PRepertoireAttach(PWindow host, LEngine engine)`

Binds the panel to the window it asks for confirmations and panel switches through.

## `internal void PRepertoireReset()`

Drops the selection and reads the catalog again, for when the workspace underneath changed.

## `internal bool PRepertoireChangeCheck()`

Whether the panel holds a draft the engine says differs from what is stored.
The window asks before anything can leave the panel.
The answer is the engine's, so the panel decides nothing about what counts as a change.

## `internal void PRepertoireClose()`

Closes the popups the panel owns, so neither outlives the window.
