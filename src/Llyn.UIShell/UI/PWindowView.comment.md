# PWindowView.cs

## `public partial class PWindow`

The window putting the shell back the way the user left it.
The stored view state is read once and applied from here, so no panel reads it for itself.
The Entry each duplex side stands on comes from the workspace row.
Everything that names only panels comes from the settings file, one layout record per tab.

## `internal void PWindowViewRestore(LWorkspaceState state)`

Applies `state` and the settings' layout records across the shell.
Every browse panel takes a vista started here, which reads its own order and filter from the tab's layout record.
Each panel's designed ordering is named at the call, since the record holds nothing before a choice.
The yunjing panel takes two, since its two columns keep two orderings under two records.
A tab whose middle column lists entries under a chosen record takes a child vista for that column as well.
The child vista carries the chosen entry, which the display reads.
It is also the Entry each duplex side stands on.
It is also the tab standing open and whether that tab shows its editor.
The vistas are handed over before the tab, so the opening panel lists in the ordering it was left in.
A settings file naming no tab leaves the window on the tab it opens with.
The same call puts the shell onto a workspace the user has just switched to.
That workspace carries its own settings file, and starting the vistas here spares every panel from asking.
