# Shell

The program's start and the shell pieces no single panel owns.
It holds the startup dialogs, remembered geometry and GUI state, and the helpers every panel shares.
It stands apart from the panels because nothing here belongs to one panel.
Subfolders hold the frame, the screens and the window.

## `QBootstrap`

The start of the program, with every dialog a start can raise and the fault handlers.
The application is running already, and the class reaches it through `Application.Current` alone.
The host builds the engine and hands each step in as a seam, so no engine type is named.
The engine is a type parameter for the same reason.
A failed step shows its dialog and the rest of the run is never called.
So nothing here calls `Shutdown` while the program is starting.
The host only constructs and wires, and the decision to continue lives here.

### Order of the start

The dispatcher context goes in first, before any window is built.
The host builds the main window before the dispatcher runs, and WPF installs no context until then.
An await raised while the window is built would otherwise resume on the thread pool.
On a new empty workspace the flag download is such an await.
The speaker and wing lists then changed off the UI thread and ended the process.
The theme applies next, then the visual-state handlers, before any window loads.
Attaching builds palette brushes, and a brush built before its color never takes it.
A failed theme stops before the build, and a failed engine stops before the rest.

### Theme and catalog

The theme step applies the default catalog, the theme colors and the shared control resources.
The catalog would have translated the failure message, so that one message stays inline in English.
It is the documented emergency fallback.
The stored language is applied before any window or notice, so the rescue notice speaks it too.
A stored language the interface does not carry leaves English in place.
A catalog that fails to load is reported and English stays.

### Engine build

An unwritable folder must reach the user as a message naming the folder, not as a crash in a constructor.
A corrupt database or a file from a newer build no longer stops the launch.
`LDoctor` sets it aside inside the engine and the rescue notice reports it afterwards.
A database another program holds is not set aside, and the busy seam lets the message say so.
Conduct words the notice from the busy answer, and the consult only shows it.
The key comes from Conduct, and no window exists yet to own the dialog.

### Rescue notice

Conduct answers no key when nothing was set aside, and then nothing shows.
The program launches either way.
The message names the kept file, so the user can see nothing was thrown away.

### Faults

Fault handlers attach once the engine exists to record what they catch.
A fault before that is a failed start, which its own dialog reports.
The audit stays silent until the host attaches the engine's audit.
An `async void` fault lands at the dispatcher.
It is written to the audit log and marked handled, so the program stays up.
Otherwise it would end the process with unsaved drafts still open.
The notice names the log, so the user can hand over what happened.
A task fault nobody awaited reaches no dispatcher, so the stray handler is the only place it is seen.
It is recorded and marked observed.
After a fault with no window showing, the program ends instead of lingering unseen.
A fault while the main window is first shown leaves it unopened, and an unopened window never closes.
Such a process held the installed executable and blocked the next build.

## `QEnsignImage`

Resolves a language pack's cached SVG flag into a frozen drawing and keeps the drawings by key.
`LEnsign` owns the asked key, the resolved path and the age a moved workspace outran.
The store is held for the program, because every tab lists the same languages.
The store is also the lock, because every reader and writer touches it.
A malformed or unknown flag becomes no image, so each surface shows its neutral globe.
A `language/variety` key reads a variety's flag the same way.
Every accent and index row reads the store directly, so no row is handed a lookup.

### Workspace moves

The window subscribes it once, before any panel, so it answers each open before any panel does.
On each open it throws every kept drawing away.
The engine forgot its paths when the workspace moved, so a reloading surface asks afresh.
So a flag for a language the new workspace lacks never lingers.
A reload still in flight at the open refills the store once it lands.

### Drawing

