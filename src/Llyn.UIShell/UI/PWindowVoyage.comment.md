# PWindowVoyage.cs

## `public partial class PWindow`

The trail of records the reader has jumped between, and the two buttons that walk it.
It works the way a browser's back and forward do.
A station is one panel mode paired with the id of the record it shows.
Only the five panels that show one record take part: Library, Repertoire, Taxonomy, Corpus, Tenor.
The trail lives for the session and is the window's alone, so the engine never hears of it.
Both stacks are capped at fifty stations, the oldest dropped first.

## `private (string PVoyageMode, FrameworkElement PVoyagePanel, Func<long> PVoyageReader)[] PVoyagePanelRead()`

The panels that take part, each with its mode name and the reader that says which record it shows.
The station reader and the station shower agree through this one table.

## `private (string PVoyageMode, long PVoyageId) PVoyageStationRead()`

The station of the panel now visible.
An id of zero, or a panel outside the table, says there is nothing to come back to.

## `private void PVoyageRecord()`

Pushes the station being left onto the past and forgets the future.
It is called by every cross-panel jump, after the leaving panel has agreed.
Nothing is recorded while the trail itself is being walked.
Nothing is recorded for an empty station, nor twice for the same one.

## `private static void PVoyageStationAdd(Stack<(string PVoyageMode, long PVoyageId)> trail, (string PVoyageMode, long PVoyageId) station)`

Pushes one station, dropping the oldest when the trail is full.
A stack has no bottom to trim, so the kept part is rebuilt.

## `private bool PVoyageStationShow((string PVoyageMode, long PVoyageId) station)`

Walks to one station through the same jump a chip would make, and says whether the jump went.
A jump is refused when the panel being left will not let go of unsaved work.
The sailing flag is raised so that jump records nothing.
It is lowered in `finally`, so a failed jump does not silence the trail for good.

## `private void PVoyageRetreatRun()`

Steps back one station, parking the current one on the future.

## `private void PVoyageAdvanceRun()`

Steps forward one station, the mirror of the retreat.

## `private void PVoyageRun(Stack<(string PVoyageMode, long PVoyageId)> source, Stack<(string PVoyageMode, long PVoyageId)> target)`

The one step retreat and advance share, with the two trails swapped between them.
The next station is only peeked, and the trails move only after the jump has gone.
A refused jump leaves both trails as they were, so the station is still there to try again.
An empty current station is not parked, so the other trail never lands on nothing.

## `private void PVoyageHandle(object sender, RoutedEventArgs e)`

The click of either button, told apart by the sender.

## `private void PVoyageKeyHandle(object sender, KeyEventArgs e)`

`Alt+Left` steps back and `Alt+Right` steps forward.
With Alt held the key arrives as a system key, so the arrow is read off `SystemKey`.

## `private void PVoyageMouseHandle(object sender, MouseButtonEventArgs e)`

The two side buttons of a mouse step back and forward, as they do in a browser.

## `private void PVoyageUpdate()`

Enables each button only while its stack has somewhere to go.
