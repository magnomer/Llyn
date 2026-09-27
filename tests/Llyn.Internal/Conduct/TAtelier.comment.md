# TAtelier.cs

## `public sealed class TAtelier`

Covers Conduct's atelier over a real posture on the fake rig, with no window and no database.
The open view saves and matches, and a fresh engine reads full volume and no split.
A volume step plays at once, and only a settled level reaches the stored posture.
A second atelier over the same engine reads the stored posture.
A level held only in memory therefore shows as unwritten there.
A workspace change moves the engine and then writes the pointer.
A folder that fails leaves the engine where it stood and writes no pointer.

## `private static LMediaPort TAtelierMediaCreate(List<double> played)`

A media port that records every level handed to the player.
