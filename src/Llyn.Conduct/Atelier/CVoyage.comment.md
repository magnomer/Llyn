# CVoyage.cs
Hash: `a3937490b66ec360`

## `internal sealed class CVoyage`

The trail of records the reader has jumped between.
It works the way a browser's back and forward do.
A station is one tab key paired with the id of the record it shows.
The trail lives for the session and belongs to the navigation, so the engine never hears of it.
Both trails are capped at fifty stations, the oldest dropped first.
Only the navigation holds it, reading the stations from the panels it registered.

## `internal CVoyageState LVoyageRead()`

Whether each trail has somewhere to go, which lights the retreat and advance buttons.

## `internal void LVoyageStationAdd(string tab, long id)`

Pushes the given station onto the past and forgets the future.
A cross-tab jump passes the station it is leaving.
Nothing is recorded when the station has not moved, nor for an empty one.

## `internal bool LVoyageUndo(string tab, long id, Func<string, long, bool> show)`

Steps back one station, parking the standing station on the future.

## `internal bool LVoyageRedo(string tab, long id, Func<string, long, bool> show)`

Steps forward one station, the mirror of the step back.

## `private static void LVoyageTrailAdd(LinkedList<(string LVoyageTab, long LVoyageId)> trail, (string LVoyageTab, long LVoyageId) station)`

Adds one station at the top, dropping the oldest when the trail is full.
An empty station is ignored, so no trail ever lands on nothing.

## `private static bool LVoyageRun(...)`

The one step back and forward share, with the two trails swapped between them.
The station parked on the other trail is the standing one, not a station read back afterwards.
The next station is only peeked, and the trails move only after the jump has gone.
The jump goes through the navigation's `show`, which records nothing.
A refused jump leaves both trails as they were, so the station is still there to try again.
