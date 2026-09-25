# LPhonograph.cs

## `public interface LPhonograph`

The port through which the logic rings play a recording.
The engine decides which file plays and at what level, and never touches a player itself.
`LPhonographMedia` in Core.Windows plays through the platform, and a test stays silent.

## `void LPhonographPlay(string file);`

Plays the file from its start, replacing whatever was playing.

## `void LPhonographStop();`

Stops the playing file, and stays quiet when nothing plays.

## `void LPhonographVolumeSet(double volume);`

Sets how loud the next and the playing file sound, from zero to one.
