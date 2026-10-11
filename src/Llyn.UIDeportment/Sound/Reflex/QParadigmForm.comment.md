# QParadigmForm.cs
Hash: `a56b5f254ebf3c42`

## `public sealed class QParadigmForm`

One cell of the inflection box, ready to draw.
Conduct already chose the pieces and the tip key.

## `public IReadOnlyList<QParadigmPiece> QParadigmFormPiece`

The runs of the text to paint, in order, as Core cut them.

## `public string? QParadigmFormTip`

The localization key of the tooltip a muted cell shows, or null for a plain cell.

## `internal static QParadigmForm QParadigmFormCreate(CParadigmForm form)`

Copies one ready Conduct cell, mapping each piece through `QParadigmPiece.QParadigmPieceCreate`.
