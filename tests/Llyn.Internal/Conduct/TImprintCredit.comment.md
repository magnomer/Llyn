# TImprintCredit.cs

## `public sealed class TImprintCredit`

Covers the source editor's credit rows and the one blank row it keeps.
A fresh draft opens with one blank row, and no draft reads as no rows.
The blank row arrives placed among the rows, with id zero, no name and no move.
Add under a credit opens a blank row beneath it and asks for the caret.
Remove on the blank row closes it again.
Later and earlier shift a credit through the engine, so the rows come back reordered with their move verdicts.
The blank row and a row with no place move nothing.
Enter on a blank name reverts, and on a new name it creates the Author.
Enter on an unchanged name keeps the credit.
Enter on a blank name in the blank row keeps that row open, and a new name there closes it.
No draft or no row leaves the key untaken, and Escape with no byline reverts the field.

## `private static List<LAuthor> TImprintOpen(LEngine engine, CImprint imprint, params string[] names)`

Stores a Source crediting the named Authors in order, and opens the editor over it.
