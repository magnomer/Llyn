# QLecternEntry.cs
Hash: `0599a91f1700e203`

## `public sealed class QLecternEntry`

The reading view's entry section, drawing the speech names, the lexical unit, the note and the stamp row.
[QLectern](QLectern.comment.md) builds it once in its constructor over the view's page.
It pulls its own parts by contract ID and subscribes its own events, so nothing is handed in late.

## `public QLecternEntry(FrameworkElement surface, CDisplay display, CAtelier atelier)`

Pulls the stamp row, the speech section, the unit line and the note section from `surface`.
The atelier answers a link followed in the note's Markdown.
The sections redraw on both the area's open and close, since a closed area shows the blank entry.
The note redraws on open only, since a closed area collapses its section anyway.

## `private void QLecternEntryRefine()`

Draws the shown speech names, the unit, the note section and the stamp row, on open and on close alike.
A closed area shows the blank entry, so every empty section collapses instead of standing blank.

## `private void QLecternUnitRefine(string key)`

Names the entry's lexical unit above its parts of speech through the key Conduct chose.
An unchosen unit hides the line, so a bare entry shows no empty label.

## `private void QLecternNoteRefine()`

The note is Markdown, drawn as blocks by `QMarkdownFace` inside the note card.
