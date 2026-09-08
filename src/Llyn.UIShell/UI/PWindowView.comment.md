# PWindowView.cs

## `public partial class PWindow`

The window putting the shell back the way the user left it.
The stored view state is read once and applied from here, so no panel reads it for itself.

## `internal void PWindowViewRestore(LWorkspaceState state)`

Applies `state` across the shell.
That is the ordering each browse panel lists by and the Entry each duplex side stands on.
It is also the tab standing open and whether that tab shows its editor.
The orderings are applied before the tab, so the panel that opens lists in the ordering it was left in.
A state naming no tab leaves the window on the tab it opens with.

The same call puts the shell onto a workspace the user has just switched to.
That workspace carries its own view state, and applying it here spares every panel from asking.
