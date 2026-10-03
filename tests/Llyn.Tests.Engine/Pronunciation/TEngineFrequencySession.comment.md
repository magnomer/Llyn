# TEngineFrequencySession.cs
Hash: `0c059ea184e978ba`

## `public sealed class TEngineFrequencySession`

Covers when the fill asks its sources again within one session.
A word every source reached yet none knew is asked once, however often it is read.
A word no source could be reached for is asked again on the next read.
A read while a fill is pending leaves that fill running instead of restarting it.
It counts requests through the `TSourceHandler` and builds its entry through `TEngineFrequency.TFrequencyDraftCreate`.

## Inline notes

### `private static async Task TFrequencyCountCheck(TSourceHandler handler, int count)`

Polls the handler until it has seen `count` requests or the patience runs out.
The short delay that follows in the callers lets the fill settle after its last request.
