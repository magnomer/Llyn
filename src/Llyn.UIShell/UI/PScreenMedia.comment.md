# PScreenMedia.cs

## `public partial class PScreen`

The machine's own player, used for a film that is a file on this machine.
A local file is opened directly and costs the program nothing to serve.
Everything the framework's player needs is written here and nowhere else.

## `private void PScreenClockPrepare()`

The span clock is wired once, when the Screen is built.
The clock is made where it is read rather than in the constructor, so the player owns its own parts.

## `private void PScreenMediaPlay(Uri address)`

Shows the player, hands it the file, and starts the clock that keeps the span.

## `private void PScreenMediaSync()`

Runs or halts the film to match what the row holds.
A player with no film is left alone, because there is nothing to run.

## `private void PScreenMediaStop()`

Takes the film down and drops the file, so a Screen shown again does not start on the last one.

## Inline notes

### `private void PScreenSpanHandle(object? sender, EventArgs e)`

The film is sent back to the start once it passes the end the row carries.
The framework's player cannot be told to stop at a moment, so the position is watched instead.
An end at or before the start is no span at all and is ignored.
