# CLeafLine.cs
Hash: `6e7cb04876fe7741`

## `public sealed record CLeafLine(string CLeafLineHead, long CLeafLineSentence, IReadOnlyList<CMentionPiece> CLeafLinePiece, string CLeafLineCitation, IReadOnlyList<CGlossDraft> CLeafLineGloss)`

One example line of a reading card, ready to paint.

**Parameters**

- `CLeafLineHead`: the frame in brackets before the sentence, empty when the row states none.
- `CLeafLineSentence`: the id of the sentence row, the handle a click hands back to the find gate.
  The gate reads the text, language and Mentions itself, so none of them rides on the line.
- `CLeafLinePiece`: the sentence divided at its Mentions, with the unknown mark for an unknown one.
  The driver draws it run by run.
- `CLeafLineCitation`: the cited Source's line, empty when the row cites none.
- `CLeafLineGloss`: the sentence rendered in other languages, in the order the Example keeps them.

## `internal static CLeafLine LLeafLineRead(LSentenceDraft sentence, LSentenceOrder order, string mark, IReadOnlyDictionary<long, string> citations, LExamplePort examples)`

Maps one sentence row to its line.
The engine composes the frame, the sentence and the Source line through `examples`, by the rule the portrait prints by.
The sentence arrives divided around its Mentions in that same one call.
A row quoting no Example has no text to divide and no Source line.