A file that does not read is answered with nothing and handed to `delete` with what the reader threw.
The engine keeps a locked or refused file and deletes one whose content was bad.
The file is a cache the workspace can fetch again, and a truncated one would otherwise fail every launch.
The drawing is made when the fill lands and not when a row asks.
So a row never waits on a file.
The engine calls it outside its lock, so parsing holds no lock.
The drawing stays on the shell's thread, where the fill was awaited.
A language already asked comes back in no row, so a warm load draws nothing.
The commit stores each drawing inside the cache's lock, only while the fill's workspace is current.
So a workspace move cannot slip between the engine's check and the store.

## `QFontFace`

Puts a language pack's declared typography onto the surfaces that show its words.
The editor field and the reading view get the same record, so a headword looks the same in both.
A part the pack leaves out is cleared, so the theme's own value stands.
A text box and a text block share one attached property from `TextElement`.
Conduct has read the font already, so a block handed its font asks nothing more.

### Baseline

A headword is pinned by its baseline and not by the middle of its line box.
Each family has its own ascent and descent, so centring shifted the letters as the language changed.
Readers expect stroke bottoms at one height across languages, as Word aligns text.
The top margin fills up to `QFontBaseline`.
The surface must be top-aligned inside a row of fixed minimum height.
It runs after the font is put on, because it reads the family just set.

### Glyph, example and gloss

The glyph font goes into the resources of the row's list, never onto the list itself.
Setting it on the list once leaked into the label and pushed every reading row sideways in the editor alone.
Only the glyph value reads those keys, so the label keeps the theme face and the label column measures alike.
The editor field and the reading chips read the same two keys, so both draw one size in one face.
A part the pack leaves out has its key removed, so `PThemeGlyph.xaml` defaults stand.
Example lines are drawn inside item templates, so they are reached through resources.
Each area reads the font in its own entry's language, so no driver hands a language back.
The gloss under an example also carries its slant.
`CFontSlantTheme` maps to nothing, so the theme's upright stands.

## `QFootprint`

Where the window was and how big it was, carried from one run to the next.
The posture holds it, so a workspace opens the way its owner left it.
`QFootprintShare` is the work-area share of a first launch, because no designed size fits every desktop.
A first launch takes that share, never below the minimum size, and stays centred.
`QFootprintDelay` is 700 ms of rest after a move before the posture writes.
A drag raises a change for every pixel, and one write per pixel would be a file write per pixel.

### Restore

It runs before the window is shown, while position and size can still be set.
Stored geometry is clamped into the current virtual screen and against the minimum size.
A gone monitor or a saved size larger than the desktop must not leave the window unreachable.
Stored geometry sets the startup location to manual, since the designed default centres the window.
Whether the window was maximized is kept.

### Save

Listening begins only once the window is loaded, so the restore does not save what it has just read.
Geometry is written while the program runs and not only at close.
A killed process, a closed launcher and a machine turned off all end without closing.
The restored rectangle is stored, never the maximized one, so restoring returns to a usable size.
`RestoreBounds` supplies it whenever the window is not normal.
A minimized window is dropped by the posture, and an empty or degenerate rectangle leaves the previous geometry.
A certain close writes at once.

## `QLinkChip`

One committed link inside a card's Translation field, copied from a ready Conduct target.
It holds plain values only, so no Conduct record rides a caret or a leaf.
The id names another Entry and is the only saved part.
Headword and language are display state, fixed when the chip is built.
A link whose target should change is closed and written again, never edited in place.
The flag comes from `QEnsignImage` and is null while it has not loaded.
A chip built before the flags loaded reads the flag again and announces the find.
Its view reloads the image in place, so the chip keeps its slot.
A chip that already has its flag is left alone.

## `QMarkdownFace`

