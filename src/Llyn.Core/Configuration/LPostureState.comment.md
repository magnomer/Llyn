# LPostureState.cs
Hash: `ea33d7f7b2e30456`

## `public sealed record LPostureState(`

How the session stands in every medium: each tab's listing, the open tab, the split and the volume.
Window geometry and panel widths are GUI-only, so the GUI driver keeps them in its own Capsule.
None of it is an engine fact, so it lives beside the settings file as `posture.json` rather than inside it.
The posture holds one of these and swaps it whole on every change.
So a read never sees half a change.

**Parameters**

- `LPostureStateLayout` — The ordering and hidden languages of each tab, one record per tab, or nothing yet.
- `LPostureStateMode` — Name of the tab standing open, and nothing before any tab is chosen.
- `LPostureStateSplit` — Whether the open tab shows its editor rather than its read area.
- `LPostureStateVolume` — How loud a stored pronunciation is played, from silence at zero to full at one.
