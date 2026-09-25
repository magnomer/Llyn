# LPhonographMedia.cs

## `public sealed class LPhonographMedia : LPhonograph`

The phonograph the composition root hands the engine.
It holds one platform player and maps each port call onto it, with no logic of its own.
The root builds one for the whole run, so a workspace switch keeps the same player.

## `public void LPhonographPlay(string file)`

Opens the file on the player and starts it.

## `public void LPhonographStop()`

Stops the player.

## `public void LPhonographVolumeSet(double volume)`

Hands the level to the player.
