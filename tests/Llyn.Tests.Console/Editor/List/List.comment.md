# List

Tests of the console card rows, mirroring `src/Llyn.UIDemeanor/Editor/List/`.
Each test opens an entry in the Library tab's scribe mode and presses keys through the window.
The tests read the cells and key rows the frame drew.
So they pin what a user sees and what the legend offers.

## `TEditorCard`

These cases pin the card header, its keys and its number badge.
A list of one card offers no move and no badge, and Delete leaves that card in place.
A `#` on a one-card list starts the title with that character.
A `#` typed on another layout opens the badge by its sign.
Another sign on the same key opens nothing.
A move Conduct refuses, followed by undo, keeps the lit bar on the card.
A reset with the badge open closes the position in Conduct.
Escape in the badge closes the position in Conduct and keeps the old number.
A number too large settles the card last.
A typed number settles the card at that place.
Control with the page keys light the neighbour card headers.
Enter on a stored card folds it and leaves the entry unaltered.
A stored card draws its fold mark from the glyph set in use.
Insert on a header adds an unstored card and opens its title.
Control with Down moves the card, and the lit bar follows it.
Delete on a header of two removes the card.
A folded meaning with no title shows its definition in the header.
Enter on the add row adds a collocation on the collocation section.

## `TEditorBond`

These cases pin the linked meanings row of a collocation card.
The row shows the expression and the caption of the linked meanings.
Chips past the width flow onto the next line beside the caption, and the link mark follows the last chip.
Enter on the link mark opens every meaning, indented by depth.
A pick in the list attaches the meaning as a chip.
Escape in the list closes it and links nothing.
Left then Delete detaches the lit chip.
An undo that shrinks the chips under the light keeps a chip or the link mark lit.
A card seek away and back lights the link mark again.
Typing on the expression row opens its field and keeps the text.
A meaning card edits its definition and shows no linked meanings row.

## Stamp

Commit: `c37ca1837ded`
- `TEditorBond.cs` `413f09531b2c05a4`
- `TEditorCard.cs` `36706205dcfff8c6`
