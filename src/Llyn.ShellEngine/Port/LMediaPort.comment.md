# LMediaPort.cs

## `public interface LMediaPort`

The slice of the engine a media row sees.
It resolves a stored location to a playable address, says whether a recording file exists, and prepares one for play.
It plays, stops and sets the level of a recording through the phonograph, so the veneer owns no player.
Each play answers a ticket, and a stop acts only for the ticket of the latest play.
It opens a folder or a link through the usher, so the veneer starts no process.
The sweep drops recordings no draft or entry names any more.
`LEngine` implements it today, and a media clerk takes it over when the parts are dismantled.

## `(Uri, string?)? LEngineScreenRead(string? location);`

The resolved address of a video location and its hosted film id, in one read.
So the video row parses no address and makes no second request.

## `(bool, bool) LEnginePlaybackRead(LEntryDraft draft);`

Whether the draft's own recording exists, and whether a reading view has anything to play.
