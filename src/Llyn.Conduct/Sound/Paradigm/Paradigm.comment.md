# Paradigm

This folder holds the inflection box and its rows, slots, pieces and table, mapped from the engine's paradigm answer.

## `CParadigmForm`, `CParadigmLine`, `CParadigmPiece`, `CParadigmTable`, `CParadigmView`

The inflection box, mapped from the engine's answer without adding a rule.
The wording stays in Core, so the box, the plain slot list and the Joplin note agree.
Core cut the pieces, so the driver never counts letters itself.
Core leaves marks and split empty for a cell that shows a status, so its one piece is plain.
Headers, groups and labels are localization keys, and Deportment looks them up.
A null view stays null.
Toggling between the collapsed and expanded views is a visual state, so Conduct holds no gate for it.

## `CParadigmSlot`

One row of a paradigm, as the paradigm table lists it.
The controller maps each engine row to one slot, so the driver never groups them.
Each row arrives with the text to show and the key of its tip.

## Stamp

Commit: `c2f335c331fe`
- `CParadigmForm.cs` `fd48d5b7ad4fcb36`
- `CParadigmLine.cs` `c11e8f1a3105a68a`
- `CParadigmPiece.cs` `8b37ee2542171e6b`
- `CParadigmSlot.cs` `6f25972be133e4b1`
- `CParadigmTable.cs` `89327dddd93bed5a`
- `CParadigmView.cs` `ce2e6cbd1f144e5f`
