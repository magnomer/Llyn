# TInterfaceSource.cs
Hash: `cdac272edbe526b4`

## `internal static class TInterfaceSource`

The relays for the pronunciation sources.
That is the source spec records, the generic source over a client, and its find.
The reflex, shengfu, script and fanqie page sources are relayed here too.
Each relay is transparent and carries no test logic of its own.

## `internal static LSource TSourceGenericCreate(LSourceSpec spec, HttpClient client)`

Returns the source as its interface, so a test never names the infrastructure type.

## `internal static Task<(IReadOnlyList<LReflexDraft> LReflexFound, bool LReflexReached)> TReflexSourceFind(HttpClient client, string pattern, string character)`

Reads one character through the reflex page source with a bare rule holding only the given pattern.
The shengfu, script and fanqie finds beside it do the same for their sources.
