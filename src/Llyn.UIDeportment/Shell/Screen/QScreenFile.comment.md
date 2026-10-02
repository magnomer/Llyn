# QScreenFile.cs
Hash: `3465d9ec841d0d87`

## `internal sealed class QScreenFile`

The machine's own player, used for a film that is a file on this machine.
A local file is opened directly and costs the program nothing to serve.
Everything the framework's player needs is written here and nowhere else.
It reads the row's values from the `QScreen` that built it.

## `private const int QScreenClockDelay = 250`

How often the span clock looks at the film's position.

## `private readonly DispatcherTimer _qScreenClock`

The span clock is made where it is read, so the player owns its own parts.
The constructor wires its tick once, when the player is built.

## `private readonly QScreen _qScreenFileDriver`

The driver whose values this player plays by, and whose notice it raises on failure.

## `private readonly FrameworkElement _qScreenFileSurface`

The realized frame the player element is pulled from.

## `internal QScreenFile(QScreen driver, FrameworkElement surface)`

Subscribes the player's open, end and failure, and the span clock's tick.

## `private MediaElement QScreenMedia`

The frame's player element, pulled by contract ID on each read.

## `internal void QScreenMediaRefine(Uri address)`

Shows the player, hands it the file, and starts the clock that keeps the span.

## `internal void QScreenPlaybackRefine()`

Runs or halts the film to match what the row holds.
A player with no film is left alone, because there is nothing to run.

## `internal void QScreenSilenceRefine()`

Takes the film down and drops the file, so a Screen shown again does not start on the last one.

## `internal void QScreenLevelRefine()`

Hands the player the current level when the slider moves.

## `private void QScreenReadyRefine(object sender, RoutedEventArgs e)`

An opened film takes the level and the start, and waits paused unless the row is playing.

## `private void QScreenFinishRefine(object sender, RoutedEventArgs e)`

A film that ends starts again from the start of its span.

## `private void QScreenFailureRefine(object? sender, ExceptionRoutedEventArgs e)`

A film that will not open hides the player and shows the notice instead.

## Inline notes

### `private void QScreenSpanRefine(object? sender, EventArgs e)`

The film is sent back to the start once it passes the end the row carries.
The framework's player cannot be told to stop at a moment, so the position is watched instead.
An end at or before the start is no span at all and is ignored.
