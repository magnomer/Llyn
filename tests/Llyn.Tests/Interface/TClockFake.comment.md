# TClockFake.cs

## `internal sealed class TClockFake : LClock`

The clock every test engine is built over, running on the system clock until a test sets it.
Setting it is the one way to freeze or advance time inside an engine.

## `public DateTimeOffset LClockRead()`

The moment the current reader answers.

## `internal void TClockSet(Func<DateTimeOffset> read)`

Replaces the reader, so a test may advance a captured variable between requests.

## `internal void TClockSet(DateTimeOffset moment)`

Freezes the clock at one moment.
