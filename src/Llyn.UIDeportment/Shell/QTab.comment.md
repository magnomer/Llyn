# QTab.cs
Hash: `f157f791ac4de0f3`

## `internal sealed record QTab(string QTabMode, FrameworkElement QTabButton, FrameworkElement QTabPanel)`

One tab of the main window as the driver sees it.
It only pairs the navigation's key with the Veneer parts that paint it.
The navigation owns each tab's questions, station, restore and arrival.
The record holds no rule and sends nothing.

**Parameters**

- `QTabMode`: Key the navigation names the tab by, in its state and its select gate.
- `QTabButton`: Navigation button that opens the tab.
- `QTabPanel`: Panel the tab shows.
