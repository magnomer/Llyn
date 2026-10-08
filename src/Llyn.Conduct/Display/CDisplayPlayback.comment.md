# CDisplayPlayback.cs
Hash: `ea506107f4ac6cc6`

## `public sealed class CDisplayPlayback`

The reading view's playback area, holding the play button's read and gates.
It is split from [CDisplaySound](CDisplaySound.comment.md) by concern, since playback reads through the media port alone.
The header area [CDisplay](CDisplay.comment.md) builds one over its rules.
It holds no state of its own.
The shown draft and the latest play stay in [LDisplaySound](LDisplaySound.comment.md).

## `internal CDisplayPlayback(LDisplay display, LMediaPort media, LSettingsPort settings, CEnvoy envoy)`

Only the header area builds its playback area, over the port and the envoy the atelier handed down.
It takes the sound half and the atelier's repaint memory from `display`.
The read shows its failure through that memory, since it runs on every repaint.
The play gates answer a user act, so they show every failure.

## `public CLecternPlayback CDisplayPlaybackRead()`

The play button and volume tray verdicts for the shown entry.
The engine answers both, so Conduct reads no audio field.
A refused read shows `Sound.LoadFailed` once and hides both.

## `public void CDisplayPlaybackStart(double volume)`

The gate for the play button.
It plays the shown draft's own recording at the level the driver's slider shows.
The engine picks the file from the draft, so Conduct reads no field of it.

## `public void CDisplayPlaybackStart(string? audio, double volume)`

The gate for an accent row's play command.
It plays that row's recording at the slider's level.

## `public void CDisplayPlaybackCancel()`

The gate for a view going away.
It stops this view's own play.
A sound another view started plays on.
