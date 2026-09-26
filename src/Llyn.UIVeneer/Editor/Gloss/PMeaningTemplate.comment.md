# PMeaningTemplate.xaml

The editable meaning card, as markup alone.
The Deportment class of the same name loads it and forwards the card's events to the editor.

## `Theme.Meaning.Card`

Draws the card title, position control, and editable meaning fields.
Field order follows the reading view, keeping writing and reading aligned.
The editor's card fill writes each field and hands every list its items.

## `Theme.Card.Body`

The fields stand in the order the reading view draws them: definition, situation, translations, examples, tags.
A writer fills a card in the order a reader will meet it.
No field is labelled, because each is already drawn as what it is.
A label naming what a writer can read off the field only pushes the card down.

## `PTitle`

The title keeps the reading side's place.
The mark that drops the card is laid over the strip beside it.
Given a column of its own it would narrow the title.
The same title would then wrap in one mode and not the other.

## `Theme.Command.Group`

The pair takes the icon cluster the pronunciation lookup and the audio download already carry.
An outlined pill of muted text read as one more tag chip beside the tags above it.
The tooltip carries the label the icon drops.
The negative margin cancels the gutter column, so the cluster centres on the whole card body.

## `PCardPosition`

The badge is a field rather than a label, so the number a card carries can be written over.
Reordering by number reaches a place a drag has to scroll to.
The card badge takes the accent ring while its card's position is open.
The fill sets the ring, and the plain badge style stands otherwise.

## `PCardPositionText`

The badge number, opened for typing and the pointer while its card's position is open.
Each text box is named after the card value it writes.
So the editor's text handler knows which value a keystroke changes.
