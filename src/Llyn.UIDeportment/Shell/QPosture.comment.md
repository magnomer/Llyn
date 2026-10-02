# QPosture.cs

## `public sealed class QPosture : IDisposable`

The GUI-only state of the main window: its geometry, the panel widths and whether the widths are linked.
A console has none of it, so it lives in Deportment and never reaches Conduct.
It keeps the state through Deportment's Capsule, in a file under the workspace root.
It holds one state record and swaps it whole on every change.
It is handed the workspace root as a plain path and holds no Conduct handle.

## `public event Action? QPostureCleared;`

Raised once the stored widths are dropped, so `QLayout` puts the grids back at their markup widths.

## `public event Action? QPostureLinkedChanged;`

Raised once the linked switch has flipped.
The summary row and the tabs on screen follow in the same call.

## `public void QPostureRootRefine(string root)`

Loads the state stored under `root`, unless it is the root already held.
A move cancels a waiting write meant for the old root.
A session with no workspace folder starts from the default state, which the Capsule answers.
Until the first call the posture holds the default state and writes nothing.

## `public LCapsuleContent QPostureRead()`

The state as it stands, read whole under the gate.

## `public LCapsuleColumn? QPostureColumnRead(string tab)`

The stored widths of one tab, or null when none are stored.

## `public void QPostureWindowDefer(LCapsuleWindow window, bool minimized, int delay)`

Stores the window geometry after `delay` milliseconds, or at once when `delay` is zero.
Every call cancels the write still waiting, so a drag ends in one write.
A minimized window stores nothing, so the maximized flag it had survives a close from the taskbar.

## `public bool QPostureLinkedSave(bool linked)`

Stores whether dragging a panel in one tab sets the same width in every tab, and says whether that changed.
A change then raises `QPostureLinkedChanged`, outside the gate.
The widths themselves are stored per tab either way, so a flip neither moves nor loses any panel.

## `public void QPostureLayoutSave(IEnumerable<LCapsuleColumn> layout)`

Stores the widths of the tabs given.
A width a tab leaves empty keeps what its record had, and every tab not given keeps its record.
Nothing is written while no tab's record moved.

## `public void QPostureLayoutReset()`

Drops the stored width of every tab, keeping the tabs.
It then raises `QPostureCleared`, so the layout on screen follows in the same call.

## `public void Dispose()`

Cancels any write still waiting.

## `private async Task QPostureWindowRun(CancellationTokenSource pending, LCapsuleWindow window, int delay)`

Waits out the delay, then writes unless a later call replaced or cancelled the wait.

## `private void QPostureWindowCancel()`

Cancels and releases the waiting write, if there is one.

## `private void QPostureSave(LCapsuleContent content)`

Holds `content` and writes it, unless it equals the content already held.
A folder that cannot be written, or no folder at all, keeps the state in memory only.
The Capsule itself skips a save with no folder.
Losing a window's size is no reason to interrupt the user.
