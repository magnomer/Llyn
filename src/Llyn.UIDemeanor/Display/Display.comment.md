# Display

The console's reading view of one open entry, drawn as lines of coloured pieces.
It mirrors the GUI lectern and reads the same Conduct members each GUI twin reads.

## `BLectern`

The reading page and its key table.
It owns the builders of the other files and draws the page into a given rectangle.
The incoming builder is given only the display's card gate.
With no entry open it resets itself and centres the unselected prompt.
The page order is header, grasp, speech and frequency, glyph row, meanings, other collocations, links here, etymology, stamps.
Each builder adds its lines, and a section without lines draws nothing.
Every line is divided at the width, and the first line of each meaning is recorded for the digit keys.
One item is lit at a time, named by a string.
It is dropped when the page no longer holds it.
Up and Down move the lit item and scroll when none is left on the shown part.
Page keys, Home and End scroll, then light the first item shown.
The digits one to nine bring a meaning to the top and light its first item.
Each item kind opens through the builder that drew it, as each GUI twin handles its own clicks.
The opener answers the item to light after it, or null to keep the lit one.
The last row of the area is a muted foot.
It shows the word cursor hint or the text of the lit item.
Each handler reads the one member its GUI twin reads.
An entry change reopens the entry and a workspace change closes it, each refining the reads above.
The first lit cell is kept as the anchor that places the word menu.

## `BLecternLine`

One line of the page, a list of text, role and item pieces with an indent.
A piece with an item name is something the keys can light.
Dividing wraps the line at the width by words, the continuation lines hanging by 2 cells.
A piece that is an item is never split between words.
A word wider than the room is cut by cell width.
The trailing blanks of a plain last piece are trimmed, and neighbouring pieces of one role and item merge.

## `BLecternHeader`

The headword with its heart, the speech line with frequency, the grasp stars, the glyph row and the stamps.
The grasp is unread only before the first entry opens, and the step it shows then is the lowest.
The heart, grasp and frequency are items, and the frequency text shows in the foot when lit.
The glyph row draws the scheme label, then each character.
A linked character is an item naming its cell, and a glyph read that shows nothing draws no line.
A linked character opens its glyph through the sound display, as the GUI does.
Every mark comes from the glyph sets with a plain fallback.

## `BLecternCard`

The meanings and the other collocations of the open entry.
The cards are read when the entry opens or closes and when a fold changes.
Only a stored card folds, so an unsaved card's header is no item.
Each fold item names its card, each see line its row and each chip or link what it opens.
Drawing sends nothing.
A done fold raises the display's fold change, which reads the cards again.
A refused fold leaves the cards as read, so its mark stays.
A see line opens the row it names and lights that row, as the GUI spotlights it.
A refused open, or a row not drawn, keeps the see line lit.
A chip sends its origin, and a translation link sends its target id.
An untitled card shows its kind, dim, while open and its definition while folded.
A closed row shows the meaning cut at the line end, then the markers Conduct words.
Pictures and videos are lines with the name Conduct shows, and an empty one shows nothing.
Conduct resolves no address for a file that is gone, and that verdict marks the line missing and dim.
The driver checks no file, and Enter does nothing on such a line.
Each sentence is drawn by the mention builder, so its words can carry the word cursor.

## `BLecternMention`

The word cursor on the example sentences of an open card.
Each drawn sentence is an item remembering its sentence id, and the table is cleared on every redraw.
Enter on an idle sentence starts the cursor at its first word.
While the cursor is active, Left and Right move by word and Ctrl with them seeks.
Enter finds the mention under the cursor and shows the menu below it, and Escape stops.
The cursor stays active only while its sentence is the lit item.
It resets when the entry opens or closes.
Only the sentence holding the cursor is split at the cursor, so each word is its own item there.
A linked piece draws as a link, a plain one as text, and a muted one dim.
The menu is placed under the lit cell, shifted left by the width of the text before the cursor.

## `BLecternEtymology`

The ancestor tree of the open entry, read when the entry opens or closes.
It draws a heading, the headword, then one row per ancestor with tree marks.
An ancestor that is linked to an entry is an item that opens it.
A row lists language, text, gloss, relation and borrowed mark, skipping what is empty.
A relation with a catalog key is translated, and one without shows its own name.
Wrapped rows keep the tree trunk running down through the continuation lines.
An entry with no ancestors draws nothing.

## `BLecternIncoming`

The usages that link to the open entry, under the translated heading.
They are read from the display's card gate when the entry opens or closes.
Each row is an item naming its usage and opens it through navigation.
The name and its epithet stay apart, with the epithet dim.
No usage draws no heading.

## Stamp

Commit: `cb8ab7e8feb4`
- `BLectern.cs` `f87f06f1e21e21df`
- `BLecternCard.cs` `18bd8463600c7ea6`
- `BLecternEtymology.cs` `eb1eccd28473ccfc`
- `BLecternHeader.cs` `cadac381d7e22395`
- `BLecternIncoming.cs` `c3aa3673f641cae6`
- `BLecternLine.cs` `72d24daa8956f064`
- `BLecternMention.cs` `64b5db213f0e416c`
