# CVoyageState.cs
Hash: `83f08d7bc43a5cdf`

## `public sealed record CVoyageState(bool CVoyageStatePast, bool CVoyageStateFuture);`

Whether the voyage can step back or forward, which lights the retreat and advance buttons.

**Parameters**

- `CVoyageStatePast`: true when the past trail holds a station.
- `CVoyageStateFuture`: true when the future trail holds a station.
