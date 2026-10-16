# Window

The main window and everything it owns directly.
It holds the window itself, the user-question port and the product menu.
It also holds the tab navigation, the panel roster, the browser environment and the drawn caption.
It stands apart from `Frame`, which holds furniture shared by every tab, because these pieces own the window's life.

## `QWindow`

The window itself: what happens when it opens and when it closes.
It is the veneer's `PWindow` sheet, pulled by contract ID and held in `_qWindowSurface`.
The class is no window, so every window call goes through that field.
The panels belong to `QPanelRoster`, and this file only puts the roster to work on the opened workspace.

### Construction

It opens on Conduct's root, already built over an engine bound to a workspace that opened.
No engine is constructed or named here, and neither is the engine's posture.
The host builds the engine and the root and hands the root over.
The window builds the GUI-only posture beside it, with no Conduct handle on it.
The window disposes its posture and closes the root when it closes, and the host disposes the engine on exit.
The envoy is built over the loaded window, which owns every question while no other app window is active.
Order matters.
The mention menu comes first, so its preview key runs ahead of the chronicle's.
Then every part is introduced, then the tab navigation is built.
The navigation comes after the panels exist, so its tabs find every panel.
Then comes the one gate call, `CAtelierOpen`.
The gate takes the envoy and a marshal over the dispatcher of the building thread.
So a failed layout save shows on the window's thread, whichever thread saved.
The loaded window carries the browser environment holder in its `Tag`, so a Screen inside can find it.
The posture hears the workspace opening before any panel, so its root is set before any panel reloads.
The roster introduces every panel next, then registers its seamed grids.
The status bar attaches last, so it paints the drafts the panels opened.
So the open is one gate call, and Conduct decides what shows and in which order.
Stored geometry is applied after the opening has handed the posture its path, while the window is still unshown.

### Shared parts

The atelier is handed to every panel for its gates.
A panel asks it for a font, a flag, a respelling switch, a setting or a media address.
The static veneer helpers take it, so no panel needs an engine to call them.
The volume owner alone hears the workspace opening and the shared level, so each change reaches Conduct once.
Each editor attaches its slider and its player to it.
The browser environment holder is the window's, because the runtime allows one environment per process.
The word menu is the window's one, handed to every panel that draws a sentence.
Panels open it and subscribe its sense pick, so the window relays nothing.

### Closing

Closing runs while the window is up and can be called off.
Closed cannot.
Unsaved text is caught in the first, and the panels are stopped in the second.
Closing asks `CAtelierQuitConfirm` once through the envoy.
Declining leaves the window open on the form exactly as typed, which is the only place the work still exists.
Geometry is stored only once the close is certain, so a declined close changes nothing.
The close writes at once, with no delay.
Exit closes the roster and the status bar and lets the posture go.
It is subscribed before the session ends, so every panel stops before the engine goes.
Ending the session through `CAtelierClose` sweeps the workspace once more and lets the engine's posture go.
A copy of the program left open for days would otherwise collect nothing it wrote after it started.
The posture gets the workspace path on every opening, the first inside `CAtelierOpen`.

### Look sheet

The word menu's dictionary is merged by the window markup itself.
The look sheet scans the window's resources, merged dictionaries included, for the row chrome.
`QMentionMenu` fills and drives the menu itself.

### Navigation rail

The rail holds four groups parted by transparent gaps and not lines.
The first group is entries: make one, browse all, browse curated.
The second is the classification axes: sound, situation, register, tag.
  The three flashcard tabs, study, deck and scoreboard, close this group.
The third is attestation: example, source, author.
The fourth is the two rime tools, Xiesheng and Yunjing, each with a page of its own.
Dual panel sits with Settings below the divider because it is a mode and not a page.

## `QEnvoy`

The GUI's answer to Conduct's user-question port.
It puts up each box, dialog and window, worded from the key Conduct chose.
The main window owns every box while no other window of the app is active.
The owner is the active window of the app on the calling thread, else the main window.
So a failure raised under the modal print window sits above it and not behind it.
A call off the app's thread, or with no app, falls back to the main window.
The window's posture keeps the print window's size, and the engine's store keeps the print setting.
A failure box shows the localized headline and the notice's localized detail.
Any label and path go on lines of their own.
It asks Conduct nothing further.
A failure with nothing more to say shows the headline alone.
An empty detail line would read as a missing message.
A shared-record delete shows its tally on a line of its own under a question mark.
A union question names the dropped record first and the kept one second, joined by an arrow.
The fold reads that way.
The leave dialog offers three answers, because a leaving user often means to keep the word.
A cancelled file, markup or folder dialog answers null, which the gate reads as nothing to do.
The save dialog lists the gate's formats in order and starts on the chosen one.
Its one-based filter index picks the medium from that same list, so the filter and the format cannot drift apart.
Dialog titles stay here, since only a windowed chooser has one.
The flashcard folder dialog opens in the folder the gate hands it and answers the chosen path as picked.
The media dialog is an open dialog, titled by the gate's key and filtered to the gate's suffixes.
  It offers no all-files choice, so the phone gets only a file kind it plays.
