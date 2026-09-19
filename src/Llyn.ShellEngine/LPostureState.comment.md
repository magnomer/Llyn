# LPostureState.cs

## `public sealed record LPostureState(`

How the main window stands: geometry, tab layout, linked panels, open tab, split and volume.
None of it is an engine fact, so it lives beside the settings file as `posture.json` rather than inside it.
The posture holds one of these and swaps it whole on every change.
So a read never sees half a change.

**Parameters**

- `LPostureStateWindow` — The window geometry from the last run, or nothing before a first close.
- `LPostureStateLayout` — The widths, ordering and hidden languages of each tab, one record per tab, or nothing yet.
- `LPostureStateLinked` — Whether dragging a panel in one tab sets the same width in every tab, on by default.
- `LPostureStateMode` — Name of the tab standing open, and nothing before any tab is chosen.
- `LPostureStateSplit` — Whether the open tab shows its editor rather than its read area.
- `LPostureStateVolume` — How loud a stored pronunciation is played, from silence at zero to full at one.
