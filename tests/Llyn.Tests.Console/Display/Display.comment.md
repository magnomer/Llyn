# Display

Tests of the console reading view, mirroring `src/Llyn.UIDemeanor/Display/`.
Each test runs a whole window on a fake console and reads the page from the cells it drew.

## `TLectern`

Covers the reading view's frame, focus and scrolling inside the Library tab.
With no entry open, the view centres its prompt, takes no key and lights nothing.
Enter on the first index row keeps the Library drawn and moves the focus to the reading side.
Tab moves the focus between the index and the reading side, and Enter on the reading side selects.
The reading side holds the keys only while the viewer shows it.
So a letter or Enter it does not bind never falls through to the index.
Down and Up walk the items, the heart first, then the grasp, then the first Meaning.
Page and End keys scroll and light the first item in view, and Home returns to the top.
At the end the Created and Updated stamps are on the page.
A frequency read that fails shows its notice once, and the rest of the header still draws.
An entry edited elsewhere shows its new headword at once.
A row whose entry cannot be opened keeps the focus on the index and the prompt on the page.
Only an opened entry moves anything, as in the GUI.

## `TLecternCard`

Covers the card sections of the reading view, with their fold, see, follow and digit keys.
A stored Meaning shows its header, definition, collocation, tag, situation, register, translation and example.
An image whose file is gone shows its name with the missing mark.
Space on a header folds its card to the peek line, and Enter opens it again.
A refused fold keeps the mark, shows its notice once and leaves the header lit.
A collocation linked to both meanings homes under the first and leaves a see line under the second.
Enter on the see line lights the home row, as the GUI spotlights it.
A refused fold, or a row hidden under a folded card, keeps the see line lit.
A digit lights that Meaning's header, and a digit past the last Meaning changes nothing.
Enter on a translation opens the target entry, and a deleted target shows no link.
An unlinked collocation folds to a closed row holding its phrase, definition peek and example count.
A collocation's fold is stored apart from a meaning's, so the refused-fold case breaks the collocation fold table.
The door can take its own atelier builder, so a case can observe what the atelier opens.

## `TLecternCardRun`

The one door the reading-view tests send keys and read the page through.
It builds a whole window on a fake console and sends each key through `BWindow.BWindowKeyObserve`.
The seed saves through a callback that stores an English entry and answers its id.
With `open`, Enter on the first index row opens the entry before the check runs.
A script may run on the workspace after the tab opens, to break a table for a failure test.
Before the script runs, the door waits up to ten seconds for every seeded headword to show on the grid.
So the script cannot break a table before the index has read it.
A notice text makes a helper close any dialog that shows it, as a user would.
The door answers how many times the helper closed that notice, so a case can pin it once.
The helper sends Escape through `TConsoleFakeSettle` with the loop and the batch, and waits on no frame.
Each step runs work on the loop, lets the loop drain what the work posted, then draws the frame.

## `TLecternCompass`

Covers the contents dialog and the full view of the reading side.
Alt and F3 opens the dialog over the page, listing the shown sections with each card under its number.
The row lit on opening is the one for the page top, so scrolling to a Meaning lights that Meaning.
The dialog opens on the draw after the key.
So the test waits until a row is lit before it presses on.
Down moves the lit row and Enter closes the dialog, scrolls to the row and lights the card's header.
Escape closes the dialog and moves nothing.
A part the entry does not hold, such as a note or Other collocations, has no row.
An Other collocation lists under its own heading.
F3 widens the reading side over the whole window.
F3 again gives the index back, and so does Escape.
Enter on a see line far above scrolls to the row it names and lights its header.

## `TLecternEtymology`

Covers the etymology section of the reading view.
It draws the headword on top and the sources below as a tree.
A box shows its language, text, gloss and relation, and a borrowed ancestor carries the borrowed mark.
Enter on a box that links an entry opens that entry.
A long gloss wraps in the text column and keeps the tree trunk beside each wrapped row.
An entry with no etymology draws no heading.
The page order of the meaning, links, etymology and note headings follows the compass's page order.
The stamps come after the note.

