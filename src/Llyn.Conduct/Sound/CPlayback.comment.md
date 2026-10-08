# CPlayback.cs
Hash: `243c07809f0634fc`

## `public sealed class CPlayback`

The playback of the draft an editor holds: its own recording, each accent row's recording and the tray.
The editor builds it over its desk and media port, so it keeps no copy.

## `public CTimbrePlayback CPlaybackRead()`

The held draft's own recording while its file exists, and whether the playback tray shows.
An empty desk has nothing to play and asks the engine nothing.

## `public Uri? CPlaybackStart(string? audio)`

The user pressed play on the recording `audio`, and the gate answers the address the driver's player opens.
A file gone since the button was painted answers null, and the driver repaints from a fresh read.

## `public Uri? CPlaybackAccentStart(long accent)`

The user pressed play on the accent row `accent`, and the gate answers the address the driver's player opens.
A file gone from disk answers null, and the engine clears it off the row.
The draft bulletin then repaints the row without its play button.
A desk that fills its view answers null, so a render never resolves audio.
