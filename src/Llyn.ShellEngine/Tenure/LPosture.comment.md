# LPosture.cs

## `public sealed class LPosture : IDisposable`

How the main window stands: its geometry, tab layout, linked panels, open tab, split and volume.
It is owned by the window, since none of it belongs to one tab.
It stands beside the engine in the shell ring, not inside it, so no engine part carries window posture.
It persists through the posture port the engine hands out, so it never learns where the workspace is.
The core ring no longer holds any of this, so it knows nothing of how wide a pane is.
It listens to the engine as an observer.
A vista bulletin stores the order and filter that vista now holds.
A workspace bulletin reloads the posture of the workspace moved onto.
The window disposes it on exit, which detaches it and lets go of every vista it watched.

## `public LPostureState LPostureRead()`

The posture as it stands, read whole under the gate.

## `public bool LPostureModeMatch(string? mode)`

Whether the tab named is the one stored as standing open.

## `public bool LPostureVolumeMatch(double volume)`

Whether the level given is the one stored, so an echoing slider writes nothing.

## `public LVista LPostureVistaStart(string tab, LSubject? subject, LCatalogOrder fallback, bool blank = false)`

Starts the tab's vista on the order and filter stored under it, and on the stored split.
The deportments reach it through `LWindow`, since the veneer names no posture after plan 13.
A tab with no stored order lists by `fallback`, and one with no stored filter hides nothing.
The vista's starting order and filter are marked, so the first bulletin stores only what moved since.
The vista's editing event is taken here, since only a vista started through the posture is the window's.
The vista is remembered so the event can be given back on dispose.

## `public void LPostureWindowDefer(LWindowState window, bool minimized, int delay)`

Stores the window geometry after `delay` milliseconds, or at once when `delay` is zero.
Every call cancels the write still waiting, so a drag ends in one write.
A minimized window stores nothing, so the maximized flag it had before survives a close from the taskbar.
The shell hands the raw facts here, so no control and no clock of its own decides the write.

## `private async Task LPostureWindowRun(CancellationTokenSource pending, LWindowState window, int delay)`

Waits out the delay, then writes unless a later call replaced or cancelled the wait.

## `private void LPostureWindowCancel()`

Cancels and releases the waiting write, if there is one.

## `public void LPostureVolumeSave(double volume)`

Stores how loud a pronunciation is played, clamped so no caller writes a level the player cannot take.
A level already stored writes nothing, so no shell asks before it saves.
Every view plays through the same level.
A change made in one is the level the next one opens at.

## `public void LPostureModeSave(string mode)`

Stores the name of the tab standing open.

## `public bool LPostureLinkedSave(bool linked)`

Stores whether dragging a panel in one tab sets the same width in every tab, and says whether that changed.
The widths themselves are stored per tab either way, so a flip neither moves nor loses any panel.
No bulletin carries the switch any more, so the caller syncs the tabs itself.

## `public void LPostureLayoutSave(IEnumerable<LLayout> layout)`

Stores the panel widths, ordering and hidden languages of the tabs given.
Each field a tab gives replaces that field of its own record.
A field left empty keeps what the record had.
Every tab not given keeps the record it had.
A drag therefore never drops the ordering a panel chose, and an ordering never drops a dragged width.
Nothing is written while no tab's record moved, so a bulletin that changed nothing costs no file.

## `public void LPostureLayoutReset()`

Drops the stored width of every tab.
Each tab's ordering and hidden languages stay, because the button promises widths and nothing more.

## `public void Dispose()`

Detaches from the engine and drops the editing event of every vista started here.

## `private void LPostureBulletinHandle(LBulletin bulletin)`

The posture's own subscription, attached as a method group when it is made and detached when it is disposed.

A workspace bulletin reloads the posture, since the workspace moved onto carries its own.
A vista bulletin names the vista by id, and the engine says which one still stands under it.

## `private void LPostureVistaSave(LVista vista)`

Compares the vista's order and filter with the mark held for it and stores them only when one moved.
A query or a chosen row moves neither, so typing writes nothing.

## `private void LPostureSplitSave(bool editing)`

Stores the mode a vista was just asked for as the shared split, whether or not the vista itself changed.
Another vista may have moved the split since, so an unchanged ask still writes it back.

## `private static bool LPostureLayoutMatch(LLayout held, LLayout next)`

Two filters compare by the text the catalog writes them in, since the record compares lists by reference.

## `private void LPostureLoad()`

Reads the stored posture through the engine, then the legacy settings key when none is stored.
A vault fault on either read leaves the posture as it stands, so a broken file never resets the window.
A legacy posture that differs from the default is adopted and written under the current key.

## `private bool LPostureChange(Func<LPostureState, LPostureState> change)`

Applies `change` under the gate and writes the result out, saying whether anything moved.
An equal record is not written again.

## `private void LPostureSave()`

Writes the posture through the engine, which records and swallows a vault fault.
