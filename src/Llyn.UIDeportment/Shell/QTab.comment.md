# QTab.cs

## `internal sealed record QTab(`

One tab of the main window as the driver sees it.
It only pairs the navigation's key with the Veneer parts that paint it.
The navigation owns each tab's questions, station, restore and arrival.
The record holds no rule and sends nothing.

**Parameters**

- `QTabMode` — Key the navigation names the tab by, in its state and its select gate.
- `QTabButton` — Navigation button that opens the tab.
- `QTabPanel` — Panel the tab shows.

## `public Action<bool, bool>? QTabVoyage { get; init; }`

The panel's refine that lights its back and forward buttons from the navigation's voyage state.
A missing hook means the tab shows no voyage buttons.
