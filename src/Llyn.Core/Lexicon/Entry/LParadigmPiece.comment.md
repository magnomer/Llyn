# LParadigmPiece.cs
Hash: `b70a4ef83c8e8359`

## `public sealed record LParadigmPiece(string LParadigmPieceText, bool LParadigmPieceMarked, bool LParadigmPieceCut)`

One run of a paradigm cell's text, cut by `LParadigmForm.LParadigmFormScan`.
Every letter of a piece shares one marking.

**Parameters**

- `LParadigmPieceText`: the letters of the run, never empty.
- `LParadigmPieceMarked`: whether the run departs from the prediction.
- `LParadigmPieceCut`: whether the split between root and ending follows the run.
