# LClock.cs

## `public interface LClock`

The clock the engine stamps a moment with.
`LClockSystem` in Infrastructure answers with the system clock in UTC, and a test answers with a frozen moment.
Every draft, claim and fetch interval reads the time through here, so a test freezes the whole engine at once.

## `DateTimeOffset LClockRead()`

The present moment.