The customs window keeps its declared rows in the gate and hands none back.
Every Print command reaches the print window through the print gate, so every command shows the preview.
The coinage dialog answers the wording as typed, or null on retreat.
Its mint button stays dark on a blank wording, so a blank one never comes back.

## `QHeadquarter`

The product menu the mark opens and what its two lines do.
The mark is the only opener the window has that is not a caption button.
A click toggles the menu, so a second click puts it away.
The mark sits inside the roof, and the roof drags the window on a left press.
Left unhandled, opening the menu would start a drag under it.
It decides nothing, so each handler is a refine of the menu or the window.
The menu is read through the loaded window's name scope.
The about line puts the menu away first.
A message box over an open popup would leave the popup behind it.
Conduct chooses the version line's wording key.
The product's name titles the notice, as every notice of the window is titled.
The build number is the one the assembly carries, since Conduct is framed away from reflection.
`version.json` at the repository root is the only place a version is written.
`Directory.Build.props` reads it and stamps every assembly at compile time.
Only the first three parts are shown, because the fourth is always zero.
Exit asks the window to close and does not end the process.
That is the caption close's path, so unsaved work is still offered back and no gate stands here.

## `QNavigation`

The window's tab buttons and the jumps panels ask for.
Conduct's `CNavigation` decides which tab shows and keeps the voyage.
This driver paints what it raises.
The tabs are held here only with their buttons and panels.
Each panel's rail lights its own voyage buttons, so no tab hands a hook over.
It knows the loaded window and the navigation and no window driver.
The window builds it after every panel attaches, so every panel has registered its tab.
Each tab is a plain Veneer `Button` read through the window's name scope.
Its chosen look is the look's `Chosen` cue, so the tab style's accent rows apply.
A click goes to the navigation's select gate as the key of the button pressed.
An unknown button sends nothing.
The navigation raises the change it made, so nothing is painted at the click.
Before any change arrives, the tab whose panel the markup shows is marked.
A change hides the tabs the session does not offer, marks the open tab and shows only its panel.
A state with no open tab leaves the panels and the marked tab as they are.
The icons are resolved here and set as the look's icon, which the tab style copies.
`Alt+Left` steps back and `Alt+Right` steps forward, each through one voyage gate.
With Alt held the key arrives as a system key, so the arrow is read off `SystemKey`.
The key binding is the GUI's own, so it stays here.
The two side buttons of a mouse step back and forward, as in a browser.

## `QPanelRoster`

The window's seventeen panels and the shared widths of their seams.
It builds each panel driver over its markup, wires it, and stops it when the window closes.
It takes facets only and never `QWindow`, so no panel reaches the window through it.
The panels are input, taxonomy, study, deck, scoreboard, tenor, repertoire, corpus, reference, library and phonology.
The rest are Xiesheng, Yunjing, favorite, duplex, guild and settings.
Each is pulled from the window under its contract ID, as a plain page once moved to the veneer.
Most drivers are built at construction and wired only when the roster introduces it.
The study and scoreboard drivers wire nothing before their introduce, so the roster builds them there.
  It keeps no field for them, as `QDeck` keeps none for `QCue`.
The settings panel takes the layout and the posture, since it resets the widths.
The layout is the one owner of the seam widths, shared by every seamed tab and the settings panel.

### Introduce

Every panel gets only the facets it uses.
Then each panel that reloads answers `CWorkspaceOpened`, and each duplex wing subscribes its own.
The widths answer `CWorkspaceOpened` too, after every panel.
They reset first, then apply the stored widths of the workspace opened.

### Attach and close

Attach registers the root grid of every tab that has a seam, under the name its widths are stored by.
Each panel wears its loaded markup as a `UserControl`, so the grid is that surface's content.
The duplex panel is left out.
Its two halves are editors sharing the window and not a catalog beside a display.
The stored widths are applied by the open that follows, before any tab is shown, so nothing jumps.
Close stops every panel that holds something to stop.
The duplex, settings, study, deck and scoreboard panels are not stopped here.
The window calls it before the session ends, so every panel stops before the engine goes.

## `QWindowScreen`

The one browser environment the window owns, handed to every Screen that plays a web address.
The runtime allows a single environment per process, so the task is made once and shared.
It starts on the first ask, with autoplay allowed.
The user has already asked by the time play is called.
A failed start drops the task, so a later attempt is a real attempt.

## `QCaption`

The window frame the program draws for itself, since the system one is off.
It covers the caption buttons and dragging the window by its roof.
Minimize and close go to the system commands.
Each caption glyph strokes with its button's foreground through a binding set at introduce.
The maximize button and a double press on the roof share one toggle between maximized and restored.
A drag on a maximized window restores it under the pointer first.
The pointer keeps its share of the restored width.
It stays at most half the roof's height below the top edge.
Device pixels become layout units first, so a scaled display still lands the window under the pointer.

## Stamp

Commit: `121691f60459`
- `QCaption.cs` `a94cfc236ad84fe3`
- `QEnvoy.cs` `ce0505b274893fb3`
- `QHeadquarter.cs` `77429c1948810be1`
- `QNavigation.cs` `e0176c86a339c36b`
- `QPanelRoster.cs` `7d2d7adf6ae30d16`
- `QWindow.cs` `2a23b08f87db77ad`
- `QWindowScreen.cs` `eb76a072e3a88c02`
