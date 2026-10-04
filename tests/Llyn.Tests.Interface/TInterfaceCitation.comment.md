# TInterfaceCitation.cs
Hash: `61b64b8ba579cfe3`

## `internal static class TInterfaceCitation`

The relays that build a desk, an imprint and a card over engine ports that fail on the Source calls.
It also reads the corpus list's internal draft map.
Each relay is transparent and carries no test logic of its own.

## `internal static CDesk TDeskFailCreate(LEngine engine, CEnvoy envoy)`

Builds an entry desk whose tenure is real but whose title resolve throws.

## `internal static CDesk TDeskFailCreate(LEngine engine, CEnvoy envoy, string scope, string origin, CSubject subject)`

Builds a desk of the given scope, origin and subject whose tenure is real but whose title resolve throws.
The corpus tests build an Example desk with it.

## `internal static CImprint TImprintCreate(CAtelier atelier, CEnvoy envoy)`

Builds a Source imprint over the atelier's ports and repaint memory, as the shelf does.
The byline and imprint tests build theirs here, so they never call the internal constructor.

## `internal static CImprint TImprintFailCreate(LEngine engine, CEnvoy envoy)`

Builds a Source imprint whose tenure is real but whose byline find throws.
It holds a fresh repaint memory of its own.
The byline tests prove a failed find reaches the notice with it.

## `internal static CExample? TAnthologyDraftRead(this CAnthology anthology, LDraft? draft)`

Maps a held draft through the list's internal map, which reads the ready citation line.
The map reads the tally from the anthology's own panel, so the relay hands none.

## `internal static CCard TCardFailCreate(LEngine engine, CDesk desk, CEnvoy envoy)`

Builds a card whose Source list read throws.
