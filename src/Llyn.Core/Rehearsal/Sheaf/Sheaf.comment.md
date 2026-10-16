# Sheaf

This folder holds the records and pure rules of flashcard cues: the cue, its cells, faces, columns and markup.
A deck is studied on a phone flashcard app, and Llyn writes the deck file that app reads.
Every record is positional and immutable, so a change replaces the record whole.
Every rule here reads only its arguments.
Times arrive as stored strings, so nothing here reads the clock.

## `LCueColumn`

The kind of phone column a cell or a template slot fills.
Text, Picture and Sound repeat once per side.
Category and Distractor repeat as numbered columns too.
The numbers are stored in `column_kind`, so a member never changes its number.

## `LCue`

One flashcard of a deck.
Each source kind has its own nullable reference, so a rebuild follows every reference.
`LCueText1` and `LCueText2` hold the side texts last exported.
They let a later import match a phone row that carries no id.

## `LCueOrphaned`

A card is orphaned when a row its face reads is gone.
It is derived at read time, so a live delete and a rebuild never disagree on it.
A free card is never orphaned.
A pair is orphaned when its target is gone, or when both its meaning and its collocation are gone.
A removed translation link alone leaves both references set, so it orphans nothing.
An unknown grain counts as orphaned, so it never reaches the phone.

## `LCueResting`

A resting card is left out of the export and out of study.
An excluded card rests, and so does an orphaned one.

## `LCueCell`

One cell of a free card, a column and its index.

## `LCueFace`

The cells one card shows, built from its entry and its deck's template.
It sits in Core because the export and the desktop study read the same face.
A cell's text is already in the phone's markup.

## `LCueMarkup`

Turns Llyn text into the phone's markup, and a face's side back into text and media for study.
It sits in Core because the export and study read it.

## `LCueMarkupFormat`

Escapes plain text for a phone cell.
A literal `|` would read as a line break, so it becomes the look-alike `∣` (U+2223).
The phone's own help advises that sign for a literal bar.
`<` and `&` pass unchanged until the phone probe pins their rule.

## `LCueMarkupParse`

One side of a face for study, as an `LCueSide`.
Only the text, picture and sound cells of that side count, in the face's cell order.
A `|` in a text cell becomes a line break, as the phone shows it.
Each text cell starts a line, and an empty cell adds none.
A picture or sound cell adds each of its `|`-separated locations to the side's pictures or sounds.
  The files stay apart from the text, so the study card draws them instead of printing their names.

## `LCueSide`

One side of a card as studied: its plain text, its picture locations and its sound locations.

## Stamp

Commit: `121691f60459`
- `LCue.cs` `73bd9d7d7a394017`
- `LCueCell.cs` `1674fada9b964088`
- `LCueColumn.cs` `b29a2835b433dbc4`
- `LCueFace.cs` `5eb4e06ec86c8a5c`
- `LCueMarkup.cs` `38e9c27a8123308f`
- `LCueSide.cs` `3d0f4841bb18b9d1`
