# CParadigmPiece.cs
Hash: `8b37ee2542171e6b`

## `public sealed record CParadigmPiece(string CParadigmPieceText, bool CParadigmPieceMarked, bool CParadigmPieceCut)`

One run of a form's text, as the paradigm box paints it.
Core cut the runs, so the driver never counts letters itself.

**Parameters**

- `CParadigmPieceText`: the letters of the run.
- `CParadigmPieceMarked`: whether the run is painted as departing from the prediction.
- `CParadigmPieceCut`: whether a hyphen follows the run, where the ending starts.

## `internal static CParadigmPiece CParadigmPieceCreate(LParadigmPiece piece)`

Copies the three values of Core's piece.
It adds no rule.
