# TLecternHinge.cs
Hash: `d50f83100083af6e`

## `public sealed class TLecternHinge`

Covers the reading card's fold hinge as the reading painter and the reading driver see it.
It builds its rows and runs its threads through the shared builders of `TCardHinge`.
The card leaves it builds carry no "see" lines, and only the heading case nests one collocation row.
One case clicks a bare "see" line, since the reading driver hears that click on the same list.
Both modes share the bare header and body structure, while their title controls differ.

## `public void LeafCardRefine_FoldedCard_CollapsesBodyChecksHingeAndClosesHeader()`

A folded reading card hides its body and checks its hinge.
Its header rounds all four corners and drops its bottom line.

## `public void LeafCardRefine_UnfoldedCard_ShowsBodyAndKeepsHeaderLine()`

An unfolded reading card shows its body and keeps the header's open corners and bottom line.

## `public void LeafCardRefine_FoldedCardWithBlankTitle_ShowsMeaningPeekInTheTitlePlace()`

A folded reading card without a title shows its meaning as the peek.
The title and the kind caption both give way.

## `public void LeafCardRefine_FoldedCardWithTitle_ShowsTitleAndHidesPeek()`

A folded reading card with a title keeps its title and shows no peek.

## `public void LeafCardRefine_UnstoredCard_CollapsesHinge()`

A reading card that is not stored hides its hinge and keeps its body.

## `public void LecternHingeClick_StoredCard_FoldsTheCardByIdThroughTheReadingView()`

A hinge click on a reading card bubbles to the card list and reaches the display's fold gate.
The store then holds the fold for that card.

## `public void LecternHingeClick_RefusedWrite_PutsTheHingeBack()`

A hinge click on a stored reading card is refused while the display has no entry chosen.
The card row is handed to the list directly, since a display with no entry answers no cards.
The hinge returns to unchecked and the store holds no fold.

## `public void LecternSeeClick_FoldedRow_OpensRowAndNoticesItsId()`

A click on a "see" line opens the row it names and hands its id on for the scroll.
The row's fold is stored first, and the line's item names that card, standing in for a collocation.
The store then holds no fold, and the notice carried the card's id.

## `public void LeafRowRefine_FoldedRow_ShowsMarkersAndCollapsesBody()`

A closed collocation row hides its body and checks its hinge.
It shows its phrase, its meaning on one line, its flags and its example and "also" markers.

## `public void LeafRowRefine_OpenRow_HidesMarkersAndShowsBody()`

An open collocation row shows its body, unchecks its hinge and wraps its meaning.
Its flags and markers give way, since the body shows the fields they summarise.

## `public void LeafCardRefine_HomedCollocation_ShowsHeadingOverRows()`

A meaning card with one homed collocation shows Conduct's ready caption and the homed list.

## `private static ContentPresenter TLecternRowRead(bool folded)`

Lays out one bare collocation row and paints it with the real row painter.

## `private static CLeaf TLecternRowCreate(long id, bool folded, IReadOnlyList<CLeaf> collocation, string heading)`

Builds a stored leaf with the row's phrase, one French flag and ready markers, nesting the given rows.

## `private static ContentPresenter TLecternHingeRead(CStateWording title, bool folded, bool stored)`

Lays out one reading card row and paints it with the real reading card painter.
