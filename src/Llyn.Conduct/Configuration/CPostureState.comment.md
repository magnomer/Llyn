# CPostureState.cs

## `public sealed record CPostureState(`

The window's posture, as the window view and its parts read it.
The controller copies it from the engine's posture, so no driver names the engine's state.

**Parameters**

- `CPostureStateWindow`: the stored window geometry, null when none is stored.
- `CPostureStateLinked`: whether the panels share their column widths.
- `CPostureStateSplit`: whether the editor shows beside the display.
- `CPostureStateVolume`: the playback volume between zero and one.
