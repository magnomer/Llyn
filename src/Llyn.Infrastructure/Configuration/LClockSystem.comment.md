# LClockSystem.cs
Hash: `b36331da33b322ac`

## `public sealed class LClockSystem : LClock`

The clock port answered by the system clock.

## `public DateTimeOffset LClockRead()`

The present moment in UTC, so a stamp reads the same wherever the workspace travels.

## `public Task LClockPause(TimeSpan span, CancellationToken cancellation)`

A real wait of the span through `Task.Delay`, cancelled by the token.