## `TLecternNote`

Covers the note section of the reading view.
Every block kind keeps its own look and role, with a rule under a first-level heading.
A long code line is cut at the width and never wrapped.
Enter on a note link opens its address through the atelier and keeps the link lit.
The foot shows the address.
A scheme the page refuses draws as plain text, gets no item and opens nothing.
A drawn item opens the address read when it was drawn.
An entry with no note draws no heading.

## `TLecternHeader`

Covers the header, the grasp, the frequency chip, the stamps and the glyph row.
Enter on the heart and F6 toggle the favorite, and the heart wears its lit role on either side.
Right and Left move the grasp one step, and Left at the lowest step clears it to Not rated.
A frequency chip shows its rung in its own role.
The source name shows only while the chip is lit.
Created and Updated stamps sit below the header in the muted role.
A Korean entry with a Hanja row draws one chip per character, and Enter on one opens its glyph entry.
While a new draft shows, Tab moves the focus to the editor.
After the draft closes, Down still lights a row.
Line division wraps long text at a word with an indent and keeps an item whole.
Each row of an item drops its trailing space.
A word longer than the width is cut into pieces, and a width of zero draws nothing.

## `TLecternIncoming`

Covers the Links here section, picture and video rows and the chips that reach a missing tab.
A usage draws one row per linking entry, and Enter on it opens that entry.
An entry nothing links to draws no heading.
An empty picture or video row draws no line, as the GUI hides it.
A video whose file is gone shows its name, the missing mark and its span in the muted role.
Conduct resolves no address for a file that is gone, which is the GUI's rule for drawing no preview.
The driver checks no file, and Enter on that line does nothing.
Enter on a see line lights the target Meaning.
A stored tag or situation chip routes to Taxonomy or Repertoire.
With no panel for that tab, navigation refuses it and the entry stays open with the chip lit.
With the Taxonomy panel built, Enter opens that tab and Alt and Left comes back to the entry.
Following a translation and pressing Alt and Left also returns to the entry that held the link.
A chip of a record never saved is no item, so Down never lights it.
The test strips the situation's id through a wrapped vista port, to make that record unsaved.

## `TLecternMention`

Covers the word cursor on an example sentence.
Enter on a lit example starts the cursor on its first character and shows the word foot.
Left and Right step one text element, and Control with an arrow jumps to a word or a Mention piece.
A Chinese sentence with no spaces steps one character at a time.
Escape leaves the cursor, keeps the line lit, and Enter starts it again from the first character.
Enter on a word held by one entry opens that entry and asks nothing.
A Mention stored with a Meaning scrolls to that Meaning's card, far below, and lights its header.
A stored Mention opens its entry without a menu, even when other entries share the headword.
Leaving the sentence with Down and coming back with Up does not bring the cursor back.
Opening another entry whose item has the same name drops the cursor.
A wrapped sentence scrolls with the cursor, and Enter on its last word opens that entry.

## `TLecternMentionMenu`

Covers the menu a word held by several entries opens.
The menu opens under the word, headed `Which entry?`, and lists each entry with its language.
The first row starts lit, and Enter opens that entry and closes the menu.
Down and Up wrap round the rows, and Enter opens the lit one.
Escape closes the menu and keeps the cursor on the word.
A navigation to an entry while the menu stands closes the menu too.
A word no entry holds opens no menu and keeps the cursor.
Showing an absent offer or an offer with no entries opens no box at all.

## Stamp

Commit: `9377e657b395`
- `TLectern.cs` `1edbbbd3ae748f4f`
- `TLecternCard.cs` `efee17dd5ad612eb`
- `TLecternCompass.cs` `2d3e92419209f202`
- `TLecternEtymology.cs` `d945e9065efda48e`
- `TLecternHeader.cs` `84f6d50e8d233eaf`
- `TLecternIncoming.cs` `2f99aaf7a781d739`
- `TLecternMention.cs` `2cd62cc685fe8080`
- `TLecternMentionMenu.cs` `02d139d4424f3906`
- `TLecternNote.cs` `45f0a74a6152dcd1`
