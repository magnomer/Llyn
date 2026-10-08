# LExamplePort.cs
Hash: `c02a2d7c260aee03`

## `public interface LExamplePort`

The slice of the engine a deportment sees when it lists Examples or shows a sentence line.
`LExampleFacade` implements it.

## `IReadOnlyList<LCatalogExample> LEngineExampleFind(LVista vista, string unknown = "", string unwritten = "");`

The Examples the vista lists, named distinctly, with `unknown` and `unwritten` wording blank fields.

## `bool LEngineTextMatch(string field, string shown);`

Whether a field showing `field` already shows the text `shown`, a blank field reading as nothing recorded.

## `(string, IReadOnlyList<LMentionPiece>, string) LEngineLineRead(LSentenceDraft sentence, LSentenceOrder order, string mark, IReadOnlyDictionary<long, string> citations);`

The frame, the sentence divided around its Mentions and the Source line a reading card shows for one sentence row.
The clerk composes them, so the display and the portrait share one rule.
