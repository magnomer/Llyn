# TExampleLine.cs

## `public sealed class TExampleLine`

Covers the three texts a reading card shows for one sentence row.
The engine composes them by the portrait's rule, so the display keeps no copy of it.

## `public void EngineLineRead_TrailingOrder_WritesTheRoleBeforeTheMarker()`

The frame follows the language's order, and the sentence stands apart from it.

## `public void EngineLineRead_UnknownMarker_WritesTheMark()`

An unknown marker reads the mark inside the brackets.
A row without an Example has no sentence and no Source line.

## `public void EngineLineRead_EmptyFields_DropsTheHead()`

A frame with one part keeps no stray space, and a row with no frame has no brackets.

## `public void EngineLineRead_CitedSource_AnswersItsLine()`

A cited Source answers its ready line, and a missing line or no citation answers nothing.

## `private static (string, string) TEngineHeadRead(LSentenceDraft sentence, LSentenceOrder order, IReadOnlyDictionary<long, string> citations)`

The frame and the sentence of one row, read with `?` as the unknown mark.
