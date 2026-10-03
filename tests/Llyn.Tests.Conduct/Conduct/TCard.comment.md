# TCard.cs
Hash: `00a96760d51e4d20`

## `public sealed class TCard`

Covers the card gates over an entry desk on a real workspace, with no delay.
An entry row copies every field and its chosen mark.
An unknown state value carries the uncertain verdict.
The etymology gates write the narrative, the source links and the spans of the held draft.
The etymology read names a linked span by its words and its headword.
An empty desk reads no etymology chips.
A typed title cites the Source the engine resolves it to.
The lookups find the stored rows a typed word matches, leaving out the rows the card already holds.
A typed tag lands trimmed and once, a picked tag keeps its stored id, and an erased tag leaves.
A typed tag offers the stored Tags split around the word, at most eight.
A blank or unmatched tag offers nothing.
A typed list adds its completed tags once each and keeps the rest.
A tag the card holds stays out of its own offer, and another card still sees it.
A card's fields, sentence rows, glosses, images and situations arrive worded, each field with the hint its sheet names.
A sentence row arrives cited only when its example cites a stored Source.
The Source read lists every stored Source.
`TCardTranslation`, `TCardRegister`, `TCardSituation` and `TCardReference` cover their gates over the same helpers.

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
