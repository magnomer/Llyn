# PWindow.xaml.cs

## `public partial class PWindow : Window`

The window itself: what happens when it opens and when it closes. Each panel is a control with its own markup and its own behaviour; this file only puts them to work on the workspace the engine opened, and stops them at the end.

## `public PWindow(LEngine engine)`

Opens the window on `engine`, already built and bound to a workspace that opened. The engine is not constructed here: opening the workspace can fail, and a failure in a window constructor has nowhere to be shown. `LBootstrap` builds it and hands it over; the window owns it from here and disposes it when it closes.

## Inline notes

### `Closing += PWindowClosingHandle;`

Closing runs while the window is still up and can be called off; Closed cannot. Unsaved text is caught in the first, and the panels are stopped in the second.

### `e.Cancel = !PWindowDiscardConfirm();`

Declining leaves the window open on the form exactly as typed, which is the only place the work still exists.

### `PInput.PInputClose();`

Every panel is stopped before the engine goes.
