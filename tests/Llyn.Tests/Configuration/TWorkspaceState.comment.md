# TWorkspaceState.cs

## `public sealed class TWorkspaceState`

Covers the view state the shell stores per workspace.
That is the ordering each browse panel lists by, the tab standing open and its split.
The Entry each duplex side shows is covered too.
Each panel's ordering is pushed on its own.
The suite drives one push at a time and reads the whole state back.
Two pushes in one session must both stand, which is what a whole-record write would break.
A workspace written before the ordering columns existed must still open, listing as it always did.