Draws a Markdown note as WPF elements, one per ready block Conduct hands over.
It holds no rule about the dialect, only how each block looks on screen.
Conduct parsed the blocks and the atelier opens a followed link, so the helper names no parser and no shell.
Colors come from theme brushes, so a theme change recolors a drawn note.
A missing theme brush falls back to gray.
Refreshing empties the target and fills it in reading order.
The panel hears every followed link through one routed handler, added once however often it refreshes.
A refresh replaces the atelier remembered in `QMarkdownAtelier`, which is dropped with the panel.
A list item is a grid with its mark in a fixed lead column, indented one step per level.
Numbers count up while consecutive items stay on one level and restart otherwise.
The gap above a block depends on its kind, so list items sit close and headings stand off.
A plain string paints as one paragraph, since a key's wording carries no Markdown.
Only an absolute http or https target becomes a hyperlink, so a note cannot launch anything else.
A hyperlink carries only its address and starts no process itself.

## `QObserver`

The shell's end of Conduct's announcements and streamed steps.
It names no engine record, so a caller names the record it attaches.
A surface hands over what to run and never implements a contract itself.
So a panel keeps its bulletin response private.
The engine takes a delegate, so the wrapped delegate is what a surface attaches and later detaches.
It also moves the call to the surface's own thread.
An engine call made off the UI thread announces on the thread that made it.
An import runs on a worker, and a lookup step arrives from a source's worker.
Their announcements would otherwise reach a panel where it cannot touch a control.
The response runs now when the call already stands on the right thread.
Otherwise it is queued there and the call returns, so the engine is never held for a redraw.
A driver that attaches on the UI thread needs no surface to find the dispatcher.

## `QPosture`

The GUI-only state of the main window and its print window.
It holds geometry, panel widths, the linked switch and the print window's size.
A console has none of it, so it lives in Deportment and never reaches Conduct.
It is kept through the Capsule in a file under the workspace root.
It holds one state record and swaps it whole on every change.
It gets the root as a plain path and holds no Conduct handle.
Until the first root it holds the default state and writes nothing.
A root change loads the stored state and cancels a waiting write meant for the old root.
A folder that cannot be written, or none, keeps the state in memory only.
Losing a window's size is no reason to interrupt the user.

### Window writes

Every window write cancels the waiting one, so a drag ends in one write.
A zero delay writes at once.
A minimized window stores nothing, so the maximized flag survives a close from the taskbar.

### Widths

The linked switch makes a drag in one tab set the same width in every tab.
Widths are stored per tab either way, so a flip neither moves nor loses any panel.
A real change raises `QPostureLinkedChanged` outside the gate.
A saved layout keeps what a record had for any width left empty.
Every tab not given keeps its record.
Nothing is written while no tab's record moved.
A reset drops every stored width but keeps the tabs.
It raises `QPostureCleared`, so `QLayout` puts the grids back at their markup widths.
The print window's size is stored at its close, and the setting itself lives in the engine's store.

## `QSplice`

Joins a freshly built row list onto the rows a list control already shows.
When every row still shows the same values, only the chosen marks are copied across.
So choosing a row moves a mark on two rows and keeps every container and the scroll.
Any other difference clears and refills the list whole.
The match and the mark are the item's own, so the helper never reads a logic value.
No row is moved or overwritten in place, because the walker treats that as reordering a store.
The item builder holds no rule.
A map from an engine record to a Conduct shape belongs in Conduct.

## `QTab`

One tab of the main window as the driver sees it.
It pairs the navigation's key with the Veneer parts that paint it.
The navigation owns each tab's questions, station, restore and arrival.
The record holds no rule and sends nothing.

## Stamp

Commit: `c2f335c331fe`
- `QBootstrap.cs` `e0acb96d04b6b1b8`
- `QEnsignImage.cs` `bd809993ba4a4397`
- `QFontFace.cs` `affd158188696d07`
- `QFootprint.cs` `7fa0031e0f667eb8`
- `QLinkChip.cs` `961fb2f6e76128ff`
- `QMarkdownFace.cs` `a8b1a38c9f5de60f`
- `QObserver.cs` `7cd84c14bc937a2b`
- `QPosture.cs` `9516255b716aecbb`
- `QSplice.cs` `5aa311caba5bae78`
- `QTab.cs` `f157f791ac4de0f3`
