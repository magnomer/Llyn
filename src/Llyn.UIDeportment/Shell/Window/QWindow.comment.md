# QWindow.cs
Hash: `3228fbc2b82f5942`

## `public partial class QWindow`

The window itself: what happens when it opens and when it closes.
The window is the veneer's `PWindow` sheet, pulled by contract ID and held in `_qWindowSurface`.
The class is no window itself, so every window call goes through that field.
Each panel is a control with its own markup and its own behaviour.
This file only puts them to work on the workspace the engine opened.
It stops them at the end.

## `public QWindow(CAtelier atelier)`

Opens the window on Conduct's root, already built over an engine bound to a workspace that opened.
No engine and no posture is constructed or named here.
The host builds the engine and the root, and hands the root over.
The window keeps the root, and builds the GUI-only posture beside it with no handle on it.
The window disposes its posture and closes the root when it closes, and the bootstrap disposes the engine on exit.
The envoy is built over the loaded window and this class, so that window owns every question.
It paints the chrome, introduces every part, then makes its one gate call, `CAtelierOpen`, with its envoy.
The opening hands the posture its workspace path, and the stored geometry is placed after it.

## `private readonly QEstablishment _qEstablishment;`

Drives the status bar, pulled from the window under the contract ID `PEstablishment`.

## `private readonly QInput _qInput;`

Drives the input panel, pulled from the window under the contract ID `PInput`.
It is built here beside the other drivers and wired only when the window introduces it.

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

## `private readonly QSettings _qSettings;`

Drives the settings panel, pulled from the window under the contract ID `PSettings`.

## `internal Window QWindowSurface => _qWindowSurface;`

The loaded window, reached by the bootstrap to show it and by panels as the owner of their dialogs.

## `private UserControl QInputSurface`

The named panels are read through the window's name scope.
A panel already moved to the veneer is pulled by its contract ID as a plain page.

## `internal CEnvoy QWindowEnvoy { get; }`

The window's answer to Conduct's user-question port, which the panels ask through.

## `private void QWindowIntroduce()`

Wires the window: every subscription, every panel's attach and every answer to an atelier event.
The window sets itself as the mention attached property, so every mention block below inherits its host.
The loaded window carries the class in its `Tag`, so a control inside can find its host.
The roof, logo, menu lines, caption buttons and tab buttons are subscribed here after the load.
The posture hears the workspace opening first, so its root is set before any panel reloads.
Every panel is attached next, and the status bar after them, so it paints the drafts the panels opened.
The navigation is heard after every panel attaches, so every panel has registered its tab.
Each panel's restore answers `CWorkspaceOpened`, and each duplex wing subscribes its own.
The widths answer `CWorkspaceOpened` too.
They reset first, then apply the stored widths of the workspace opened.
So the open is one gate call, and Conduct decides what shows and in which order.
The widths are registered last, and the open applies them before any tab is shown.

## `private void QWindowIconRefine()`

Each caption glyph strokes with its button's foreground through a binding set here.
Each tab button's icon is resolved here and set as the look's icon, which the tab style's icon part copies.

## `internal CAtelier QWindowAtelier { get; }`

Conduct's root, reached by every panel for its gates.
A panel asks it for a font, a flag, a respelling switch, a setting or a media address.
The static veneer helpers take it, so no panel needs an engine to call them.

## `internal QPosture QWindowPosture { get; }`

The GUI-only posture: the layout, the linked flag and the window bounds.
It is read under the workspace path the window hands it, so a workspace change moves it too.
It holds no Conduct handle, since GUI-only state never reaches Conduct.

## `internal QVolume QWindowVolume { get; }`

The one owner of the shared volume, which every editor and reading view attaches its slider to.
Each editor attaches its player too.
It alone hears the workspace opening and the shared level, so each change reaches Conduct once.

## `internal QWindowScreen QWindowScreen { get; } = new();`

The holder of the one browser environment every Screen shares.
The window owns it because the runtime allows a single environment per process.

## `private void QWindowLayoutAttach()`

Registers the root grid of every tab that has a seam, under the name its widths are stored by.
Each panel wears its loaded markup as a `UserControl`, so the grid is that surface's content.
The duplex panel is left out.
Its two halves are editors sharing the window, not a catalog beside a display.
The stored widths are applied by the open that follows, before any tab is shown, so nothing jumps.

## `private void QWindowClosingObserve(object? sender, CancelEventArgs e)`

Hears the window closing and asks `CAtelierQuitConfirm` once, through the window's envoy.
The answer goes straight to `QWindowClosureRefine`.

## `private void QWindowClosureRefine(CancelEventArgs e, bool confirmed)`

Declining leaves the window open on the form exactly as typed, which is the only place the work still exists.
Geometry is stored only once the close is certain, so a declined close changes nothing.
The close writes at once, with no delay.

## `private void QWindowPostureRefine()`

Hands the posture the workspace path that `CAtelierPathRead` answers.
It answers every workspace opening, the first one inside `CAtelierOpen` at construction.
It is subscribed before every panel and before the layout.
The layout therefore applies the widths of the workspace just opened.

## `private void QFootprintRefine()`

Runs before the window is shown, while position and size can still be set.
The footprint places the stored geometry inside the virtual screen the window reads here.
With nothing stored the window takes its share of the work area and stays centred.
Setting the startup location to manual is required, since the designed default centres the window.

## `private void QFootprintAttach()`

Listens for the window being moved, resized or maximized, once it is loaded.
Nothing is written before then, so the restore does not save what it has just read.
Geometry is written down while the program runs rather than only as it closes.
A killed process, a launcher window shut and a machine turned off all end without closing.

## `private LCapsuleWindow QFootprintRead()`

The window's restored rectangle, which `RestoreBounds` supplies whenever the window is not normal.

## `private void QWindowExitRefine(object? sender, EventArgs e)`

Closes every panel and lets the GUI-only posture go.
It is subscribed before `QWindowExitObserve`, so every panel stops before the session ends.

## `private void QWindowExitObserve(object? sender, EventArgs e)`

Ends the session through `CAtelierClose`, which sweeps the workspace once more and lets the engine's posture go.
A copy of the program left open for days would otherwise collect nothing it wrote after it started.

## Navigation rail

The rail holds four groups parted by transparent gaps, not lines.
The first group is entries: make one, browse all, browse curated.
The second is the classification axes: sound, situation, register, tag.
The third is attestation: example, source, author.
The fourth is the two rime tools, Xiesheng and Yunjing, each with a page of its own.
Dual panel sits with Settings below the divider because it is a mode, not a page.

The rail's buttons and panels are held here, and Conduct's `CNavigation` decides which one shows.
The navigation also keeps the voyage trail the tabs walk.

## Inline notes

### `QFootprintRefine();`

Geometry is applied after the opening has handed the posture its path, while the window is still unshown.

### `QLook.QLookStyleAttach(_qWindowSurface);`

The word menu's dictionary is merged by the window markup itself.
The look sheet scans the window's resources, merged dictionaries included, for the row chrome.
The window fills each realized row and subscribes its click to its own pick.
The list is fed from the window's own collection, and the window's preview keys and deactivation drive the menu.
However the popup closes, the menu hides, so a closed menu keeps no rows for the keys.
The popup itself is declared in the markup beside the panels, once, because the window owns it.

### `_qWindowSurface.Closing += QWindowClosingObserve;`

Closing runs while the window is still up and can be called off.
Closed cannot.
Unsaved text is caught in the first, and the panels are stopped in the second.

### `_qInput.QInputExitRefine();`

Every panel is stopped before the engine goes.
