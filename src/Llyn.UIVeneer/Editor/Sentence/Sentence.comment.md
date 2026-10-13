# Sentence

This folder holds the editable collocation card and the Example row, as markup alone.
The editor's markup merges them, and the editor's fill writes every part and subscribes the card's events.
The collocation card shares the meaning card's header marks and fold behaviour, and adds the linked-meaning row.

## `PCollocationTemplate.xaml`

### `Theme.Collocation.Card`

Field order follows the reading view, keeping writing and reading aligned.
The editor's card fill writes each field and hands every list its items.

### `Theme.Card.Body`

The body orders expression, meaning, linked meanings, situations, registers, translations, examples, tags and media.
The media-add controls follow those fields.
A writer fills a card in the order a reader will meet it.
Only the linked meanings carry a caption, and no other field is labelled.

### `PTitle`

Its right margin is `Theme.Card.TitleMargin`, the one the reading card's title carries.
It keeps room for the eraser and the hinge.
So a long title is cut at the same width in both modes.
The marks are laid over the strip beside it rather than given columns of their own.

### `PCardPeek`

It shows the card's expression while the card is folded and untitled.
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

### `PCardBond`

It shows the collocation's linked meanings as chips, set before the situations, and only this template carries it.
The caption `Card.SenseCaption` leads the row, and the first chip is the home.
Each chip shows the meaning's path and name, and an eraser unlinks it.
The caption and the add button are boxes of a chip's height.
Each pads one line of `Theme.Card.SituationSize` text by five above and below.
They take the chip's bottom margin of seven and sit at the top of the row.
Equal boxes from one top share one centre, so caption, chips and button read as one line.

### `PBondChooser`

The add button, captioned `Card.SenseAdd`, closes the chip row.
It carries no context menu, so a right click does nothing.

### `PBondMenu`

It is the meaning picker, a themed `Popup` under `PBondChooser` like the mention picker, on `Theme.Popup.Surface`.
It does not stay open, so an outside click shuts it.
The driver fills and opens it, and shuts it on a pick or Escape.

### `PBondList`

The rows are one per meaning of the entry.
Each is a `Theme.Popup.RowSurface` that lights on hover as the mention rows do.
Its path and name parts reuse the chip's part names, because the driver fills each from the same row item.

### `PCardBody`

The body is named so the shared fold painter can collapse it while the card is folded.

### `PCardPosition`

The badge is a field rather than a label, so the number a card carries can be written over.
Reordering by number reaches a place a drag has to scroll to.
The badge takes the accent ring while its card's position is open, set by the fill.

### `PCardPositionText`

The number opens for typing and the pointer while its card's position is open.
The position handler commits on Enter or focus loss, rather than treating each keystroke as a reorder.

### `PCardHeader`

The named header lets the shared fold painter replace open corners and bottom edge with closed-header resources.

### `Theme.Command.Group`

The media-add group follows stored media.
Its negative margin cancels the reserved gutter, centring controls across the body.

## `PSentenceTemplate.xaml`

### `Theme.Sentence.Row`

It is the Example row on one line.
It holds the two frame fields, the sentence field, the citation field at its right end, and the row buttons.
Each field's hint reads the unknown mark when the row holds something it cannot read back.
So an empty-looking field says which kind of empty it is.
What is typed leaves through the editor's text handler, so the row holds no copy of it.
The two frame fields sit in the first two columns.
Each is placed by the column the row was told to take.
So a language writing the role first draws the role first, and nothing else about the row changes.
Both are typed into and offer what the workspace has already saved for the language.
Neither offers anything on a workspace that has saved none, because nothing ships a marker or a role.

### `PSentenceReach`

The fill binds the four Mention commands and the Gloss removal on the row, five bindings in all.
So both the field's menu and the chip line reach the same handlers.
The menu names the field as its target and a chip names itself as parameter.
The handlers tell the two apart by what arrived.

### `PSentenceText`

The sentence field carries the shared linking menu.
Its selection is what the menu's items read to decide whether they apply.
It is named after the row value it writes, so the text handler knows which value changed.

### `PSentenceMentionLine`

It holds one chip per Mention, collapsed while there is none.
It stands in the sentence column so it reads as part of the sentence and not of the row's controls.

### `PSentenceGlossLine`

It holds the Gloss rows under the sentence.
The fill hands it the row's Glosses and attaches the Gloss row fill.

### `Theme.Sentence.Citation`

It is the field the cited Source is typed into, showing its `Author (Year)` byline.
The byline is looked up when the field is filled, from the row's anchor and the shared Source list.
A typed line is the field's alone until the editor commits or discards it.
It is bare like the sentence field and muted, so it reads as the sentence's tail.
The fill binds its font, size and baseline drop to the sentence field and its frame.
So its box stands level with the sentence box whatever font the language pack gave the sentence.
It fades out while empty and the row is neither hovered nor focused.
So an uncited row shows no blank box.

### `Theme.Sentence.Opening`

The switch that opens a frame stands where the frame will be written, right after the dot.
Standing in the gutter, it asked to be read as a handle on the row.
It is drawn as the plus the card's other openings use.
It reads as a minus while an empty frame is open.
It gives way as soon as the frame carries a marker or a role.
A written frame is closed by clearing it.
The fill writes the sign and hides the switch once the frame is written.

### `PSentenceFrame`

The frame is written where it is read, as `(+marker role)` in front of the sentence.
It uses the sentence's own face.
Fields of their own, above or beside the line, would say the frame is a property of the row.
It is the opening of the sentence, and the reading view has always drawn it as one.
A row carrying no frame shows none, so its sentence begins where the reading view begins it.

### `Theme.Sentence.Hint`

It is the hint of an empty frame field, hidden until the fill finds the field empty.

### Frame canvas

The two frame fields are laid out where they cannot widen the line they open.
A canvas asks for no room, so the ghost copies beside it alone say how wide a field is.
The fill binds each field to the size those copies settled on.
Left to measure itself, a written field asks for a little more room than its text needs.

### `PSentenceBody`

The fill drops the sentence column so its baseline meets the frame's.

### `PSentenceControl`

Everything a writer needs and a reader does not is laid out whether shown or not.
It is faded rather than hidden, and the editor shows it while the list is hovered or focused.
Hiding it would give the row one height under the pointer and another away from it.
A card that moves while it is being pointed at cannot be read as the card it will become.

## Stamp

Commit: `c2f335c331fe`
- `PCollocationTemplate.xaml` `4e2f11414f531bd8`
- `PSentenceTemplate.xaml` `03db122236c6034d`
