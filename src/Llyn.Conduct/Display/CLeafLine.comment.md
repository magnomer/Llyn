# CLeafLine.cs

## `public sealed record CLeafLine(`

One example line of a reading card, ready to paint.

**Parameters**

- `CLeafLineHead`: the frame in brackets before the sentence, empty when the row states none.
- `CLeafLineText`: the sentence, with the unknown mark for an unknown one.
- `CLeafLineLanguage`: the language the sentence is written in, empty when none is stated.
- `CLeafLineMention`: the words of the sentence that link to an entry.
- `CLeafLineCitation`: the cited Source's line, empty when the row cites none.
- `CLeafLineGloss`: the sentence rendered in other languages, in the order the Example keeps them.

## `internal static CLeafLine LLeafLineRead(LSentenceDraft sentence, LSentenceOrder order, string mark, IReadOnlyDictionary<long, string> citations)`

Maps one sentence row to its line.
The engine composes the frame, the sentence and the Source line, by the rule the portrait prints by.
A row quoting no Example answers its empty language, Mentions and Glosses from the row itself.
