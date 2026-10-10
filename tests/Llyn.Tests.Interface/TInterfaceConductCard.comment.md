# TInterfaceConductCard.cs
Hash: `8e32259e6250bd76`

## `internal static class TInterfaceConductCard`

The relays for Conduct's card area.
They build the card, card list and sentence gates over a desk and reach the folio's internal maps.
Each relay is transparent and carries no test logic of its own.

## `internal static CCard TCardCreate(LEngine engine, CDesk desk, CEnvoy envoy)`

Builds the card gates over `desk` and real outlets on `engine`, as the editor does.
`envoy` is where a failed translation is shown.

## `internal static CCardList TCardListCreate(CDesk desk)`

Builds the card list gates over `desk`, as the editor does once.
Its stub card port supplies no real fold persistence.

## `internal static CSentence TSentenceCreate(LEngine engine, CDesk desk)`

Builds the sentence gates over `desk` with the engine's vocabulary facade and a real draft outlet.
Its settings port is the shared fake, and its envoy answers no to every question.
It holds a fresh repaint memory of its own.

## `internal static CStateValue TCardStateRead(LStateValue value)`

Relays the internal written-value map.

## `internal static IReadOnlyList<CImageDraft> TCardImageRead(IReadOnlyList<LImageDraft> images, LMediaPort media)`

Relays the internal image map, so a fact can hand it a row nobody located.
A fact hands its own `media`, so it can watch what the map asks and what the address becomes.

## `internal static IReadOnlyList<CVideoDraft> TCardVideoRead(IReadOnlyList<LVideoDraft> videos, LMediaPort media)`

Relays the internal video map, so a fact can hand it a row nobody located.

## `internal static CEntryDraft TCardEntryRead(LEntryDraft draft, IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> targets)`

Relays the entry draft map, so the facts pass engine drafts through the boundary.
It hands no folded ids, so every card maps unfolded.
