# List

List holds the console's card rows of one section, which are the header, the definition row and the linked meanings.
It is the console face of the GUI's card, and it holds no entry rule.
Every verdict, position and list comes from Conduct, and these classes only draw it and route keys.

## `BCard`

This is the header of one meaning or collocation card and the keys that act on it.
The header is two rules, the position, a rule, the caption, filling rules and the hinge.
The caption is the card's title, or the section's word while the title is muted.
A folded card with no title shows its definition or expression as the caption, which is the peek.
So a folded card still says what it holds.
The hinge shows only when the step carries a fold gate, so a card not yet stored has none.
The hinge glyph turns with the folded state, and the fold itself goes through that gate.
The header is muted while its caption is muted, a heading otherwise, and the lit tone wins over both.
A card has parts, with part 0 the header and part 1 the definition or expression row.
A collocation card has part 2, the linked meanings.
So `BCardLeadRead` is 1 for a meaning and 2 for a collocation.
The chip fields come after those parts, so the frame counts them from that lead.
Enter on the header folds it.
Insert on a header adds a card of the section's kind.
Enter on the add row after the last card does the same.
A new card is lit and its title opens at once, so the user types its name.
Delete on a header removes the card.
Control with Up or Down moves the card, and the light follows the card by its id.
The move and the badge are offered only when the section has more than one card.
Control with the page keys light the neighbour card headers.
A printable key on a header opens the title field, and on part 1 it opens the definition field.
Enter, Escape, Up and Down close an open title or definition field and keep its text.
Closing calls the keeper, so a close is one stored step.
A close settles an open badge and drops an open linked-meanings list.
A reset drops the fields without the keeper and closes the badge through Conduct.
So a new tenure never leaves a position open in Conduct.
While the badge is open it takes every key first, then the linked meanings, then the field keys.

## `BCardBadge`

This is the number a card is moved to, drawn in brackets where the position stood.
Opening asks Conduct whether the position may open, and a refusal opens nothing.
The entry starts with the card's current position, and the first typed digit replaces it.
A printable key that is not a digit is swallowed, so the entry holds digits only.
Enter, Up and Down settle the number through Conduct, and Escape closes it through Conduct.
The settle answer is dropped, since Conduct keeps the card where it was when it refuses.
`#` opens the badge by its sign, so the key works on any layout.

## `BCardField`

This is one text field of a card, with the gate that stores its text passed in.
The title and the definition or expression row are two fields of one type.
Opening seeds the entry with the wording's text and picks a hint.
The hint is the one given, else the wording's own, else the definition or expression word of the kind.
Closing applies the pending edit before it resets, so no typed text is lost.
A closed field draws its wording indented three cells.
A muted wording draws as its hint in the muted role, and a keyed wording is read from the catalog.
An open field is drawn indented the same, so the text does not jump on open.

## `BBond`

This is the linked meanings row of a collocation card.
It shows a caption, the linked meanings as bracketed chips, and a link mark after them.
The chips wrap beside the caption, and each further line starts under the first chip.
A chip too wide for the room is cut to it, so a long name never breaks the line.
The lit chip is an index, and no index means the link mark is lit.
The index is clamped by the card's current count.
So a detach or an undo never leaves it past the end.
Left and Right move the light over the chips and the mark.
Delete on a lit chip detaches that meaning through Conduct, and it is offered only while a chip is lit.
Enter on the link mark opens a list of every meaning, indented by its depth.
The list takes Up, Down, Enter and Escape before the row, and a printable key seeks in it.
Enter attaches the lit meaning and shuts the list.
An empty list offers no Enter pick.
The row reports an entry column only while the list is open, so the list hangs under the link mark.
The keys are live only on part 2 of a collocation card with no other card field open.

## Stamp

Commit: `c37ca1837ded`
- `BBond.cs` `f6f335cc7be31f94`
- `BCard.cs` `900c7248c8e363ad`
- `BCardBadge.cs` `c16b782872765352`
- `BCardField.cs` `edbf97b5c4f8f1c2`
