# PWindow.cs

## `public partial class PWindow`

The window itself: what happens when it opens and when it closes.
The window is the veneer's `PWindow` page, pulled by contract ID and held in `_pWindowSurface`.
The class is no window itself, so every window call goes through that field.
Each panel is a control with its own markup and its own behaviour.
This file only puts them to work on the workspace the engine opened.
It stops them at the end.

## `public PWindow(CAtelier atelier)`

Opens the window on Conduct's root, already built over an engine bound to a workspace that opened.
No engine and no posture is constructed or named here.
Opening the workspace can fail.
A failure in a window constructor has nowhere to be shown.
The host builds the engine and the root, and hands the root over.
The window keeps the root, and builds the panel factory and the GUI-only posture over it.
The window disposes its posture and closes the root when it closes, and the bootstrap disposes the engine on exit.
The envoy is built over the loaded window and this class, so that window owns every question.
It paints the chrome, places the stored geometry, introduces every part, then makes its one gate call, `CAtelierOpen`.

## `private readonly QEstablishment _qEstablishment;`

Drives the status bar, pulled from the window under the contract ID `PEstablishment`.

## `private readonly QTaxonomy _qTaxonomy;`

Drives the taxonomy panel, pulled from the window under the contract ID `PTaxonomy`.

## `private readonly QTenor _qTenor;`

Drives the tenor panel, pulled from the window under the contract ID `PTenor`.

## `private readonly QLibrary _qLibrary;`

Drives the library panel, pulled from the window under the contract ID `PLibrary`.

## `private readonly QFavorite _qFavorite;`

Drives the favorites panel, pulled from the window under the contract ID `PFavorite`.

## `private readonly QDuplex _qDuplex;`

Drives the duplex panel, pulled from the window under the contract ID `PDuplex`.

## `internal Window PWindowSurface => _pWindowSurface;`

The loaded window, reached by the bootstrap to show it and by panels as the owner of their dialogs.

## `private PInput PInput`

The named panels are read through the window's name scope.
A panel already moved to the veneer is pulled by its contract ID as a plain page.

## `internal CEnvoy PWindowEnvoy { get; }`

The window's answer to Conduct's user-question port, which the panels ask through.

## `private void PWindowIntroduce()`

Wires the window: every subscription, every panel's attach and every answer to an atelier event.
The window sets itself as the mention attached property, so every mention block below inherits its host.
The loaded window carries the class in its `Tag`, so a control inside can find its host.
The roof, logo, menu lines, caption buttons and tab buttons are subscribed here after the load.
The flag store is subscribed first, so it empties before any panel reloads on an open.
Every panel is attached next, and the status bar after them, so it paints the drafts the panels opened.
The navigation is heard after every panel attaches, so every panel has registered its tab.
Each panel's restore answers `CWorkspaceOpened`, and the duplex answers `CWorkspaceStateOpened`.
So the open is one gate call, and Conduct decides what shows and in which order.
The widths are registered and applied last, before any tab is shown.

## `private void PWindowIconRefine()`

Each caption glyph strokes with its button's foreground through a binding set here.
Each tab button's icon is resolved here.

## `internal PLayout PWindowLayout => _pLayout;`

The keeper of panel widths, reached by the settings panel when the user links or unlinks the tabs.

## `internal CAtelier PWindowAtelier { get; }`

Conduct's root, reached by every panel for its gates.
A panel asks it for a font, a flag, a respelling switch, a setting or a media address.
The static veneer helpers take it, so no panel needs an engine to call them.

## `internal QForge PWindowForge { get; }`

The factory every panel builds its deportment through.

## `internal QPosture PWindowPosture { get; }`

The GUI-only posture: the layout, the linked flag and the window bounds.
It is read under the atelier's workspace path, so a workspace change moves it too.

## `private void PWindowLayoutAttach()`

Registers the root grid of every tab that has a seam, under the name its widths are stored by.
Each panel wears its loaded markup as a `UserControl`, so the grid is that surface's content.
The duplex panel is left out.
Its two halves are editors sharing the window, not a catalog beside a display.
The stored widths are applied right after, before any tab is shown, so nothing jumps on the first view.

## `private void PWindowClosingObserve(object? sender, CancelEventArgs e)`

Hears the window closing and asks `CAtelierQuitConfirm` once, through the window's envoy.
The answer goes straight to `PWindowClosureRefine`.

## `private void PWindowClosureRefine(CancelEventArgs e, bool confirmed)`

Declining leaves the window open on the form exactly as typed, which is the only place the work still exists.
Geometry is stored only once the close is certain, so a declined close changes nothing.
The close writes at once, with no delay.

## `private void PFootprintRefine()`

Runs before the window is shown, while position and size can still be set.
The footprint places the stored geometry inside the virtual screen the window reads here.
With nothing stored the window takes its share of the work area and stays centred.
Setting the startup location to manual is required, since the designed default centres the window.

## `private void PFootprintAttach()`

Listens for the window being moved, resized or maximized, once it is loaded.
Nothing is written before then, so the restore does not save what it has just read.
Geometry is written down while the program runs rather than only as it closes.
A killed process, a launcher window shut and a machine turned off all end without closing.

## `private LCapsuleWindow PFootprintRead()`

The window's restored rectangle, which `RestoreBounds` supplies whenever the window is not normal.

## `private void PWindowExitRefine(object? sender, EventArgs e)`

Closes every panel and lets the GUI-only posture go.
It is subscribed before `PWindowExitObserve`, so every panel stops before the session ends.

## `private void PWindowExitObserve(object? sender, EventArgs e)`

Ends the session through `CAtelierClose`, which sweeps the workspace once more and lets the engine's posture go.
A copy of the program left open for days would otherwise collect nothing it wrote after it started.

## Navigation rail

The rail holds four groups parted by transparent gaps, not lines.
The first group is entries: make one, browse all, browse curated.
The second is the classification axes: sound, situation, register, tag.
The third is attestation: example, source, author.
The fourth is the rime table, the one tool with a page of its own.
Dual panel sits with Settings below the divider because it is a mode, not a page.

The rail's buttons and panels are held here, and Conduct's `CNavigation` decides which one shows.
The navigation also keeps the voyage trail the tabs walk.

## Inline notes

### `PFootprintRefine();`

Geometry is applied before any panel is attached, while the window is still unshown.

### `_pWindowSurface.Resources.MergedDictionaries.Add(_pMentionMenuTemplate);`

The word menu's rows are drawn by a dictionary that forwards their clicks here.
The window fills each realized row and registers the row chrome with the look sheet.
The list is fed from the window's own collection, and the window's preview keys and deactivation drive the menu.
The popup itself is declared in the markup beside the panels, once, because the window owns it.

### `_pWindowSurface.Closing += PWindowClosingObserve;`

Closing runs while the window is still up and can be called off.
Closed cannot.
Unsaved text is caught in the first, and the panels are stopped in the second.

### `PInput.PInputClose();`

Every panel is stopped before the engine goes.
