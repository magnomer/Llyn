# LTab.cs

## `public sealed record LTab(`

One tab of the main window as the deportment sees it.
One record per tab replaces the parallel tables the window kept before.
The navigation owns each tab's questions, station, restore and arrival.
The record holds only the voyage buttons.

**Parameters**

- `LTabMode` — Name the posture stores the open tab under.
- `LTabButton` — Navigation button that opens the tab.
- `LTabPanel` — Panel the tab shows.

## `public Action<bool, bool>? LTabVoyage { get; init; }`

Tells the tab whether it can go back and forward.
A missing hook means the tab shows no voyage buttons.
