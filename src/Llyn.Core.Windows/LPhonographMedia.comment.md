# LPhonographMedia.cs

## `public sealed class LPhonographMedia : LPhonograph`

The phonograph the composition root hands the engine.
It holds one platform player and maps each port call onto it, with no logic of its own.
The root builds one for the whole run, so a workspace switch keeps the same player.
The player belongs to the shell's thread, so a call from another thread is sent there first.

## `public LPhonographMedia()`

Subscribes the player's failure to `LPhonographFailHandle`.

## `public void LPhonographPlay(string file)`

Opens the file on the player and starts it.

## `public void LPhonographStop()`

Stops the player and closes it, so the file is no longer held open.

## `public void LPhonographVolumeSet(double volume)`

Hands the level to the player.

## `private void LPhonographFailHandle(object? sender, ExceptionEventArgs e)`

Closes the player when a file fails to play, so the file is released.
The failure is not reported, since the phonograph has no route to the window.
