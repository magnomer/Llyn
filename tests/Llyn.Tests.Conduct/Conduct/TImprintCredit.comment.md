# TImprintCredit.cs
Hash: `7e26512814de5c3b`

## `public sealed class TImprintCredit`

Covers the source editor's credit rows and the one blank row it keeps.
A fresh draft opens with one blank row, and no draft reads as no rows.
The blank row arrives placed among the rows, with id zero, no name and no move.
Add under a credit opens a blank row beneath it and asks for the caret.
Add on the blank row only asks for the caret, and add with no credit elsewhere does nothing.
Neither raises a change or touches the engine's credits.
Remove on the blank row closes it again.
Later and earlier shift a credit through the engine, so the rows come back reordered with their move verdicts.
The blank row and a row with no place move nothing.
Each add, remove and move hands only the row's place and id, as the driver does.
Enter on a blank name reverts, and on a new name it creates the Author.
Enter on an unchanged name keeps the credit.
Enter on a blank name in the blank row keeps that row open, and a new name there closes it.
No draft or no row leaves the key untaken, and Escape with no byline reverts the field.

## `private static List<LAuthor> TImprintOpen(LEngine engine, CImprint imprint, params string[] names)`

Stores a Source crediting the named Authors in order, and opens the editor over it.
