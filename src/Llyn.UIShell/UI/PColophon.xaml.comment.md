# PColophon.xaml.cs

## `public partial class PColophon : UserControl`

The read area over one Source, shared by the sources panel and the authors panel.
It draws what it is handed and asks the engine for nothing, so the owning panel decides what is read.
The owning panel also decides when the control is shown, because only it knows what else stands in the cell.

## `internal void PColophonAttach(PWindow host)`

Binds the control to the window it reads localized text through.

## `internal void PColophonShow(LReference reference, IReadOnlyList<LAuthor> credits, string tally)`

Writes one Source onto the page with the credits the owner read for it and the tally sentence it composed.
The credits come from the owner, because the catalog it browses already holds them for every row.

## `internal void PColophonTallyShow(string tally)`

Rewrites the citation chip alone, for when the count changes under a Source that stays shown.

## `internal void PColophonClear()`

Hides the page and shows the prompt in its place.

## `private string? PColophonTextRead(LStateValue value)`

The text a three-state value reads as, or null when it was never written.
An unknown value reads the unknown mark, as the situation reading reads it.

## `private void PColophonTitleShow(LStateValue value)`

Writes the title at the head of the page.
A never-written or unknown title reads as the placeholder the edit side shows in its place.

## `private void PColophonKindShow(LReferenceKind kind)`

Writes the kind into its chip, or hides the chip when no kind was recorded.
An unknown kind is a kind the row records, so it reads inside the chip rather than hiding it.
The kind's text key is the one the sources panel owns, so the two read the same word.

## `private void PColophonValueShow(TextBlock field, StackPanel section, LStateValue value)`

Writes one three-state field under its heading, or hides the heading when the value was never written.
So a blank line never stands for two facts, and a heading never stands over nothing.
An unknown value is dressed as the placeholder the edit side shows, so the two sides read alike.

## `private void PColophonAuthorShow(LReference reference, IReadOnlyList<LAuthor> credits)`

Writes the credited authors under their heading, or the unknown mark when the authorship is recorded unknown.
No credit and no mark hides the heading, as any other never-written field hides its own.
The mark alone is dressed as the placeholder the edit side shows, so the two sides read alike.
