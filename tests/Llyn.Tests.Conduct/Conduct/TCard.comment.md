# TCard.cs
Hash: `a1c183924f8ad673`

## `public sealed class TCard`

Covers how an entry draft is worded for its sheet, read straight from drafts with no workspace.
An unknown state value carries the uncertain verdict.
A card's fields, sentence rows, glosses, images and situations arrive worded, each field with the hint its sheet names.
A sentence row arrives cited only when its example cites a stored Source.
A panel row copies every field of the engine row and its chosen mark.
The other gates live in sibling classes named `TCard` plus the gate, such as `TCardTag` and `TCardList`.
Several of them build through the helpers kept here.

## `internal static (CDesk TCardDesk, CCard TCardCard) TCardPrepare(LEngine engine)`

Starts an entry desk with no delay, and builds its card gate over it.

## `internal static long TCardSheetAdd(CDesk desk)`

Adds a meaning card to the held draft and answers its id.
`TCardTag`, `TCardTranslation`, `TCardRegister` and `TCardSituation` share it.

## `internal static IReadOnlyList<long> TCardTranslationRead(CDesk desk, long sheet)`

The ids of the entries the held draft's card links as translations.
`TCardTranslation` shares it.

## `internal static (long TCardSheet, long TCardSentence) TCardSentenceAdd(CDesk desk)`

Adds a meaning card with one sentence to the held draft, and answers both ids.
`TCardReference` shares it.
