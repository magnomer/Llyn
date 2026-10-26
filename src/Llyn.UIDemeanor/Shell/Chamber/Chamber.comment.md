# Chamber

The console main window and everything it owns directly.
It mirrors Deportment's `Shell/Window`, so each window piece has the same name and seam on both families.
It holds the window and its pieces, its drawing and the status line.
It also holds the tab navigation.
It stands apart from `Rafter`, which holds the roof menu bar every tab shares.
`Rafter` also holds the bar's menus and the boxes they open.
It stands apart from `Envoy`, which holds the user-question port.

## `BWindow`

The full-screen window, owning a canvas that follows the console size.
It holds the window's state and routes each key, and `BMural` turns that state into the frame.
It delegates the rest, so each piece has one reason to change.
`BWardrobe` holds the look choices, `BDocket` the open boxes, `BKeymapWindow` the key table and `BEstablishment` the status line.
`BMirror` holds the Conduct state the roof shows and the roof built from it.
Two `BLibrary` panels and one `BInput` are built when the window opens with an atelier.
One panel serves the Library tab and one the Favorite tab.
The Favorite panel is the same panel made over the favorite listing.
`BInput` serves the Input tab with a bare editor frame and no list.
The panel is the one named by the tab in front, and the Input tab names none.
Each panel asks whether it is the shown one, and the input asks whether the Input tab is in front.
So the roof's library menus and every key lookup follow the shown panel, never a panel that is hidden.
The layout comes from the `AWindow` blueprint, and the window only fills the parts it names.
The constructor finds `AWindow`, then `ASParley`, which help and the other boxes use.
The mural it builds then finds the parts it draws.
A missing blueprint or part throws at construction, not while drawing.
Below 80 by 24 cells the focus is the notice, which wins over any open box.
With no box open and the roof at the window focus, the focus may change.
The shown panel or the input may name the lectern or editor focus instead.
The window ends the run through `BWindowExit` and holds the atelier and the envoy it was given.
The atelier may be null, when the start was refused and only a box is shown.
The courier exists only with an atelier, so a refused start has no Joplin items.

## `BWindowOpen`

Reads the size and observes it.
With an atelier it builds the two panels.
It subscribes to navigation changes, which tell it which tab is in front.
A tab with no panel leaves no panel shown.
A state without a tab only refreshes the voyage, and the shown panel stays.
It then subscribes to arrivals, has `BMirrorOpen` subscribe the mirrored state, then subscribes to establishment changes.
Each subscription but the arrival one and the workspace-open one comes through `BObserverCreate`, so it runs on the loop's thread.
Work from the loop's thread runs at once, and other work is deferred onto the loop.
A tab arrival closes any open roof menu, so a menu never stays over the new tab.
It opens nothing, since subscribing and opening are two roles.
The caller runs `BWindowAtelierOpen` next.

## `BWindowAtelierOpen`

Opens the atelier with the envoy, when the window has an atelier, after `BWindowOpen` subscribed.
The marshal handed to the atelier comes through `BObserverCreate`, as the subscriptions do.

## `BWindowSizeObserve`

Builds a new canvas with the theme when the size differs from the last one, and marks the frame changed.
A negative size counts as 0, and the first call always builds a canvas.
The new canvas starts blank, so the next sync prints the whole frame.
The canvas takes the wardrobe's ambiguous-width setting, so a wide choice changes how every cell is counted.

## `BWindowBatchObserve`

Draws and syncs the frame once when anything marked it changed since the last batch.
A size, a key and an arrival each mark it.
So do an establishment and a mirrored state change.
It first folds the `AIndex` part when the shown panel or the input holds the whole tab.
So the display side takes the room.
It hands `BMuralDraw` the size, the focus, the roof, the navigation and the courier state.
It also hands the shown panel's undo and redo flags, which exist only while a scribe shows.
It adds the boxes, the panel's texts from `BLibrary` or `BInput`, the glyphs, the marks and its key lookup.
The width measure comes with them.
Without a mark no frame is built, so a burst of keys costs one frame.

## `BWindowKeyObserve`

The key is looked up in the window's table, then the shown panel's, then the input's, at the current focus.
At the lectern or editor focus a key none binds falls back to the same lookups at the window focus.
Before a found row runs, the typed text still pending in a field is applied.
So a key such as save sees the whole text, not one that lags.
The row itself does nothing unless its live state is shown, so a dim row is safe wherever it runs.
At a box or help focus, a key the table lacks goes to the top box.
At the window, roof, lectern and editor focus, such a key goes to the roof.
The frame is marked changed when the roof used the key.
At the window and editor focus, a key no table took and the roof refused goes on.
It goes to the shown panel, else to the input.
The panel sends it to the index or to its editor by the focus.
At the notice focus, every key without a row does nothing.
The frame is marked changed after a key that reaches a box.
Quit goes through the atelier's quit confirmation, and only a yes completes the exit task.
Without an atelier the request quits at once.

## `BWindowParleyConsult`

Hands the box to the docket, which runs it.

