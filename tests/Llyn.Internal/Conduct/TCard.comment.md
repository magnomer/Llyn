# TCard.cs

## `public sealed class TCard`

Covers the card gates over an entry desk on a real workspace, with no delay.
An entry row copies every field and its chosen mark.
A written value keeps the engine's verdict, and a draft splits its main pronunciation from the accents.
The etymology gates write the narrative, the source links and the spans of the held draft.
A typed title cites the Source the engine resolves it to.
The lookups find the stored rows a typed word matches, leaving out the rows the card already holds.
A typed tag lands trimmed and once, a picked tag keeps its stored id, and an erased tag leaves.
A typed tag offers the stored Tags split around the word, at most eight.
A blank or unmatched tag offers nothing.
A typed list adds its completed tags and situations once each and answers the rest.
A picked situation lands once under its stored id, and an erased situation leaves.
The frame line follows the order, and an unknown value shows the mark.
The translation gates are covered by `TCardTranslation`, and the register gates by `TCardRegister`, over the same helpers.

## `internal static (CDesk TCardDesk, CCard TCardCard) TCardPrepare(LEngine engine)`

Starts an entry desk by origin and subject with no delay, and builds its card gates.

## `private static LEtymologyDraft TCardEtymologyRead(CDesk desk)`

The held draft's etymology.

## `internal static long TCardSheetAdd(CDesk desk)`

Adds a meaning card to the held draft and answers its id.

## `private static IReadOnlyList<LTagDraft> TCardTagRead(CDesk desk, long sheet)`

The tags the held draft's card carries.

## `internal static IReadOnlyList<long> TCardTranslationRead(CDesk desk, long sheet)`

The entries the held draft's card links as translations.

## `internal static (long TCardSheet, long TCardSentence) TCardSentenceAdd(CDesk desk)`

Adds a meaning card with one sentence to the held draft, and answers both ids.
`TCardReference` shares it.
