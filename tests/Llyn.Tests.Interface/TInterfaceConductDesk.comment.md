# TInterfaceConductDesk.cs
Hash: `303f53e2b7d13a4d`

## `internal static class TInterfaceConductDesk`

The relays for Conduct's desk area, its session and its errand.
They build a desk or a session and reach the desk's starts, its tenure and the errand's maps.
They read the held draft's sentence ids and build the editor's marks over an entry port a fact fakes.
Each relay is transparent and carries no test logic of its own.

## `internal static CDesk TDeskCreate(LEngine engine, string scope, CEnvoy envoy)`

Builds a desk over real draft and settings outlets on `engine`, as the Conduct areas that own one do.

## `internal static CDesk TDeskCreate(LEngine engine, string scope, CEnvoy envoy, string origin, CSubject subject)`

Builds the same desk under its own `origin` and `subject`, as the corpus and the playwright build theirs.

## `internal static void TDeskOccurrenceStart(this CDesk desk, long? situation)`

Starts a fresh entry already linked to the Situation, as the repertoire's new occurrence does.

## `internal static void TDeskQuotationStart(this CDesk desk, long? example)`

Starts a fresh entry already citing the Example, as the corpus's new quotation does.

## `internal static IReadOnlyList<long> TDeskSentenceRead(this CDesk desk)`

The ids of every sentence row on the held draft's meaning and collocation cards, in ascending order.
It answers none with no draft held, so a fact names no engine draft member itself.

## `internal static CEsteem TEsteemCreate(LEngine engine, LEntryPort entries, long stored)`

Builds the editor's marks over a real desk holding the stored entry `stored`, as the editor does.
The desk is an Input desk bound to a fresh library vista, over a real draft outlet.
Its display rules ask `entries`, so a fake lets a fact answer the grasp reads with hostile values.
Its envoy answers no and records nothing, and its media port is a bare stub.

## `internal static CSession TSessionCreate(CDesk desk, IReadOnlyList<Func<bool>> pending, Func<bool> readySeam, Action<long> storedSeam)`

Builds a session over `desk` alone, with no editor desk, as the guild does.
The overload over an editor desk records each editor finish in `seen`.
