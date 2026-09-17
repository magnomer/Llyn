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
The stored view state is read once here and applied after every panel is attached.
Reading it once is what keeps the panels from each asking the workspace the same question.
The status bar attaches after every panel, so its first reading already sees the drafts the panels opened.

## `internal PLayout PWindowLayout => _pLayout;`

The keeper of panel widths, reached by the settings panel when the user links or unlinks the tabs.

## `private void PWindowLayoutAttach()`

Registers the root grid of every tab that has a seam, under the name its widths are stored by.
The duplex panel is left out.
Its two halves are editors sharing the window, not a catalog beside a display.
The stored widths are applied right after, before any tab is shown, so nothing jumps on the first view.

## `private void PWindowExitHandle(object? sender, EventArgs e)`

Closes every panel, sweeps the workspace once more, and lets the engine go.
Sweeping on the way out as well as on the way in bounds what a long session leaves behind.
A copy of the program left open for days would otherwise collect nothing it wrote after it started.

## Navigation rail

The rail holds four groups parted by transparent gaps, not lines.
The first group is entries: make one, browse all, browse curated.
The second is the classification axes: sound, situation, register, tag.
The third is attestation: example, source, author.
The fourth is the rime table, the one tool with a page of its own.
Dual panel sits with Settings below the divider because it is a mode, not a page.

Above the first group sits the voyage strip.
Its two buttons step back and forward through the records the reader has jumped between.
They are sized to the tab strip and start disabled, there being nowhere to go before a jump.
`PWindowVoyage.cs` keeps the trail and enables them.

## Inline notes

### `PWindowStateRestore();`

Geometry is applied before any panel is attached, while the window is still unshown.

### `PreviewKeyDown += PVoyageKeyHandle;`

The voyage keys and mouse buttons are hooked on the window, beside the mention keys.
Preview events see them before any panel does, so the trail answers from every page.

### `Resources.MergedDictionaries.Add(new PMentionMenuTemplate(this));`

The word menu's rows are drawn by a dictionary that forwards their clicks here.
The list is fed from the window's own collection, and the window's preview keys and deactivation drive the menu.
The popup itself is declared in the markup beside the panels, once, because the window owns it.

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
