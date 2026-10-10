# PMeaningTemplate.xaml
Hash: `44a74836c93b401d`

The editable meaning card, as markup alone.
The editor's markup merges it, and the editor's fill subscribes the card's events.

## `Theme.Meaning.Card`

Draws the card title, position control, and editable meaning fields.
Field order follows the reading view, keeping writing and reading aligned.
The editor's card fill writes each field and hands every list its items.

## `Theme.Card.Body`

The body orders definition, situations, registers, translations, examples, tags and media.
The media-add controls follow those fields.
A writer fills a card in the order a reader will meet it.
No field is labelled, because each is already drawn as what it is.
A label naming what a writer can read off the field only pushes the card down.

## `PTitle`

The title keeps the reading side's place.
The marks that drop and fold the card are laid over the strip beside it.
Given columns of their own they would narrow the title.
Both title styles prevent wrapping, so unequal widths would clip their text differently.
Its right margin is `Theme.Card.TitleMargin`, the one the reading card's title carries.

## `PCardPeek`

The card's definition as display text, shown while the card is folded and untitled.
It stands before `PTitle` in the same cell, so it draws behind the transparent title box.
The title box stays visible when cleared, preserving focus and caret on folded cards.
The peek takes no pointer, so a click there lands in the title box.
At rest it matches the reading card's peek in style and place.

## `PCardEraser`

The eraser sits left of the hinge by `Theme.Card.EraserMargin`.
It keeps that place when the hinge is collapsed on an unsaved card.

## `PCardHinge`

The hinge is a plain `ToggleButton`, found by its contract name.
It stands where the reading card's hinge stands.
A press on it is taken by the button, so the header under it never starts a drag.
The card fill subscribes its click and paints its state.

## `PCardBody`

The body is named so the shared fold painter can collapse it while the card is folded.

## `Theme.Command.Group`

The media-add group follows stored media.
Tooltips retain the labels when the controls show only icons.
Its negative margin cancels the reserved gutter, centring controls across the body.

## `PCardPosition`

The badge is a field rather than a label, so the number a card carries can be written over.
Reordering by number reaches a place a drag has to scroll to.
The card badge takes the accent ring while its card's position is open.
The fill sets the ring, and the plain badge style stands otherwise.

## `PCardPositionText`

The badge number, opened for typing and the pointer while its card's position is open.
The position handler commits on Enter or focus loss, rather than treating each keystroke as a reorder.

## `PCardHeader`

The named header lets the shared fold painter replace open corners and bottom edge with closed-header resources.
