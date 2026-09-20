# PColophon.xaml.cs

## `public partial class PColophon : UserControl`

The read area over one Source, shared by the sources panel and the authors panel.
It draws the sheet it is handed and asks the engine for nothing.
The owning panel decides what is read.
The owning panel also decides when the control is shown, because only it knows what else stands in the cell.

## `internal void PColophonAttach(PWindow host)`

Binds the control to the window it reads localized text through.

## `internal void PColophonShow(LReference reference, IReadOnlyList<LAuthor> credits, string tally)`

Writes one Source onto the page with the credits the owner read for it and the tally sentence it composed.
The Source composes its own sheet, localizing through the catalog, so the control branches on nothing.
The credits come from the owner, because the catalog it browses already holds them for every row.

## `internal void PColophonShow(LColophon sheet)`

Writes a composed sheet onto the page: one text and one look per field, and the page shown.
A never-written field hides its heading, so a blank line never stands for two facts.
An unknown field is dressed as the placeholder the edit side shows, so the two sides read alike.

## `internal void PColophonTallyShow(string tally)`

Rewrites the citation chip alone, for when the count changes under a Source that stays shown.

## `internal void PColophonClear()`

Hides the page and shows the prompt in its place.
