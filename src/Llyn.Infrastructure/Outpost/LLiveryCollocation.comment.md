# LLiveryCollocation.cs
Hash: `05c6d5672c67b022`

## `internal static class LLiveryCollocation`

How a Joplin note shows its collocations.
A collocation at home under a meaning card is a folding row, and a further link is a one-line stub.
`LLiveryCard.LLiveryFaceAppend` calls both after the card body.
The collocations linked to no meaning are the same folding rows under their own heading.

## `internal static void LLiveryListAppend(StringBuilder sheet, LLiveryPage page, LCardDraft card, IReadOnlyList<LCardDraft> collocations, Func<long, string> note, Func<string, string> lookup)`

Writes every collocation whose home is `card` as a folding row, in a `llyn-collocations` block.
The home is found by `LCollocation.LCollocationHomeRead`, so a collocation shows in full only there.
No homed collocation writes nothing, not even the heading.
The block opens with a `llyn-heading` span worded by `Display.CollocationHeading` with the row count.
The rows come from `LLiveryRowAppend`.

## `internal static void LLiveryOtherAppend(StringBuilder sheet, LLiveryPage page, IReadOnlyList<LCardDraft> others, string heading, Func<long, string> note)`

Writes `heading` as a level two heading, then `others` as folding rows in one `llyn-collocations` block.
The block has no `llyn-heading` span, since the level two heading names it.
No collocations write nothing, not even the heading.

## `private static void LLiveryRowAppend(StringBuilder sheet, LLiveryPage page, IReadOnlyList<LCardDraft> rows, Func<long, string> note)`

The one row writer, for a homed list and the unlinked band alike.
Each row is a `llyn-collocation` `details` through `LLiveryCard.LLiveryFoldFormat`, open unless `LLiveryPageFold` holds its id.
Its summary holds the phrase from `LLiveryPhraseFormat`, then any stored meaning in a `llyn-meaning` span.
So a folded row still reads as phrase and meaning.
The row body is the collocation's fields through `LLiveryCard.LLiveryFieldAppend` and `LLiveryCard.LLiveryBodyAppend`.
A row carries no number and no child cards of its own, like the Portrait row.

## `internal static void LLiveryStubAppend(StringBuilder sheet, LLiveryPage page, LCardDraft card, IReadOnlyList<LCardDraft> collocations, Func<string, string> lookup)`

Writes a `llyn-stub` span for each collocation that links to `card` as a further link.
The span words `Display.CollocationStub` with the phrase from `LLiveryPhraseFormat`, and its home meaning's number.
The number comes from `LMeaning.LMeaningPathRead`.
A collocation with no home or no path writes nothing.

## `private static string LLiveryPhraseFormat(LCardDraft collocation)`

The phrase a collocation row or stub shows, as `LCollocation.LCollocationPhraseRead` gives it.
The export prints no unknown mark, so a phrase marked unknown gives way to the title.
