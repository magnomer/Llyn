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

## `TEditorMedia`

These cases pin the image and video rows of a card.
Each case adds one meaning card holding an image and a video, then presses keys through the window.
The envoy fake answers a browse with a chosen path, or with none when the case sets none.
A location that cannot be reached draws muted and ends with the missing mark.
A wide unreached video location keeps its file name and the missing mark, muted.
A reachable one keeps the plain text role.
An unfolded card shows both adders on one row, the video adder to the right.
Typing on an image row opens its field, and Enter or Page Down stores the text.
Insert on an image row adds an image, and Delete removes it.
Delete on a video row removes the video.
The browse key asks the envoy for an image or a video file by the lit row.
A cancelled browse keeps the old location.
The menu offers Browse and Remove, and Escape shuts it.
Typing on the video span slot stores the span.
Right on the lit image adder moves to the video adder, and Enter adds a video.

## Stamp

Commit: `9377e657b395`
- `TEditorBond.cs` `4a27e085281ad8c1`
- `TEditorCard.cs` `fa1293d9c7a8828d`
- `TEditorMedia.cs` `0968fa8f37b890f5`
