# TParadigmPiece.cs
Hash: `1fe5571e95a62e29`

## `public sealed class TParadigmPiece`

Covers how Core cuts a paradigm cell's text into the pieces every reader paints.
The app box and the Livery paint these pieces, so their own tests feed pieces and pin only the painting.

## `public void ParadigmFormScan_MarkedMiddle_CutsThreePieces()`

A form marked at offset 1 for 2 letters cuts into three pieces around the mark.
Only the middle piece is marked, and no piece carries a cut.

## `public void ParadigmFormScan_Split_CutsRootFromEnding()`

A form split at 4 with its root marked cuts into the marked root and the plain ending.
Only the root carries the cut, so a reader draws one hyphen between them.
