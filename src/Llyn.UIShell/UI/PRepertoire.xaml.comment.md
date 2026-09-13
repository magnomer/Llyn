# PRepertoire.xaml.cs

## `public partial class PRepertoire : UserControl`

The Repertoire panel: the view of the shared stock of usage contexts itself.
A Situation is independent data owned by nothing, so this panel is not a view of one Entry's contexts.
It holds the host and the engine the browsing side calls, and nothing else.
The browsing behavior lives in `PRepertoireBrowse.cs`, one file per responsibility.

## `internal void PRepertoireAttach(PWindow host, LEngine engine)`

Binds the panel to the window it asks for confirmations and panel switches through.
It binds its lists and subscribes to the engine, and reads nothing yet.
It attaches the entry display and editor to the same host and engine, so an Entry is read and written.
The editor's change notice drives `PRepertoireStore`, as it drives the save button of the tenor panel.
The window fills the situation catalog when it restores the stored ordering.
Every change after that arrives as an announcement.
The panel is current whether or not its tab is in front.

## `internal void PRepertoireReset()`

Drops the selection and reads the catalog again, for when the workspace underneath changed.

## `internal bool PRepertoireChangeCheck()`

Whether the panel holds a draft the engine says differs from what is stored.
The window asks before anything can leave the panel.
The answer is the engine's, so the panel decides nothing about what counts as a change.
The entry editor answers while it is in front, and the Situation editor otherwise.

## `internal bool PRepertoireDraftFinish(bool store)`

Ends whichever draft is in front, committing it or discarding it.
The entry editor finishes its own draft, and the Situation editor finishes its own.

## `internal void PRepertoireClose()`

Detaches the panel from the engine, so a closed panel is never announced to.
Closes the popups the panel owns, so neither outlives the window.
