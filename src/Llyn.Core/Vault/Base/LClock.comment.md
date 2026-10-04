# LClock.cs
Hash: `7cec97284660ee50`

## `public interface LClock`

The clock the engine stamps a moment with.
`LClockSystem` in Infrastructure answers with the system clock in UTC, and a test answers with a frozen moment.
Every draft, claim and fetch interval reads the time through here, so a test freezes the whole engine at once.
Every fetch interval also waits through here, so a harness can make those waits virtual.
The draft debounce in ShellEngine still waits on the real clock.

## `DateTimeOffset LClockRead()`

The present moment.

## `Task LClockPause(TimeSpan span, CancellationToken cancellation)`

Waits for the span to pass, or ends early when the token is cancelled.
