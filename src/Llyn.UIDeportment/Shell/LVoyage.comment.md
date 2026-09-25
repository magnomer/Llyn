# LVoyage.cs

## `public sealed class LVoyage`

The trail of records the reader has jumped between, and the buttons that walk it.
It works the way a browser's back and forward do.
A station is one tab paired with the id of the record it shows.
Every tab that shows one record takes part, and every one of them carries the buttons.
The buttons live in each panel's command rail, next to save, and the trail lights them all.
The trail lives for the session and belongs to the navigation, so the engine never hears of it.
Both trails are capped at fifty stations, the oldest dropped first.

## `public LVoyage(UIElement window, LNavigation navigation)`

Keeps the navigation that jumps between tabs.
It hooks the window's key and mouse previews to the shortcuts.

## `public (LTab LVoyageTab, long LVoyageId) LVoyageStationRead()`

The station of the open tab, which comes from the posture.
A tab without a station reader, or an id of zero, says there is nothing to come back to.

## `public void LVoyageRecord()`

Records the station now open.

## `public void LVoyageRecord((LTab LVoyageTab, long LVoyageId) station)`

Pushes the given station onto the past and forgets the future.
A cross-tab jump passes the station it is leaving.
Nothing is recorded when the station has not moved, nor for an empty one.

## `private static void LVoyageStationAdd(LinkedList<(LTab LVoyageTab, long LVoyageId)> trail, (LTab LVoyageTab, long LVoyageId) station)`

Adds one station at the top, dropping the oldest when the trail is full.
An empty station is ignored, so no trail ever lands on nothing.

## `public void LVoyageRetreat()`

Steps back one station, parking the current one on the future.
Each panel's retreat button calls it, as do the keyboard and mouse shortcuts.

## `public void LVoyageAdvance()`

Steps forward one station, the mirror of the retreat.

## `private void LVoyageRun(LinkedList<(LTab LVoyageTab, long LVoyageId)> source, LinkedList<(LTab LVoyageTab, long LVoyageId)> target)`

The one step retreat and advance share, with the two trails swapped between them.
The station parked on the other trail is the standing one, not a station read back afterwards.
The next station is only peeked, and the trails move only after the jump has gone.
The jump goes through the navigation's tab show, which records nothing.
A refused jump leaves both trails as they were, so the station is still there to try again.

## `private void LVoyageKeyHandle(object sender, KeyEventArgs e)`

`Alt+Left` steps back and `Alt+Right` steps forward.
With Alt held the key arrives as a system key, so the arrow is read off `SystemKey`.

## `private void LVoyageMouseHandle(object sender, MouseButtonEventArgs e)`

The two side buttons of a mouse step back and forward, as they do in a browser.

## `private void LVoyageUpdate()`

Enables each tab's pair only while the matching trail has somewhere to go.
Every tab is told, because any of them may be the one on screen.
