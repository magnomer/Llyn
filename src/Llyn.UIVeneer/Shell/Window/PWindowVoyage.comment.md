# PWindowVoyage.cs

## `public partial class PWindow`

The trail of records the reader has jumped between, and the buttons that walk it.
It works the way a browser's back and forward do.
A station is one panel mode paired with the id of the record it shows.
Every panel that shows one record takes part, and every one of them carries the buttons.
The window also keeps the station it stands on, so a move knows which place it is leaving.
The buttons live in each panel's command rail, next to save, and the window lights them all.
The trail lives for the session and is the window's alone, so the engine never hears of it.
Both stacks are capped at fifty stations, the oldest dropped first.

## `private (string PVoyageMode, FrameworkElement PVoyagePanel, Func<long> PVoyageReader)[] PVoyagePanelRead()`

The panels that take part, each with its mode name and the reader that says which record it shows.
The station reader and the station shower agree through this one table.

## `private (string PVoyageMode, long PVoyageId) PVoyageStationRead()`

The station of the panel now visible.
An id of zero, or a panel outside the table, says there is nothing to come back to.

## `internal void PVoyageRecord()`

Pushes the station the window was standing on onto the past and forgets the future.
It is called after a record is shown, by a panel's own row click and by every cross-panel jump.
The standing station then becomes the one now shown.
Nothing is recorded while the trail itself is being walked.
Nothing is recorded when the station has not moved, nor for an empty one left behind.

## `private static void PVoyageStationAdd(Stack<(string PVoyageMode, long PVoyageId)> trail, (string PVoyageMode, long PVoyageId) station)`

Pushes one station, dropping the oldest when the trail is full.
A stack has no bottom to trim, so the kept part is rebuilt.

## `private bool PVoyageStationShow((string PVoyageMode, long PVoyageId) station)`

Walks to one station through the same jump a chip would make, and says whether the jump went.
A jump is refused when the panel being left will not let go of unsaved work.
The sailing flag is raised so that jump records nothing.
It is lowered in `finally`, so a failed jump does not silence the trail for good.

## `internal void PVoyageRetreatRun()`

Steps back one station, parking the current one on the future.
Each panel's retreat button calls it, as do the keyboard and mouse shortcuts.

## `internal void PVoyageAdvanceRun()`

Steps forward one station, the mirror of the retreat.

## `private void PVoyageRun(Stack<(string PVoyageMode, long PVoyageId)> source, Stack<(string PVoyageMode, long PVoyageId)> target)`

The one step retreat and advance share, with the two trails swapped between them.
The station parked on the other trail is the standing one, not a station read back afterwards.
The next station is only peeked, and the trails move only after the jump has gone.
A refused jump leaves both trails as they were, so the station is still there to try again.
An empty current station is not parked, so the other trail never lands on nothing.

## `private void PVoyageKeyHandle(object sender, KeyEventArgs e)`

`Alt+Left` steps back and `Alt+Right` steps forward.
With Alt held the key arrives as a system key, so the arrow is read off `SystemKey`.

## `private void PVoyageMouseHandle(object sender, MouseButtonEventArgs e)`

The two side buttons of a mouse step back and forward, as they do in a browser.

## `private void PVoyageUpdate()`

Enables each panel's pair only while the matching stack has somewhere to go.
Every panel is told, because any of them may be the one on screen.
