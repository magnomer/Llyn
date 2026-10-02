# CLeafLine.cs
Hash: `1d18a14305c141da`

## `public sealed record CLeafLine(`

One example line of a reading card, ready to paint.

**Parameters**

- `CLeafLineHead`: the frame in brackets before the sentence, empty when the row states none.
- `CLeafLineSentence`: the id of the sentence row, the handle a click hands back to the find gate.
  The gate reads the text, language and Mentions itself, so none of them rides on the line.
- `CLeafLinePiece`: the sentence divided at its Mentions, with the unknown mark for an unknown one.
  The driver draws it run by run.
- `CLeafLineCitation`: the cited Source's line, empty when the row cites none.
- `CLeafLineGloss`: the sentence rendered in other languages, in the order the Example keeps them.

## `internal static CLeafLine LLeafLineRead(LSentenceDraft sentence, LSentenceOrder order, string mark, IReadOnlyDictionary<long, string> citations)`

Maps one sentence row to its line.
The engine composes the frame, the sentence and the Source line, by the rule the portrait prints by.
The sentence arrives divided around its Mentions in that same one call.
A row quoting no Example answers its empty Mentions and Glosses from the row itself.
