# TInterfacePronunciation.cs

## `internal static partial class TInterface`

The relays for the pronunciation lookup seams below the engine.
That is the reading and answer records, the generic source over a client, and the lookup fan-out.
The language pack loader and the per-session trove are relayed here too.
Each relay is transparent and carries no test logic of its own.

## `internal static LSource TSourceGenericCreate(LSourceSpec spec, HttpClient client)`

Returns the source as its interface, so a test never names the infrastructure type.

## `internal static LSeeker TLookupCreate(IReadOnlyList<LSource> sources)`

Returns the lookup as its interface for the same reason.

## `internal static LTrove TTroveCreate()`

The trove is internal to the engine assembly, which opens its internals to this suite.
