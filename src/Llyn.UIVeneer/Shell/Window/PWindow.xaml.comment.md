# PWindow.xaml.cs

## `public partial class PWindow : Window`

The window itself: what happens when it opens and when it closes.
Each panel is a control with its own markup and its own behaviour.
This file only puts them to work on the workspace the engine opened.
It stops them at the end.

## `public PWindow(LWindow window, Action<string> workspaceOpener)`

Opens the window on its deportment, already built over an engine bound to a workspace that opened.
No engine and no posture is constructed or named here.
Opening the workspace can fail.
A failure in a window constructor has nowhere to be shown.
`LBootstrap` builds the engine, the posture and the deportment, and hands the deportment over.
The window disposes the deportment when it closes, and the bootstrap disposes the engine on exit.
The deportment is set as the mention attached property on the window, so every mention block below inherits it.
`workspaceOpener` is the bootstrap's own workspace change, since only it may build a rig for a new folder.

## `internal void PWindowWorkspaceChange(string path)`

Moves the user onto the workspace at `path` through the bootstrap's delegate.
The settings panel calls it, so no panel names the factory or the pointer.

## `private void PWindowAttach()`

Puts every panel to work on the window deportment, which builds each panel's deportment over the one engine.
The workspace is swept first, so nothing already saved is counted as lost work.
The recordings are swept next, while no draft is held, so a file no draft or entry names goes.
The leftovers are counted next, so the number describes the workspace as it was found.
The stored view state is read once here and applied after every panel is attached.
Reading it once is what keeps the panels from each asking the workspace the same question.
The status bar attaches after every panel, so its first reading already sees the drafts the panels opened.
The navigation is built after every panel attaches.

## `internal PLayout PWindowLayout => _pLayout;`

The keeper of panel widths, reached by the settings panel when the user links or unlinks the tabs.

## `internal LWindow PWindowDeportment => _lWindow;`

The window deportment, reached by every panel.
A panel asks it for a font, a flag, a respelling switch or a media address.
It also answers the posture: the volume, the layout, the open tab and the window bounds.
The static veneer helpers take it, so no panel needs an engine to call them.

## `private void PWindowLayoutAttach()`

Registers the root grid of every tab that has a seam, under the name its widths are stored by.
The duplex panel is left out.
Its two halves are editors sharing the window, not a catalog beside a display.
The stored widths are applied right after, before any tab is shown, so nothing jumps on the first view.

## `private void PWindowExitHandle(object? sender, EventArgs e)`

Closes every panel, sweeps the workspace once more, lets the posture go, and lets the engine go.
Sweeping on the way out as well as on the way in bounds what a long session leaves behind.
A copy of the program left open for days would otherwise collect nothing it wrote after it started.

## Navigation rail

The rail holds four groups parted by transparent gaps, not lines.
The first group is entries: make one, browse all, browse curated.
The second is the classification axes: sound, situation, register, tag.
The third is attestation: example, source, author.
The fourth is the rime table, the one tool with a page of its own.
Dual panel sits with Settings below the divider because it is a mode, not a page.

The rail's buttons are handed to `LNavigation`, which lights the chosen one.

## Inline notes

### `_lFootprint.LFootprintRestore();`

Geometry is applied before any panel is attached, while the window is still unshown.

### `Resources.MergedDictionaries.Add(new PMentionMenuTemplate(this));`

The word menu's rows are drawn by a dictionary that forwards their clicks here.
The list is fed from the window's own collection, and the window's preview keys and deactivation drive the menu.
The popup itself is declared in the markup beside the panels, once, because the window owns it.

### `Closing += PWindowClosingHandle;`

Closing runs while the window is still up and can be called off.
Closed cannot.
Unsaved text is caught in the first, and the panels are stopped in the second.

### `PInput.PInputClose();`

Every panel is stopped before the engine goes.
