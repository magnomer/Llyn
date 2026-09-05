# PWindow.xaml.cs

## `public partial class PWindow : Window`

The window itself: what happens when it opens and when it closes.
Each panel is a control with its own markup and its own behaviour.
This file only puts them to work on the workspace the engine opened.
It stops them at the end.

## `public PWindow(LEngine engine)`

Opens the window on `engine`, already built and bound to a workspace that opened.
The engine is not constructed here.
Opening the workspace can fail.
A failure in a window constructor has nowhere to be shown.
`LBootstrap` builds it and hands it over.
The window owns it from here and disposes it when it closes.

## `internal int PWindowLeftover { get; private set; }`

How many held drafts the workspace carried that no open window claims.
It is read once as the window attaches, before any panel starts a draft of its own.
Reading it later would count this session's own work, which no one needs offering back.
The count is held and nothing is shown yet, because the recovery dialog is not built.

## `private void PWindowAttach(LEngine engine)`

Puts every panel to work on the one engine.
The workspace is swept first, so nothing already saved is counted as lost work.
The leftovers are counted next, so the number describes the workspace as it was found.

## `private void PWindowExitHandle(object? sender, EventArgs e)`

Closes every panel, sweeps the workspace once more, and lets the engine go.
Sweeping on the way out as well as on the way in bounds what a long session leaves behind.
A copy of the program left open for days would otherwise collect nothing it wrote after it started.

## Inline notes

### `PWindowStateRestore();`

Geometry is applied before any panel is attached, while the window is still unshown.

### `Closing += PWindowClosingHandle;`

Closing runs while the window is still up and can be called off.
Closed cannot.
Unsaved text is caught in the first, and the panels are stopped in the second.

### `e.Cancel = !PWindowDiscardConfirm();`

Declining leaves the window open on the form exactly as typed, which is the only place the work still exists.

### `PWindowStateSave();`

Geometry is stored only once the close is certain, so a declined close changes nothing.

### `PInput.PInputClose();`

Every panel is stopped before the engine goes.
