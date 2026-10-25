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

## `TLecternCardRun`

The one door the reading-view tests send keys and read the page through.
It builds a whole window on a fake console and sends each key through `BWindow.BWindowKeyObserve`.
The seed saves through a callback that stores an English entry and answers its id.
With `open`, Enter on the first index row opens the entry before the check runs.
A script may run on the workspace after the tab opens, to break a table for a failure test.
A notice text makes a helper close any dialog that shows it, as a user would.
Each step runs work on the loop, lets the loop drain what the work posted, then draws the frame.

## `TLecternEtymology`

Covers the etymology section of the reading view.
It draws the headword on top and the sources below as a tree.
A box shows its language, text, gloss and relation, and a borrowed ancestor carries the borrowed mark.
Enter on a box that links an entry opens that entry.
A long gloss wraps in the text column and keeps the tree trunk beside each wrapped row.
An entry with no etymology draws no heading.

## `TLecternHeader`

Covers the header, the grasp, the frequency chip, the stamps and the glyph row.
Enter on the heart and F6 toggle the favorite, and the heart wears its lit role on either side.
Right and Left move the grasp one step, and Left at the lowest step clears it to Not rated.
A frequency chip shows its rung in its own role.
The source name shows only while the chip is lit.
Created and Updated stamps sit below the header in the muted role.
A Korean entry with a Hanja row draws one chip per character, and Enter on one opens its glyph entry.
With a new draft open or closed meanwhile, Tab still leaves the focus on the index.
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
A tag or situation chip routes to Taxonomy or Repertoire, which the console has no panel for.
So navigation refuses it and the entry stays open with the chip lit.

## `TLecternMention`

Covers the word cursor on an example sentence.
Enter on a lit example starts the cursor on its first character and shows the word foot.
Left and Right step one text element, and Control with an arrow jumps to a word or a Mention piece.
A Chinese sentence with no spaces steps one character at a time.
Escape leaves the cursor, keeps the line lit, and Enter starts it again from the first character.
Enter on a word held by one entry opens that entry and asks nothing.
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

Commit: `8c403d1df768`
- `TLectern.cs` `d4aae2072b528891`
- `TLecternCard.cs` `7d68406a05439905`
- `TLecternEtymology.cs` `15a281b7d55640c7`
- `TLecternHeader.cs` `fe35c98740c71504`
- `TLecternIncoming.cs` `e1210568240b314b`
- `TLecternMention.cs` `0b79af8ed9fcc6e1`
- `TLecternMentionMenu.cs` `49dfccbd5a869061`
