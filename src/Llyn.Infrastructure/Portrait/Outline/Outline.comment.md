# Outline

This folder holds the Markdown rendering of a page likeness, with its sections and note.

## `LOutline`

It renders a page likeness as Markdown.
Markdown carries no theme, so only the reading order and the emphasis survive.
The title heads the document, with the star after it when the page is a favourite.
The language, reading lines and chips share one crest line, because Markdown has no chips.
Each reading line writes its label in italics and its text in bold, the marks inside the bold and unescaped.
The sections follow in the order the likeness carries them.
`LOutlineNormalize` guards entry text, where a stray hash or asterisk would silently restructure the document.
Each guarded character gets a backslash rather than being stripped, so nothing the reader wrote is lost.

## `LOutlineNote`

It carries the note into the outline, where it is already the right language.
`LOutlineNoteAppend` writes the normalized note text with no escaping.
A heading is demoted two levels, so the note stays under the outline's own `##` band.
Lines inside a code fence are left alone.

## `LOutlineSection`

It renders one section node as Markdown and recurses into its children.
The role of the node decides its form, so a situation, a register and a tag keep their own marks.

### `LOutlineSectionAppend`

Card, kind, phrase, scene, tone, label, bridge, quote, usage, list and row each have a form.
A collocation list is a heading leveled by its depth as a card's is, then its rows.
A row is its phrase in bold, its meaning, then its fields, always open, since Markdown has no fold.
Any other role is a band.
A scene is an italic run, and a tone and a label are code runs.
A bridge is the arrow run of the shared body.

### Bands and cards

A band is a second-level heading, the chips as code runs, one bullet per line, then the shared body.
The card heading holds the position and heading, so a reader can cite a card by number.
The heading level deepens with nesting and stops at six, which is all Markdown has.
A phrase standing first among the children is written before the lines, as the panel shows the expression.

### Quotes and usages

A usage is one list item with the headword bold and the card title after a dash.
The kind and language close it in brackets.
A quote is one list item whose text is the first line.
The further lines, the chips and every link nest one level deeper as sub-items.
A child of another role, the note and the plates follow as any section's would, so nothing is dropped.
A quote with no line writes nothing, since there is nothing to quote.

### Body and helpers

Each image is a picture link and each video a play link with its span.
The shared body is the links as an arrow run, the note unescaped, the children, then the plates.
A blank line closes a run of quote or usage items, so the next paragraph does not join the list.
A code run needs no escaping, so a chip in backticks is written as the reader wrote it.
Any other run is escaped like every field.
A line writes its label in bold before the text, or the text alone when it has no label.
A link is an arrow, then each headword with its language in brackets, separated by dots.

## Stamp

Commit: `c2f335c331fe`
- `LOutline.cs` `04c1fe57bee401a5`
- `LOutlineNote.cs` `fdd8eea1b5827283`
- `LOutlineSection.cs` `08c5cda1dd64e535`
