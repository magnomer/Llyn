# TImprintCredit.cs

## `public sealed class TImprintCredit`

Covers the credit rows of the source editor and the one blank row the deportment keeps.
A fresh draft opens with one blank row, and no draft reads as no rows.
Add under a credit opens a blank row beneath it and asks for the caret.
Remove on the blank row closes it again.
Later shifts a credit through the engine, so the rows come back reordered with their move verdicts.
Enter on a blank name reverts, on a new name creates the Author, and on an unchanged name sends nothing.
