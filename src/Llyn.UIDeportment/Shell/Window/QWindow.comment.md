# QWindow.cs
Hash: `fbf784839ec95bcc`

## `public sealed class QWindow`

The window itself: what happens when it opens and when it closes.
The window is the veneer's `PWindow` sheet, pulled by contract ID and held in `_qWindowSurface`.
The class is no window itself, so every window call goes through that field.
The panels belong to `QPanelRoster`, which builds, wires and stops them.
This file only puts the roster to work on the workspace the engine opened.
It stops the roster at the end.

## `public QWindow(CAtelier atelier)`

Opens the window on Conduct's root, already built over an engine bound to a workspace that opened.
No engine and no posture is constructed or named here.
The host builds the engine and the root, and hands the root over.
The window keeps the root, and builds the GUI-only posture beside it with no handle on it.
The window disposes its posture and closes the root when it closes, and the host disposes the engine on exit.
The envoy is built over the loaded window, so that window owns every question.
It builds the mention menu, introduces every part, builds the tab navigation, then makes its one gate call, `CAtelierOpen`.
The menu comes first, so its preview key runs ahead of the chronicle's.
The navigation is built after the panels exist, so its tabs find every panel.
The gate takes its envoy and a marshal over the dispatcher of the thread that builds the window.
So a failed layout save shows on the window's thread, whichever thread saved.
The opening hands the posture its workspace path, and the stored geometry is placed after it.

## `private readonly Window _qWindowSurface;`

The loaded `PWindow` sheet, pulled by contract ID.

## `private readonly QFootprint _qFootprint;`

Places the stored window bounds and saves them when the window moves or closes.

## `private readonly QCaption _qCaption;`

Drives the caption buttons of the window.

## `private readonly QHeadquarter _qHeadquarter;`

Drives the product menu of the window.

## `private readonly QEstablishment _qEstablishment;`

Drives the status bar, pulled from the window under the contract ID `PEstablishment`.

## `private readonly QPanelRoster _qPanelRoster;`

The fourteen panels and their shared widths, built over the loaded window and the posture.
The window hands it facets only, so no panel holds the window.

## `internal Window QWindowSurface => _qWindowSurface;`

The loaded window, reached by the bootstrap to show it.

## `internal CEnvoy QWindowEnvoy { get; }`

The window's answer to Conduct's user-question port, which the panels ask through.

## `private void QWindowIntroduce()`

Wires the window: every subscription, the roster's introduce and the status bar's attach.
The loaded window carries the browser environment holder in its `Tag`, so a Screen inside can find it.
The roster is handed only the facets its panels use: the atelier, the envoy, the volume and the mention menu.
The caption and the product menu introduce themselves here.
The posture hears the workspace opening first, so its root is set before any panel reloads.
The roster introduces every panel next, then registers its seamed grids.
The status bar attaches after them, so it paints the drafts the panels opened.
So the open is one gate call, and Conduct decides what shows and in which order.

## `internal CAtelier QWindowAtelier { get; }`

Conduct's root, handed to every panel for its gates.
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

## `internal QMentionMenu QWindowMention { get; }`

The one word menu of the window, handed to every panel that draws a sentence.
Panels open it and subscribe its sense pick, so the window itself relays nothing.

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

## `private void QWindowExitRefine(object? sender, EventArgs e)`

Closes the roster and the status bar, and lets the GUI-only posture go.
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

The rail's buttons and panels are held by `QNavigation`, and Conduct's `CNavigation` decides which one shows.
The navigation also keeps the voyage trail the tabs walk.

## Inline notes

### `_qFootprint.QFootprintRefine();`

Geometry is applied after the opening has handed the posture its path, while the window is still unshown.

### `QLook.QLookStyleAttach(_qWindowSurface);`

The word menu's dictionary is merged by the window markup itself.
The look sheet scans the window's resources, merged dictionaries included, for the row chrome.
`QMentionMenu` fills and drives the menu itself.

### `_qWindowSurface.Closing += QWindowClosingObserve;`

Closing runs while the window is still up and can be called off.
Closed cannot.
Unsaved text is caught in the first, and the panels are stopped in the second.
