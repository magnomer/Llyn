# LMediaPort.cs

## `public interface LMediaPort`

The slice of the engine a media row sees.
It resolves a stored location to a playable address, says whether a recording file exists, and prepares one for play.
The sweep drops recordings no draft or entry names any more.
`LEngine` implements it today, and a media clerk takes it over when the parts are dismantled.
