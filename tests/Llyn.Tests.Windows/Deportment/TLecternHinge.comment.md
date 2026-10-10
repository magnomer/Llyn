# TLecternHinge.cs
Hash: `4f739a6f787e3563`

## `public sealed class TLecternHinge`

Covers the reading card's fold hinge as the reading painter and the reading driver see it.
It builds its rows and runs its threads through the shared builders of `TCardHinge`.
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

## `private static ContentPresenter TLecternHingeRead(CStateWording title, bool folded, bool stored)`

Lays out one reading card row and paints it with the real reading card painter.
