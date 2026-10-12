# PCollocationTemplate.xaml
Hash: `4e2f11414f531bd8`

The editable collocation card, as markup alone.
The editor's markup merges it, and the editor's fill subscribes the card's events.

## `Theme.Collocation.Card`

Draws the collocation expression, meaning, and supporting fields.
Field order follows the reading view, keeping writing and reading aligned.
The editor's card fill writes each field and hands every list its items.

## `Theme.Card.Body`

The body orders expression, meaning, linked meanings, situations, registers, translations, examples, tags and media.
The media-add controls follow those fields.
A writer fills a card in the order a reader will meet it.
Only the linked meanings carry a caption, and no other field is labelled.

## `PTitle`

The title's right margin is `Theme.Card.TitleMargin`, the one the reading card's title carries.
It keeps room for the eraser and the hinge.
So a long title is cut at the same width in both modes.
The marks are laid over the strip beside it rather than given columns of their own.

## `PCardPeek`

The card's expression as display text, shown while the card is folded and untitled.
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

## `PCardBond`

The collocation's linked meanings as chips, set before the situations.
The caption `Card.SenseCaption` leads the row, and the first chip is the home.
Each chip shows the meaning's path and name, and an eraser unlinks it.
Only the collocation template carries it.

The caption and the add button are drawn as boxes of a chip's height.
Each pads one line of `Theme.Card.SituationSize` text by five above and below, as the chip and the situation entry do.
They take the chip's bottom margin of seven and sit at the top of the row.
Equal boxes from one top share one centre, so caption, chips and button read as one line.

## `PBondChooser`

The add button, captioned `Card.SenseAdd`, closing the chip row.
It carries no context menu, so a right click does nothing.

## `PBondMenu`

The meaning picker, a themed `Popup` under `PBondChooser` like the mention picker.
It takes `Theme.Popup.Surface`, the surface the mention picker's popup also takes.
It does not stay open, so an outside click shuts it.
The driver fills and opens it, and shuts it on a pick or Escape.

## `PBondList`

The picker's rows, one per meaning of the entry.
Each row is a `Theme.Popup.RowSurface`, so it lights on hover as the mention rows do.
Its path and name parts reuse the chip's part names, because the driver fills each from the same row item.

## `PCardBody`

The body is named so the shared fold painter can collapse it while the card is folded.

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

## `Theme.Command.Group`

The media-add group follows stored media.
Its negative margin cancels the reserved gutter, centring controls across the body.
