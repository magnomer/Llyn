# Phonograph

Tests for Core.Windows's phonograph, run on a bare STA thread with no window.

## `TPhonographMedia`

A file that cannot play raises `LPhonographFinished` on the thread that built and played the phonograph.
The thread installs its context after construction, as the shell does after the root builds the phonograph.
So the test holds the rule that the playing thread's context, not only the constructing one, carries the finish.
A ten-second timer ends the wait, so a missing event fails the test instead of hanging it.

## Stamp

Commit: `ed7d18d90760`
- `TPhonographMedia.cs` `329fc759feebb77f`
