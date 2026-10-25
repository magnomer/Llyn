# Text

The clickable sentence and the Mention chips under an editor row.
It stands apart from Menu because it draws and reports, and never opens a popup or calls the engine.

## `QMention`

The one control that draws a sentence a reader may click word by word.
It reports a click as surface values over its whole text and never talks to the engine.
Its host hands it the sentence already divided into pieces by Conduct, so it never divides a text itself.
The driver that hears the click hands the values to a gate, which decides what to open.
So the display panel and the corpus panel draw a sentence the same way.
Each answers a click its own way.
A block not yet under the window reports no click.

### `QMentionHostProperty`

The host menu is inherited down the tree, so a block inside a template still finds it.
The block reads it only to tell whether it stands under the window yet.
No surface is handed a Conduct object.

### `QMentionSentenceProperty`

It is a handle on the sentence row and not a copy of the sentence.
The reading view hands it back to its find gate, which reads the text and Mentions itself.
It draws nothing, so a change to it redraws nothing.

### `QMentionPieces`

It is the control's only content input, so every host hands pieces and no host hands a bare text.
A placeholder word arrives as one plain piece at code point zero.
So it still reports a click.
Null draws nothing.

### `OnMouseLeftButtonDown`

Where the button went down is kept, so the release can tell a click from a drag.

### `OnMouseLeftButtonUp`

A click is answered on release, and only when the pointer has not moved a drag's distance since the press.
The pointer must land on a tagged run, and that hit-test is pure surface work.
The click calls no gate, and the driver that hears the event makes the one gate call.
A drag is the reader selecting text to copy, and it opens nothing.
In the reading view the band captures the mouse on a drag, so no release reaches here at all.
The event bubbles, so a host holding many sentences listens once above them all.

### `QMentionWord`

Enter on a focused block raises this event, and its owner decides whether to start a word cursor.
If a handler marks it handled, the key is handled too.
Its argument names only the origin and holds no text.

### `QMentionWordRefine`

Sets the span of the lit word, as a unit and a length, or none.
The block then redraws, and the lit span wears the soft accent background.

### `QMentionShow`

There is one run per piece, so the whole text is present and no gap is split into words.
A piece the lit word crosses is cut into runs at the word's edges.
Only the word's run is lit.
The pieces cover the text from code point zero with no gap, so the runs in order are that text.
The span of an unlinked word is the engine's to decide at click time, from the offset.
A linked run wears the linked style.
A run standing for nothing wears the silent style.
A gap wears nothing.
Each run keeps its piece in its tag, which marks the runs that make up the shown text.

### `QMentionPlaceRead`

The window asks so the menu can hang under the found word rather than under the sentence's left edge.
The unit comes ready on the gate's offer, so the control names no Conduct type.
It walks the tagged runs adding their lengths.
So a word that starts in an earlier run than the click is found.
An empty character box answers like a unit no run holds.
A null unit, or one no run holds, answers the control's bottom left, where the popup would hang anyway.

### `QMentionRunFind`

The unit is the pointer's unit inside the run plus the lengths of the runs before it.
A pointer outside every run answers nothing.
So does a lookup made while the runs are being rebuilt, which the text block refuses.

## `QMentionArgument`

A click on a shown sentence carries surface values over the whole text the control shows.
They are that text and the UTF-16 unit under the pointer in it.
The control converts nothing, and the gate turns them into a code-point offset inside Conduct.
Only `QMention` builds it, so the origin cast never fails.

### `QMentionArgumentSentence`

The control owns the row, so the argument reads it there and holds no copy that could drift.
So a host names the row to its gate without naming the control's members.

## `QMentionChip`

One Mention as the chip line shows it under an editor row.
It is display only and never edits the span.
Editing is the gesture on the text.
Its word is cut from the current sentence text.
Its name is the headword the word stands for, or the silent mark when it stands for nothing.
Its sense is the Meaning it is narrowed to, or empty for the whole Entry.

### `QMentionChipCreate`

This maps Conduct's Mention labels to the chips a chip line shows.
It keeps the Conduct record out of the line, so the line only diffs chips.
A label carrying a key shows the key's text as its name.
Conduct chose that key for a Mention standing for nothing.

### `QMentionChipRefine`

The button takes the unlink command with the chip as its parameter.
Every chip line attaches it through `QLookItem.QLookItemAttach`.

## `QMentionCommand`

The four commands the linking gesture is asked through, one per menu item.
A fifth, the pick, carries the Entry chosen in the prospect menu back to the field it stood at.
They are routed commands because the menu is one shared resource and a resource dictionary carries no code.
The host the menu opened over binds each command where it lives.
The card row binds them on the row, and the corpus scribe on the transcript.
The same commands serve the chip line, so a chip's remove button asks the same way the menu does.

## `QMentionItem`

One row of the menu that opens at a clicked word.
It carries display strings and ids only, so the menu never reads the engine.
In Entry mode a row stands for one candidate Entry, with its headword, language and flag.
In Sense mode a row stands for one Meaning of one Entry.
It stands for the whole Entry when its sense is zero.
The flag comes from `QEnsignImage`, so the rows read as every other headword list does.

### `QMentionItemIndent`

A sub-sense is listed flat under its parent and pushed right by one step per level.
The template binds this, because a margin is the one thing markup cannot compute from a depth.

### `QMentionItemCreate`

Entry rows keep the order the engine returned.
Sense rows are named and ordered by Conduct, the whole-Entry row first, so the record only wraps them.
A Sense-mode pick reads only the sense, so these rows carry no Entry.

## `QMentionLine`

The chip line one editor row shows its Mentions on.
A card row and the corpus scribe each keep one.
It holds the chips alone and paints the ready chips its owner's driver built through `QMentionChip`.

### `QMentionLineRefine`

The engine alone owns the order, so the line copies the wanted order and never chooses one.
A line that already holds the wanted chips is left alone, so it raises no change.
Any difference clears the line and adds every wanted chip in order.

## `QMentionSelection`

Places a popup for what the user has selected in a sentence field.
Both hosts of the gesture place it the same way, so the placing lives apart from either.

### `QMentionSelectionPlace`

The place is the rectangle of the selection's first character.
A field not yet laid out answers a zero-size rectangle at the field's foot.

## `QMentionPiece`

One run of a shown sentence, carrying its text and its link state.
So the control never holds a Conduct piece.
The link state is null for plain text.

### `QMentionPieceCreate`

The conversion only copies, so the driver neither cuts nor judges a span.
A whole text without Mentions becomes one plain run at code point zero.
The corpus placeholder word comes through there, so the mention text takes only pieces.

## Stamp

Commit: `cb8ab7e8feb4`
- `QMention.cs` `29dfc15207da150d`
- `QMentionArgument.cs` `9ce0696255995ea8`
- `QMentionChip.cs` `aede12944c42533f`
- `QMentionCommand.cs` `f2076eda6b6e4b2c`
- `QMentionItem.cs` `5c0213ee77d0d487`
- `QMentionLine.cs` `d6f933705a31753e`
- `QMentionPiece.cs` `b7c7137a742199df`
- `QMentionSelection.cs` `cd56241db8d5f380`
