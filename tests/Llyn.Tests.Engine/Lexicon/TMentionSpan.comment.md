# TMentionSpan.cs
Hash: `40e8ee01cf5db21d`

## `public sealed class TMentionSpan`

Span operations use code-point offsets and respect word and script boundaries.
Division preserves gaps and avoids empty pieces around mentions.
Selection normalization trims outer spaces.
Etymology and Example lookup accept only selections contained by one mention.
A draft reads a sentence example by its card and sentence ids.

## `public void MentionSpanResolve_MidWord_ReturnsTheWholeWord()`

A caret inside an English word or contraction resolves to the full word span.

## `public void MentionSpanResolve_OnSpace_ReturnsEmptySpan()`

A caret on a separator or trailing punctuation boundary produces an empty span rather than selecting adjacent text.

## `public void MentionSpanResolve_AtEndOfText_ReturnsEmptySpan()`

A caret at or beyond the end of text has no selected word.

## `public void MentionSpanResolve_SurrogatePairInWord_CountsItOnce()`

A supplementary Unicode character contributes one offset unit when a word span is resolved.

## `public void MentionSpanResolve_JapaneseRun_StopsAtScriptChange()`

Japanese runs stop when the script changes, and punctuation positions resolve to empty spans.

## `public void MentionSpanDivide_TwoMentionsAndGap_CutsFivePiecesInOrder()`

Out-of-order mentions return as alternating text and mention pieces in source order.
Each piece retains its exact text and mention identity.

## `public void MentionSpanDivide_MentionsAtBothEnds_CutsNoEmptyPiece()`

Mentions at the start and end leave only the intervening text piece.
No zero-length boundary pieces are emitted.

## `public void MentionSpanDivide_SurrogatePairBeforeMention_CutsAtTheMention()`

A mention after a surrogate pair is cut at its own characters when the text is divided.
Each piece's text is still cut whole, with the pair kept in one piece.

## `public void MentionSpanDivide_NoMention_CutsOnePiece()`

Nonempty text without mentions becomes one plain-text piece.
Empty text yields no pieces.

## `public void MentionSpanDivide_OverlappingMentions_Throws()`

Overlapping mention ranges are rejected instead of producing ambiguous pieces.

## `public void MentionOffsetRead_AcrossSurrogatePair_RoundTripsWithUnitRead()`

Offset-to-unit and unit-to-offset conversions round-trip every code-point position across a surrogate pair.
A unit inside the pair reads back as the pair's own offset.

## `public void MentionSpanRead_PaddedSelection_DropsTheOuterSpaces()`

A space-padded selection is normalized to the trimmed word span.

## `public void MentionSpanRead_SurrogatePair_CountsCodePoints()`

Selection lengths count Unicode code points rather than UTF-16 code units.

## `public void MentionSpanRead_OnlySpaces_ReadsAnEmptySpan()`

A selection containing only spaces normalizes to an empty span.

## `public void EtymologyDraftFind_CaretInsideSpan_ReturnsThatMention()`

A caret within a mention resolves to that draft mention.
An exact selection of it resolves to the same mention.

## `public void EtymologyDraftFind_SpanPastTheMention_ReturnsNothing()`

A wider selection or a later unrelated selection does not match a mention merely because it overlaps or follows it.

## `public void ExampleDraftFind_SelectionInsideMention_ReturnsIt()`

An Example draft answers a selection wholly inside one of its Mentions, and nothing for one running past its end.

## `public void DraftExampleRead_HeldCardAndSentence_ReturnsTheSentenceExample()`

A card id and a sentence id name the example a Mention request would edit.

## `public void DraftExampleRead_UnknownSentence_ReturnsNothing()`

A sentence id the card lacks reads nothing.
Zero ids read nothing too.
