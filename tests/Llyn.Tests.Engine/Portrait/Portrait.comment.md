# Portrait

These tests pin the portrait, the page tree read from a stored row, and every format written from it.
They stand apart from the lexicon tests because they test what a reader sees and what each export carries.
Stored pages are read from a real workspace, and the press is always the fake one.
Shared samples live in `TPortraitSample`, so every export answers to the same entry.

## `TPortraitSample`

The entry likenesses every export test shares.
`TPortraitRowCreate` builds one meaning card holding a list of one folded collocation row, its meaning and a register field.
It catches a writer that drops a folded row's fields or folds a row in print.

`TPortraitSampleCreate` carries the common section fields, including the awkward cases.
Collocation lists and rows live in `TPortraitRowCreate` instead.
Angle brackets and an ampersand catch a writer that forgets to escape.
A titleless collocation catches a writer that forgets the kind label.
An example row with a gloss, a source and a mention link catches a writer that skips nested rows.
A hosted video catches the poster frame path.
A usage row under Links catches a writer that flattens the four parts of an incoming link.
The reading line carries its slashes as marks, so a writer that ignores the marks prints a bare reading.

## `TPortraitEntry`

Covers the entry likeness, where one saved entry is read into the section tree every export writes.
The tree holds title, language, reading line, speech chips, the numbered card, its example row and the note.
They come in the panel's order.
A chosen unit's word opens the speech chips, and the chosen freedom's word follows it.
Both stand ahead of the parts of speech.
A Latin headword with no saved glyph row prints no glyph section, as the view shows none.
A Han headword prints its characters there.

A collocation's first link decides the meaning it prints under, and no other-collocations band appears.
It prints as an open row, headed by its phrase.
The list is headed by the plain collocations heading, with no count.
Each further link adds a see-line under its meaning.
A collocation linked to no meaning shows under the Other heading as a row, not a numbered card.
That row carries the phrase as heading, the meaning as its line, and its stored fold.
A stored fold reaches the collocation's row, which still carries its meaning.

With respelling on, the reading line prints each respelling in place of its IPA.
The page follows the setting at read time, so one entry shows either form without a new save.
A linked Ancestor box prints as an up-arrow line with its language and the linked headword.
The linked entry's own box nests beneath it behind a borrowed arrow, with its language and gloss.
A closing line points to the linked entry.
The entry sheet draws the same ancestry as a tree, the borrowed box stacked above the linked one.
The headword closes it as the root, and the sheet prints no `⇡` mark.
An entry that no longer stands is an error, not an empty page.

## `TCitationPage`

Covers the pages the citation clerks compose for an Example, a Source and a Situation, read without a workspace.
Each carries the rows the read areas show, so the engine's page facade has nothing left to decide.
The sentence heads an Example page, the language is its own field, and the tally is its chip.
The gloss and source are its sections.
An unwritten sentence heads its page with the legend's word and carries no section.
A Source page joins the credits on one line, and the year and address each take a section.
A Source whose authors were ruled unknown shows the mark under the authors heading and nothing else.
A Situation page shows the kind as a chip beside the tally.
Its description rides as a note so it draws as blocks.

## `TPortraitPage`

Covers the page likeness, where an example, a source or a situation is read into the shape the sheet prints.
It covers the engine read for each realm, the HTML rendering, and the press opened for print.
An Example page holds sentence, language, tally, glosses and the cited source, in the excerpt's own order.
A sentence never written heads the page with the legend's word, and empty sections are left out.
A Source page holds title, kind chip, tally, credited authors and the written fields, with the unwritten note left out.
A Situation description arrives as Markdown, so the sheet draws it as blocks rather than one line.
A page for a row that does not exist throws, rather than reading as an empty page.

Opening a page for print clears a row fold nested under a band, a card and a list.
The row keeps its heading and its fields, so only the fold changes.
The sheet carries title, chip row, labelled lines and the note, with reader text escaped.
The rendered page opens a board on the press, and nothing prints yet.
The print window then decides every setting, so the engine hands the press none.
The fact disposes the relay, so the board never outlives it.

