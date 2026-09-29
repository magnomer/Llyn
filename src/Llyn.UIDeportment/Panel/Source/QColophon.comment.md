# QColophon.cs

## `internal sealed class QColophon`

The driver of the read area over one Source, shared by the sources panel and the authors panel.
It draws the sheet it is handed and asks the engine for nothing.
The owning driver builds it over the nested `PColophon` page and decides what is read.
The owning driver also decides when the page is shown, because only it knows what else stands in the cell.

## `internal QColophon(UserControl surface)`

Takes the veneer page as its surface.
Every part is pulled from it by contract ID, so no name scope is copied.

## `private StackPanel QColophonBody`

Each named part of the page is found through `QContract.QContractFind` under its markup name.

## `internal void QColophonRefine(CColophon sheet)`

Writes a composed sheet onto the page: one text and one look per field, and the page shown.
A never-written field hides its heading, so a blank line never stands for two facts.
An unknown field is dressed as the placeholder the edit side shows, so the two sides read alike.

## `internal void QColophonTallyRefine(string tally)`

Rewrites the citation chip alone, for when the count changes under a Source that stays shown.

## `internal void QColophonClearRefine()`

Hides the page and shows the prompt in its place.
