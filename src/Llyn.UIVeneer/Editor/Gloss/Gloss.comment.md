# Gloss

This folder holds the language selector row and the editable meaning card, as markup alone.
The editor's markup merges them, and the editor's fill fills the named parts and subscribes the card's events.

## `PLanguageTemplate.xaml`

`Theme.Language.Row` shows a language flag and name, with an empty circle when no flag exists.
The circle starts hidden, and the fill shows it for a language without a flag.
The flag and name parts carry the names the corpus speaker row uses, so one fill serves both.
Clicking the row delegates speaker selection to the editor.

## `PMeaningTemplate.xaml`

### `Theme.Meaning.Card`

Field order follows the reading view, keeping writing and reading aligned.
The editor's card fill writes each field and hands every list its items.

### `Theme.Card.Body`

The body orders definition, situations, registers, translations, examples, tags and media.
The media-add controls follow those fields.
A writer fills a card in the order a reader will meet it.
No field is labelled.
A label naming what a writer can read off the field only pushes the card down.

### `PTitle`

The title keeps the reading side's place.
The marks that drop and fold the card are laid over the strip beside it.
Given columns of their own they would narrow the title.
Both title styles prevent wrapping, so unequal widths would clip their text differently.
Its right margin is `Theme.Card.TitleMargin`, the one the reading card's title carries.

### `PCardPeek`

It shows the card's definition while the card is folded and untitled.
It stands before `PTitle` in the same cell, so it draws behind the transparent title box.
The title box stays visible when cleared, preserving focus and caret on folded cards.
The peek takes no pointer, so a click there lands in the title box.
At rest it matches the reading card's peek in style and place.

### `PCardEraser`

The eraser sits left of the hinge by `Theme.Card.EraserMargin`.
It keeps that place when the hinge is collapsed on an unsaved card.

### `PCardHinge`

The hinge is a plain `ToggleButton`, found by its contract name, standing where the reading card's hinge stands.
A press on it is taken by the button, so the header under it never starts a drag.
The card fill subscribes its click and paints its state.

### `PCardBody`

The body is named so the shared fold painter can collapse it while the card is folded.

### `PCardHeader`

The named header lets the shared fold painter replace open corners and bottom edge with closed-header resources.

### `Theme.Command.Group`

The media-add group follows stored media.
Tooltips retain the labels when the controls show only icons.
Its negative margin cancels the reserved gutter, centring controls across the body.

### `PCardPosition`

The badge is a field rather than a label, so the number a card carries can be written over.
Reordering by number reaches a place a drag has to scroll to.
The badge takes the accent ring while its card's position is open, set by the fill.

### `PCardPositionText`

The number opens for typing and the pointer while its card's position is open.
The position handler commits on Enter or focus loss, rather than treating each keystroke as a reorder.

## Stamp

Commit: `c2f335c331fe`
- `PLanguageTemplate.xaml` `0e92873e942cc0c6`
- `PMeaningTemplate.xaml` `44a74836c93b401d`
