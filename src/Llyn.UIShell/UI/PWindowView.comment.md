# PWindowView.cs

## `public partial class PWindow`

The window putting the shell back the way the user left it.
The stored view state is read once and applied from here, so no panel reads it for itself.
The Entry each duplex side stands on comes from the workspace row.
Everything that names only panels comes from the settings file, one layout record per tab.

## `internal void PWindowViewRestore(LWorkspaceState state)`

Applies `state` and the settings' layout records across the shell.
That is the languages each browse panel hides and the ordering it lists by.
It is the Entry each duplex side stands on.
It is also the tab standing open and whether that tab shows its editor.
The orderings are applied before the tab, so the panel that opens lists in the ordering it was left in.
A tab with no layout record, or one naming no ordering, opens on the ordering it was designed with.
A settings file naming no tab leaves the window on the tab it opens with.
The same call puts the shell onto a workspace the user has just switched to.
That workspace carries its own settings file, and applying it here spares every panel from asking.

## `private static LCatalogFilter PWindowFilterRead(Dictionary<string, LLayout> layout, string tab)`

The languages one tab hides, or none when no record names any.

## `private static LCatalogOrder PWindowOrderRead(Dictionary<string, LLayout> layout, string tab, LCatalogOrder fallback)`

The ordering one tab lists by, or `fallback` when no record names one.
Each panel's designed ordering is named at the call, since the record holds nothing before a choice.
