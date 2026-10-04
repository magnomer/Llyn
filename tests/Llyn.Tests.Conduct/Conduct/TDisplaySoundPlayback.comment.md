# TDisplaySoundPlayback.cs
Hash: `1df60b105975f575`

## `public sealed class TDisplaySoundPlayback`

Covers the sound area's playback gates on a real workspace.
The wing opens the entry.
Where a case must see the player, a fake media port records each file and level handed to it.
It builds its wing and entries through `TDisplaySound.TDisplayWingPrepare` and `TDisplaySound.TDisplayDraftCreate`.

## `public void DisplayPlaybackRead_AccentWithAudioOnly_ShowsTheTrayButNotTheButton()`

An accent row with audio shows the tray, while the missing own recording hides the button.
Nothing open hides both.

## `public void DisplayPlaybackStart_AccentRow_PlaysAtTheLevelAndTheCancelStopsThatPlay()`

The row plays its file and the button plays the shown draft, each at the level handed in.
Nothing open plays nothing, and the cancel stops the latest play's ticket.

## `public void DisplayPlaybackCancel_NothingPlaying_KeepsTheShownEntry()`

Stopping play with nothing playing leaves the shown entry standing.
