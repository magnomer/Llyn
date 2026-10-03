# TEngineFrequencyBand.cs
Hash: `fd58ade5d27080e7`

## `public sealed class TEngineFrequencyBand`

Covers how a stored frequency row is graded into a band when it is read.
A numeric raw is graded by interval from the source's `once` estimate, ahead of its band patterns.
A raw that is not a number is graded by the first band pattern it matches.
It is graded null when none does.
A stale or missing band is regraded from the raw and written back.
A blank language leaves the band and interval null.
It stores its rows through `TEngineFrequency.TFrequencyStoredSet` over the pack `TEngineFrequency.TEngineFrequencyPack`.

## Inline notes

### `private static string? TFrequencyBandRead(TWorkspace workspace, LEngine engine, long entryId, string source, string raw)`

Stores one raw value with no band for `source` and reads back the band the pack grades it into.
The entry's earlier rows are dropped first, so each call reads exactly one row.
