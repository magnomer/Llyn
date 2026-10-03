# TInterfaceConductDesk.cs
Hash: `5d39336b237399d5`

## `internal static class TInterfaceConductDesk`

The relays for Conduct's desk area, its session and its errand.
They build a desk or a session and reach the desk's starts, its tenure and the errand's maps.
Each relay is transparent and carries no test logic of its own.

## `internal static CDesk TDeskCreate(LEngine engine, string scope, CEnvoy envoy)`

Builds a desk over a real draft outlet on `engine`, as the owners in Deportment do.

## `internal static void TDeskOccurrenceStart(this CDesk desk, long? situation)`

Starts a fresh entry already linked to the Situation, as the repertoire's new occurrence does.

## `internal static void TDeskQuotationStart(this CDesk desk, long? example)`

Starts a fresh entry already citing the Example, as the corpus's new quotation does.

## `internal static CSession TSessionCreate(CDesk desk, IReadOnlyList<Func<bool>> pending, Func<bool> readySeam, Action<long> storedSeam)`

Builds a session over `desk` alone, with no editor desk, as the guild does.
The overload over an editor desk records each editor finish in `seen`.