## `BWindowHelpShow`

Opens help as a box with one Close button, for the focus that asked.
For each page, help reads the window's rows of that page's focus, then the shown panel's, then the roof's rows.
It adds the rows of the box with the same focus, or else the top box.
Those rows list the body's keys first.
The window page also reads the shown panel's lectern rows, so help lists the reading view's keys.
The window page reads no box, since it is not about one.
The box has the help focus, so its own rows are the keys inside help.

## `BWindowNavigationShow`

Opens the tab chooser as a box and waits until a tab is picked or the box closes.
It selects the tab only if one was picked and the window has not exited meanwhile.
Without an atelier or a navigation state it does nothing.

## `BWindowKeymapFind`

The legend looks in the window's table first, then in the shown panel's and the input's.
At the lectern or editor focus it looks in the panel's and the input's table first.
Then it repeats the search at the window focus.
Then it looks in the roof's, then in the top box's table.

## `BMirror`

The window's copy of the Conduct state the roof shows, and the roof built from it.
It mirrors the navigation, the ledger and the courier state, and each has one writer, its observer.
The roof is built once, and each change hands it newly built menus.
Each change also marks the frame changed through a callback.
So a verdict is computed once on its event and an open menu stays open across it.
The window reads the roof, the navigation and the courier state, and never writes them.
The product menu comes from a callback that receives the courier state reader.
A look choice reaches the window through a restyle callback, which drops the canvas.
The library's menus come from an area callback and stand between the tab menu and `Options`.

## `BMirrorOpen`

Subscribes to navigation, ledger and courier changes, each through `BObserverCreate`.
With a courier it then reads the courier state once through the courier observer, as the GUI courier does.
So the Joplin items show even when the workspace open fails and no courier change comes.
A navigation or courier change gives the roof new menus, since those menus depend on that state.
Each ledger change applies the ledger's texts to the catalog and keeps the ledger state.
It then gives the roof new menus, as the GUI settings do.
Without a courier only navigation and ledger are heard.

It also hears each workspace open directly, since a change runs on the loop from a roof row.

## `BMirrorWorkspaceObserve`

Each workspace open hands the wardrobe the open root, as the GUI posture rereads its root.
So a workspace change mid-run gives the window the new workspace's look.
The roof then gets new menus and the canvas is dropped once, as a look choice does.
The first open names the root the wardrobe already holds, so it changes no choice.

## `BMirrorMenuBuild`

Builds the roof's menus, the product menu, the tab menu, the library's menus and `Options`.
It runs again after any state a menu shows changes, and the roof applies the result.
A look choice gives the roof new menus only when the wardrobe accepted the name.
It then drops the canvas and calls the size observer again.
So the whole window is printed once anew.
Without an atelier the language save does nothing and `Settings…` selects nothing.

## `BKeymapWindow`

The window's own key table, with rows for the window, a box, the notice, help and the roof.
It is built in the constructor, so a refused key fails at construction.
A box keeps its own rows in `BSParleyKeymap`.
F1 opens help at the window, at a box and at the roof.
F10 and Ctrl+C ask to quit at the window, the notice and the roof.
Ctrl+C also asks at a box and in help.
The roof rows close the open menu before they act.
Alt+F1 chooses a tab, and Alt+Left and Alt+Right move back and forward in the tab history.
Each of those rows exists at the window and the roof focus.
The history rows are dim while the voyage has no past or no future.
They are hidden before a navigation state is heard.
Every row is built from callbacks, so the table knows nothing about the window.

## `BWardrobe`

Holds the look the user can change during the run.
That is the theme, the glyph set and the width of ambiguous characters.
A choice answers false for a name it does not know, so a bad name changes nothing.
The theme is replaced whole with its name.
A theme that fails to load leaves the current theme in place.
A theme the user chooses reports its failure, so the theme box shows the reason.
A stored theme that fails is not the user's failure, so it falls back without a report.
That one rule holds at start and on a workspace change.
The glyph set in use is named, and the marks are read from that set's glyphs.
The initial set comes from the glyph set of the `ASParley` child, and is empty when that has none.
The glyph sets it hands the blueprint map every set name to the set in use.
So frames draw the chosen set whatever set they name.
Ambiguous width is `narrow` or `wide`, and it builds the canvas width measure everything else shares.
The choices outlive the run in the console Capsule under the workspace root.
The wardrobe holds that root, and every save goes there.
A choice that succeeds is stored at once, so a crash later in the run keeps it.

## `BWardrobeRootRefine`

Takes a workspace root and applies the look stored under it.
The constructor calls it with the start root.
A root equal to the held one changes nothing.
So a stale stored name is tried once per root.
Each stored name goes through its own choice.
A field the file lacks returns that choice to its default.
A name this build does not know returns that choice to its default too.
A stale name and a missing one mean the same, since neither names a look this build can show.
So the new workspace's look is replaced whole, as Deportment's `QPosture` replaces its content.
Keeping the old value would carry one workspace's look into another.
The default glyph set is the one the constructor was given.
The reset writes nothing, so the new workspace's file stays as it was.
A stored theme equal to the current one is skipped, so the theme is not loaded again.
Saves after it go to the new root alone, so no state merges across workspaces.

