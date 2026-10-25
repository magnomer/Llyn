# Display

The console's reading view of one open entry, drawn as lines of coloured pieces.
It mirrors the GUI lectern and reads the same Conduct members each GUI twin reads.

## `BLectern`

The reading page and its key table.
It owns the builders of the other files and draws the page into a given rectangle.
The incoming builder is given only the display's card gate, and the note builder the atelier.
With no entry open it resets itself and centres the unselected prompt.
The page opens with the header, grasp, speech line and glyph row.
The sections below follow the compass's shown parts, in the order Conduct sets.
So the page and the contents never disagree.
A part the compass does not show is not drawn, and a part with no builder here draws nothing.
The meaning and collocation parts share the card builder, which draws only the list its part names.
The card builder's item tables are cleared once before the sections, since two parts now draw into them in turn.
The header builder subscribes its own heart, grasp and frequency refreshes to the display.
It asks the page to repaint after each.
Each builder adds its lines, and a section without lines draws nothing.
Every line is divided at the width, and the first line of each meaning is recorded for the digit keys.
One item is lit at a time, named by a string.
It is dropped when the page no longer holds it.
Up and Down move the lit item and scroll when none is left on the shown part.
Page keys, Home and End scroll, then light the first item shown.
The digits one to nine bring a meaning to the top and light its first item.
Each shown part and card records the frame line it starts on.
So the contents and a sought card can scroll to it.
A sought card arrives from the compass event and is held until the next draw.
That draw scrolls to the card and lights its first item.
The item lit before stays lit if it still stands on the shown page.
A sought card the page does not hold is dropped, so a stale request moves nothing.
Alt and F3 opens the contents over the page, anchored at the page's right edge.
Each item kind opens through the builder that drew it, as each GUI twin handles its own clicks.
The opener answers the item to light after it, or null to keep the lit one.
The last row of the area is a muted foot.
It shows the word cursor hint, else a lit note link's address, else the lit item's text.
Enter on a note link opens its address through the atelier.
Each handler reads the one member its GUI twin reads.
An entry change reopens the entry and a workspace change closes it, each refining the reads above.
The first lit cell is kept as the anchor that places the word menu.

## `BSCompass`

The contents of the open entry, shown as a dialog over the page.
It is the console twin of the GUI compass, and both read the rows `CCompass` gives.
A section row stands flush and a card row carries its number.
A heading line is muted and cannot be lit.
Up and Down move the lit row past the muted ones, wrapping at the ends.
Enter closes the dialog and scrolls the page to the lit row's line.
The row lit on opening is the one `CCompass.CCompassLitRead` names for the page top, so both views agree.
With no row to light the dialog completes at once and shows nothing to choose.
The dialog's width is its widest row or its title, and it sits against the page's right edge.

## `BLecternLine`

One line of the page, a list of text, role and item pieces with an indent.
A piece with an item name is something the keys can light.
Dividing wraps the line at the width by words, the continuation lines hanging by 2 cells.
A piece that is an item is never split between words.
A word wider than the room is cut by cell width.
Pieces with no space between them are kept together on one row when they fit.
The trailing blanks of a last piece are trimmed, an item piece that is all blanks excepted.
Neighbouring pieces of one role and item merge.

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
Each fold item names its card by kind and id, so a meaning and a collocation never share an item.
Each see line names its row, and each chip or link what it opens.
Drawing sends nothing.
A done fold raises the display's fold change, which reads the cards again.
A refused fold leaves the cards as read, so its mark stays.
A see line always names a collocation.
It opens the row it names and lights that row, as the GUI spotlights it.
A refused open, or a row not drawn, keeps the see line lit.
A chip sends its origin, and a translation link sends its target id.
A chip of a record never saved is plain text, since it opens nothing.
An untitled card shows its kind, dim, while open and its definition while folded.
A closed row shows the meaning cut at the line end, then the markers Conduct words.
Pictures and videos are lines with the name Conduct shows, and an empty one shows nothing.
Conduct resolves no address for a file that is gone, and that verdict marks the line missing and dim.
The driver checks no file, and Enter does nothing on such a line.
Each sentence is drawn by the mention builder, so its words can carry the word cursor.

## `BLecternNote`

The entry's note, drawn from the Markdown blocks Conduct parsed when the entry opened.
The driver parses nothing, and it draws only when the header says a note stands.
Headings, lists, quotes, fenced code and rules each keep their own look, with a heading rule under a first-level heading.
A span with an address is a link item numbered in draw order.
The table is cleared on every redraw.
A link opens through the atelier's location opener and keeps the lit item, since the page stays.
The foot shows the lit link's address, so the user sees where Enter goes.

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

Commit: `36e31eb71c98`
- `BLectern.cs` `7c991dcfb630eaf9`
- `BLecternCard.cs` `5789c16738bc242c`
- `BLecternEtymology.cs` `eb1eccd28473ccfc`
- `BLecternHeader.cs` `8156c8ca5789e032`
- `BLecternIncoming.cs` `c3aa3673f641cae6`
- `BLecternLine.cs` `87521ccffaaa93a5`
- `BLecternMention.cs` `64b5db213f0e416c`
- `BLecternNote.cs` `e8dbcb45714220f4`
- `BSCompass.cs` `61b42b86d9531d1a`
