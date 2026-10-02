# LMediaPort.cs
Hash: `ba9007f05060ea3d`

## `public interface LMediaPort`

The slice of the engine a media row sees.
It resolves a stored location to a playable address, says whether a recording file exists, and prepares one for play.
It plays, stops and sets the level of a recording through the phonograph, so the veneer owns no player.
Each play answers a ticket, and a stop acts only for the ticket of the latest play.
It opens a folder or a link through the usher, so the veneer starts no process.
`LMediaOutlet` implements it today, and a media clerk takes it over when the parts are dismantled.

## `Uri? LEngineLocationRead(string? location);`

The playable address of a stored location, or null when it names a file that does not exist.

## `(Uri, string?)? LEngineScreenRead(string? location);`

The resolved address of a video location and its hosted film id, in one read.
So the video row parses no address and makes no second request.

## `int LEngineRecordingPlay(string? file, double volume);`

Plays the recording at `volume`, and plays nothing for a file that does not exist.
It answers the play's ticket, or zero when nothing played.

## `int LEngineRecordingPlay(LEntryDraft draft, double volume);`

Plays the draft's own recording, the audio of its first pronunciation, and answers its ticket.

## `static (TimeSpan, TimeSpan?) LEngineSpanRead(LVideoDraft video)`

The moments a video row plays from and until, so the video row parses no timestamp.
It is static, since it reads only the row the caller holds.

## `(bool, bool) LEnginePlaybackRead(LEntryDraft draft);`

Whether the draft's own recording exists, and whether a reading view has anything to play.

## `(string?, bool) LEngineAudioRead(LEntryDraft draft);`

The draft's own recording while its file exists, and whether the editor's playback tray shows.

## `Uri? LEngineAudioResolve(string? file);`

The playable address of a stored recording, or null when its file is gone.

## `void LEngineRecordingStop(int ticket);`

Stops the playing recording only while `ticket` names it, so a stale stop never cuts a newer play.
It skips the engine gate, so it never waits behind a long engine call.

## `void LEngineVolumeSet(double volume);`

Sets the playing level, skipping the gate for the same reason as a stop.