## `BWardrobeSave`

Stores one choice and leaves the other stored fields as they are.
It writes nothing when the stored state already holds the choice.
So applying the stored names on start writes no file.
A folder that cannot be written loses the choice silently, and the run goes on with it.

## `BDocket`

The stack of open boxes and the loop that serves them.
A box is put on the stack, and the loop runs nested.
It stops when the box answers, a given task completes, or the window exits.
Queued work keeps running in the nested loop, and a fault there goes to the handler given.
The box then gets Escape.
So it answers its safe index even when the window exits or the task ended the wait.
A box run with a task, such as the tab chooser, leaves the stack at once.
Any other box leaves when its own key answers.
It answers null at once if the window has already exited.
Each change marks the frame changed through a callback.

## `BEstablishment`

The status line, one row under the frames.
Before the workspace has reported an establishment, or without a courier state, it shows only the opening text.
After that it shows the Joplin state on the left, then the courier's last line after a slash.
The line is cut with an ellipsis to the room left.
A first character too wide for the room beside the ellipsis leaves the ellipsis alone.
The room reserves the unsaved mark, so that mark is never pushed out.
The unsaved mark shows only while the establishment has pending changes.
The entry and size tally sits on the right in the `muted` role, joined with a dot.
Dot, ellipsis and pending glyphs come from the marks, with a built-in fallback for each.
All widths are measured, so wide characters keep their columns.

## `BNavigation`

Maps the tab parts of the `ASNavigation` blueprint to their catalog labels and builds what shows them.
A part with no mapping throws at construction.
The tab menu lists the visible tabs in their blueprint groups.
A separator sits between groups, and a radio mark sits on the current tab.
It adds Back and Forward, enabled by the voyage, and a Choose entry that opens the chooser.
Without an atelier or state the menu is empty.
Drawing puts the current tab name at the right end of the roof row.
Back and Forward sit at the foot of the right frame.
While a scribe shows, the same two places hold Undo and Redo, from the flags the window hands in.
A move or step that is not possible draws in the `disabled` role.

## `BNotice`

Draws the small-console notice.
A console narrower than 44 or lower than 8 cells gets the bare text in the `text` role.
Each line starts at the left edge on its own row.
A larger one gets a framed `ANotice` box 42 cells wide, centred, with ` Llyn ` on its top border.
The box height follows the wrapped lines, and lines past the box are cut.
The legend of the notice focus comes in from the caller.

## `BMural`

The window's drawing: it turns the state `BWindow` holds into the texts of one frame.
The window decides and the mural draws, so no drawing code reads or writes window state.
It finds `ALegend` and `AVoyage` in `AWindow` at construction, and builds the notice drawing.
It keeps the tab navigation and the status line, which never change after construction.

## `BMuralDraw`

Draws the small-console notice at the notice focus, and otherwise the whole frame.
The roof, the navigation and the courier state come in at each draw.
So do the undo and redo flags, which reach only the navigation drawing.
So do the panel's texts, which draw after the scaffold and before the status line.
The boxes, the glyphs, the marks, the key lookup and the measure come in too.

## `BMuralFrameBuild`

Places the `AWindow` blueprint for the current size and draws its frames with the glyph set each names.
The panel's texts follow the frames.
The status line, the legend, the roof and the tab drawing come next.
The open boxes draw last, in stack order, so the top box covers the rest.
Only the top box passes its `caret` text on.
So the terminal cursor never lands in a box that another box covers.
The legend is built for the current focus.
The left half is half the width, and the right half takes the rest.
Each half has a rule above its last inner row.
Row 0 is the roof, which the scaffold leaves to `BRoof`.

## `BMuralLegendBuild`

Walks F1 to F10 and draws each key the focus binds and does not hide.
A drawn key has its number and its label.
A key sits in its own slot of 8 cells, so F10 is at column 72.
Under the notice the keys pack from the left instead.
A dim key draws its label in the `disabled` role, and a shown one in the legend theme with `-label`.
The number takes the legend theme with `-number`, so the theme file styles both.

## `BMuralNoticeBuild`

Fills `Window.Small` with the minimum and the actual size, then hands it to `BNotice`.
The legend sits on the last row at column 0.

## Stamp

Commit: `c05429890cec`
- `BDocket.cs` `2f1ab27227907c41`
- `BEstablishment.cs` `f0b09dc75cb89774`
- `BKeymapWindow.cs` `ac57c5af4fc27418`
- `BMirror.cs` `2d5cfafbf7bb9ee3`
- `BMural.cs` `4d0cdafd28a38552`
- `BNavigation.cs` `8d7a242f082a5e6e`
- `BNotice.cs` `7a5f8cc1b48d424d`
- `BWardrobe.cs` `3aa4f791074e68eb`
- `BWindow.cs` `0378c8b4fe45d105`
