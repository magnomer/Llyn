# TInterfacePronunciation.cs

## `internal static partial class TInterface`

The relays for the pronunciation lookup seams below the engine.
That is the reading and answer records, the respelling rules, the generic source over a client, and the lookup fan-out.
The language pack loader and the per-session trove are relayed here too.
Each relay is transparent and carries no test logic of its own.

## `internal static LSource TSourceGenericCreate(LSourceSpec spec, HttpClient client)`

Returns the source as its interface, so a test never names the infrastructure type.

## `internal static LSeeker TLookupCreate(IReadOnlyList<LSource> sources, IReadOnlyList<LVariety>? varieties = null)`

Returns the lookup as its interface for the same reason.
The varieties default to none, so a test without a pack behaves like a language without varieties.
The cleanups default to none too, so a test without a pack sees only the built-in normalization.

## `internal static LReceiver TReceiverRespellingCreate(LReceiver inner, IReadOnlyList<LRespelling> groups)`

Returns the respelling wrapper as the receiver interface, so a test drives it as the engine would.
The three receiver relays below it let a test push one callback at a time through that interface.

## `internal static IReadOnlyList<LReading> TLookupReadingScan(IReadOnlyList<LReading> readings, IReadOnlyList<LVariety> varieties)`

Relays the static fan-out so it can be checked without a source or receiver.

## `internal static LTrove TTroveCreate()`

The trove is internal to the engine assembly, which opens its internals to this suite.
