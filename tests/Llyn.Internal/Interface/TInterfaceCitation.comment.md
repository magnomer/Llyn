# TInterfaceCitation.cs

## `internal static class TInterfaceCitation`

The relays that build a desk and a card over engine ports that fail on the Source calls.
It also reads the corpus list's internal draft map.
Each relay is transparent and carries no test logic of its own.

## `internal static CDesk TDeskFailCreate(LEngine engine, CEnvoy envoy)`

Builds an entry desk whose tenure is real but whose title resolve throws.

## `internal static CDesk TDeskFailCreate(LEngine engine, CEnvoy envoy, string scope, string origin, CSubject subject)`

Builds a desk of the given scope, origin and subject whose tenure is real but whose title resolve throws.
The corpus tests build an Example desk with it.

## `internal static CExample? TAnthologyDraftRead(this CAnthology anthology, LDraft? draft)`

Maps a held draft through the list's internal map, which reads the ready citation line.
It hands an empty tally, since the tally is `CCorpus`'s read.

## `internal static CCard TCardFailCreate(LEngine engine, CDesk desk, CEnvoy envoy)`

Builds a card whose Source list read throws.
