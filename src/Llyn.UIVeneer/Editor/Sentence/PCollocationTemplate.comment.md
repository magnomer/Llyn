# PCollocationTemplate.xaml

The editable collocation card, as markup alone.
The Deportment class of the same name loads it and forwards the card's events to the editor.

## `Theme.Collocation.Card`

Draws the collocation expression, meaning, and supporting fields.
Field order follows the reading view, keeping writing and reading aligned.
The editor's card fill writes each field and hands every list its items.

## `Theme.Card.Body`

The fields stand in the order the reading view draws them: expression, meaning, situation, translations, examples, tags.
A writer fills a card in the order a reader will meet it.
No field is labelled, for the same reason the meaning card labels none.

## `PCardPosition`

The badge is a field rather than a label, so the number a card carries can be written over.
Reordering by number reaches a place a drag has to scroll to.
The card badge takes the accent ring while its card's position is open.
The fill sets the ring, and the plain badge style stands otherwise.

## `PCardPositionText`

The badge number, opened for typing and the pointer while its card's position is open.
Each text box is named after the card value it writes.
So the editor's text handler knows which value a keystroke changes.
