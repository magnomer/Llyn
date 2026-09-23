# TMentionSpan.cs

## `public sealed class TMentionSpan`

Span operations use code-point offsets and respect word and script boundaries.
Division preserves gaps and avoids empty pieces around mentions.
Selection normalization trims outer spaces.
Etymology and sentence lookup accept only selections contained by one mention.
A draft reads a sentence example by its card and sentence ids.

## `MentionSpanResolve_MidWord_ReturnsTheWholeWord()`

A caret inside an English word or contraction resolves to the full word span, including when placed at its end.

## `MentionSpanResolve_OnSpace_ReturnsEmptySpan()`

A caret on a separator or trailing punctuation boundary produces an empty span rather than selecting adjacent text.

## `MentionSpanResolve_AtEndOfText_ReturnsEmptySpan()`

A caret at or beyond the end of text has no selected word.

## `MentionSpanResolve_SurrogatePairInWord_CountsItOnce()`

A supplementary Unicode character contributes one offset unit when a word span is resolved.

## `MentionSpanResolve_JapaneseRun_StopsAtScriptChange()`

Japanese runs stop when the script changes, and punctuation positions resolve to empty spans.

## `MentionSpanDivide_TwoMentionsAndGap_CutsFivePiecesInOrder()`

Out-of-order mentions return as alternating text and mention pieces in source order.
Each piece retains its exact offset, length, and mention identity.

## `MentionSpanDivide_MentionsAtBothEnds_CutsNoEmptyPiece()`

Mentions at the start and end leave only the intervening text piece.
No zero-length boundary pieces are emitted.

## `MentionSpanDivide_SurrogatePairBeforeMention_KeepsCodePointOffsets()`

A mention after a surrogate pair retains code-point offsets when the text is divided.
Each piece's text is still cut whole, with the pair kept in one piece.

## `MentionSpanDivide_NoMention_CutsOnePiece()`

Nonempty text without mentions becomes one plain-text piece.
Empty text yields no pieces.

## `MentionSpanDivide_OverlappingMentions_Throws()`

Overlapping mention ranges are rejected instead of producing ambiguous pieces.

## `MentionOffsetRead_AcrossSurrogatePair_RoundTripsWithUnitRead()`

Offset-to-unit and unit-to-offset conversions round-trip code-point positions across a surrogate pair, including positions inside the pair.

## `MentionSpanRead_PaddedSelection_DropsTheOuterSpaces()`

A space-padded selection is normalized to the trimmed word span.

## `MentionSpanRead_SurrogatePair_CountsCodePoints()`

Selection lengths count Unicode code points rather than UTF-16 code units.

## `MentionSpanRead_OnlySpaces_ReadsAnEmptySpan()`

A selection containing only spaces normalizes to an empty span.

## `EtymologyDraftFind_CaretInsideSpan_ReturnsThatMention()`

A caret within a mention resolves to that draft mention.
An exact selection of it resolves to the same mention.

## `EtymologyDraftFind_SpanPastTheMention_ReturnsNothing()`

A wider selection or a later unrelated selection does not match a mention merely because it overlaps or follows it.

## `ExampleDraftFind_SelectionInsideMention_ReturnsIt()`

A sentence answers a selection wholly inside one of its Mentions, and nothing for one running past its end.

## `DraftExampleRead_HeldCardAndSentence_ReturnsTheSentenceExample()`

A card id and a sentence id name the example a Mention request would edit.

## `DraftExampleRead_UnknownSentence_ReturnsNothing()`

A sentence id the card lacks reads nothing.
Zero ids on a draft holding no example read nothing too.
