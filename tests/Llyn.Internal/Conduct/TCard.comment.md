# TCard.cs

## `public sealed class TCard`

Covers the card gates over an entry desk on a real workspace, with no delay.
An entry row copies every field and its chosen mark.
A written value keeps the engine's verdict, and a draft splits its main pronunciation from the accents.
The etymology gates write the narrative, the source links and the spans of the held draft.
A typed title cites the Source the engine resolves it to.
The lookups find the stored rows a typed word matches.
A typed tag lands trimmed and once, a picked tag keeps its stored id, and an erased tag leaves.
A court opened and then dropped leaves neither the link nor its draft.
The frame line follows the order, and an unknown value shows the mark.

## `private static (CDesk TCardDesk, CCard TCardCard) TCardPrepare(LEngine engine)`

Starts an entry desk by origin and subject with no delay, and builds its card gates.

## `private static LEtymologyDraft TCardEtymologyRead(CDesk desk)`

The held draft's etymology.

## `private static long TCardSheetAdd(CDesk desk)`

Adds a meaning card to the held draft and answers its id.

## `private static IReadOnlyList<LTagDraft> TCardTagRead(CDesk desk, long sheet)`

The tags the held draft's card carries.

## `private static (long TCardSheet, long TCardSentence) TCardSentenceAdd(CDesk desk)`

Adds a meaning card with one sentence to the held draft, and answers both ids.