## `TPortraitClerk`

The portrait clerk composes a page over a real rig.
The press clerk then opens that page on the fake press.
The page of a stored entry is rendered as sheet HTML and opened on the press.
Nothing is printed, since only the print window asks the board to print.
The fact disposes the opened session clerk and its board, so no press session outlives it.

## `TPortraitExporting`

Covers the portrait exports from a stored entry to a file, one format per test.
The PDF export and the print are checked at the workspace's fake press instead.
The expected strings are read from the page tree, since a writer walks nodes and never the draft.
So a new node field the reader fills turns every format red until its writer carries it.

The exemplar written as an HTML file carries every text the page tree shows.
A node field the writer forgets turns this red, so no export silently drops a string.
The HTML body follows view mode, so it skips what the view never shows as such.
`unshown` names those texts outright.
The glyph, frequency, form and paradigm bands drop whole, since the view draws them in shapes of its own.
Two joined lines of the reflex drop too, its label line and its text line.
The view splits each of them into cells.
Two joined etymology lines drop as well, since the tree writes each part of a box apart.
The Markdown and Word checks pass no list, so they still answer for every text.

The PDF export hands the press the entry sheet, not the portrait sheet, so the PDF follows view mode.
The fake press records the html and the path it was handed.
Print opens a press board over the same entry sheet, with its headword.
The fake press records that a board was opened, and nothing prints until a window asks.
The Word package is opened as a zip and only the main document part is read.

The other direction is also pinned.
Every typed string must reach the page tree, unless `TExemplarHidden` names it.
A hidden string that shows anyway is stale, so the list never outlives the reader.
The child card is found once in the whole tree, and that one sits under its parent.

### `TPortraitExportMatch`

One page string is named outright, so an empty tree cannot pass by having nothing to check.
A top-level band whose heading `unshown` names is left out with all its texts.
Any other text `unshown` names is skipped alone.

### `TPortraitExportFind`

Collects every section headed with the heading across the tree.
It never descends into a match, so a nested card is found under its parent only once.

## `TPortraitFolio`

Covers the Word export, whose package is written by hand.
A folded row prints the list heading, its phrase, meaning and fields, in that order.
Every part is parsed, because an unbalanced tag makes a file Word simply refuses to open.
That failure is invisible until someone tries, so it is caught here instead.
The same fields the page carries must reach the document.

## `TPortraitOutline`

Covers the Markdown rendering.
A folded row prints open under its fourth-level list heading, as its bold phrase, meaning and tone chip.
Markdown carries no theme, so order is the only fidelity it has.
An unescaped angle bracket, hash or asterisk in field text would silently restructure the document.
Every character the formatter escapes is checked in the title, language and chip, and through the normalizer directly.
The note is already Markdown, so its emphasis and list marks pass into the page as written.
Escaping it as field text would show raw asterisks in place of a note the user formatted.

## `TPortraitVista`

Portrait export and the print press resolve content from the view's selection.
An empty Entry selection writes no file and opens no press.
A catalog selection resolves the catalog's own subject.
Exporting with a chosen Entry writes a file holding its headword.
Opening the press after another Entry was chosen and then cleared hands it nothing.
The opened sheet holds no trace of the cleared one.
A selected Example in the corpus view opens the press over the Example text.

## Stamp

Commit: `8c403d1df768`
- `TCitationPage.cs` `bbba4a3d7bb84873`
- `TPortraitClerk.cs` `c69cf83f9713e596`
- `TPortraitEntry.cs` `0ba74d6b0bd111a9`
- `TPortraitExporting.cs` `bc7bcef2e8061fb9`
- `TPortraitFolio.cs` `e9389e56a2c4ecb1`
- `TPortraitOutline.cs` `3312559e8e1f3164`
- `TPortraitPage.cs` `6bb628aabbfd96d7`
- `TPortraitSample.cs` `741c3d7a438cdc57`
- `TPortraitVista.cs` `22284381645c0ddc`
