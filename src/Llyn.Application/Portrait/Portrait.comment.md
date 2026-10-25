# Portrait

The Portrait folder composes the entry page for display and sends a composed page out.
The main clerk reads each section, and the card and crest classes append sections from data already read.
The label class words the pages.
The press class saves or prints a composed page.
The kind pages of examples, references and situations are composed by their own clerks.

## `LPortraitClerk`

A failing read of any section throws to the caller.
No section is silently left out of the page.

### `LPortraitClerkRead`

The page of one entry, refused when the entry no longer stands.
Sections come in display order, then the readings with respelling and phonemic marks decided per language.
Collocations show as rows under their home meaning at any depth, each carrying its stored fold.
Each further link of a collocation shows a see-line under that meaning.
The collocations linked to no meaning follow under the Other heading as the same rows, each with its fold.
The speech chips follow the order `LEntryChip.LEntryChipRead` gives.
That order is the lexical unit, the freedom, then the named speeches.
Unit and freedom take their words from the label.
A unit or freedom the label has no word for is left out.
The stored folds are read as kind and id pairs, so a Meaning and a Collocation never share a fold.
The DOCX and Markdown writers keep the chips on one plain line.
A failing favorite check throws rather than showing the entry as no favorite.
A false `fetch` reads stored frequencies only, so a push over every entry starts no network fetch.

### Section scans

Every private scan throws on a failing read.
`LPortraitTargetScan` gives translation targets as links, keyed by entry id.
`LPortraitSourceScan` gives the citation names by reference id.
`LPortraitFanqieScan` gives fanqie rows for a language with books, and empty otherwise.
`LPortraitScriptScan` gives script images for a language with styles, and empty otherwise.
`LPortraitGlyphRead` is null for a blank language.
`LPortraitFrequencyScan` starts a fetch for an entry without rows only when fetching is allowed.

## `LPortraitClerkCard`

The card-side sections of the entry page: the meaning and collocation bands, the etymology, the incoming links and the note.

### `LPortraitBandAdd`

A band of cards under a heading, added only when there are cards.

### `LPortraitIncomingAdd`

One usage row per entry that translates into this one, each linking back to its owner.

### `LPortraitEtymologyAdd`

The etymology band, added only when the entry has an ancestor.
It carries the tree as `LPortraitSectionLineage` and the same tree as an outline in its lines.
The first line is the headword, then one line per etymon, depth first, siblings in their stored position order.
Each line's label is the relation.
Its text is language, word and quoted gloss, and an empty part leaves no gap.
Its opener is one `· ` per level below the roots, then `↑`.
A box borrowed from a linked entry's own map takes `⇡` instead.
Visible dots stand in for indentation because Markdown strips leading spaces and reads four as code.
Word and Markdown draw these lines as any band's lines.
The sheet pairs the lines after the first with the tree's boxes in that order.
It takes each box's relation from its line.
A linked etymon also adds one link, once per word and language.

### `LPortraitNoteAdd`

The note as a prose section, when there is one.

## `LPortraitClerkCrest`

Composes glyph, frequency, form, paradigm, fanqie, and script sections from already-read data.
Methods append to the supplied section list without reading storage.

### `LPortraitGlyphAdd`

A null glyph suppresses the section rather than falling back to headword chips.
Otherwise, the chips are the cells of `LGlyph.LGlyphDivide`, the rule the view uses.
The first nonempty transcription matching the glyph scheme wins.
The headword is the fallback only when it holds Han characters.

### `LPortraitFrequencyAdd`

The first non-null band supplies the only chip, even when that band is empty.
Every supplied row retains its source and raw figure, with the unit prefixed when present.

### `LPortraitFormAdd`

One line per non-empty form, its local text beside it.

### `LPortraitParadigmAdd`

Only specified inflections and localized unknown labels can produce lines, and empty rendered text is omitted.
Speech prefixes depend on distinct speech IDs across all supplied slots, including omitted ones.
Multi-value slot names retain every value rather than only the first morphology.

### `LPortraitFanqieAdd`

Initial and rime replace raw text when either exists.
Only headings receive square brackets, while readings receive slashes.
Character prefixes disambiguate books when supplied rows contain multiple characters.

### `LPortraitScriptAdd`

One plate per script image.

## `LPortraitClerkLabel`

The wording of entry and catalog pages, built from the words a caller hands in.
The kind words and the legend sit together, since the legend looks up exactly the words the kind read lists.

### `LPortraitKindRead`

The stored word of every Source kind, in the kind's own order.
A legend is worded per kind by these words, so no caller names the kinds.

### `LPortraitLabelCreate`

The label an entry page is worded with, taken word by word in the record's order.
The first 24 words fill the record's text fields, from Unknown to Tag.
The Other, Stub and Heading words stand between the Collocation word and the Incoming word.
The next three words name the units Morpheme, Word and Expression, in that order.
The last two name the freedoms Dependent and Standalone, in that order.
It throws unless exactly 29 words are given.

### `LPortraitLegendCreate`

The legend a catalog page is worded with, taken word by word in the record's order.
It throws unless exactly 13 words are given.
Each Source kind is worded by its stored word, and a kind the words leave out keeps its own name.

## `LPortraitClerkPress`

A composed page sent out, either saved in a format or printed through the press.
It composes no page itself, so the entry page and the catalog pages share it.

### `LPortraitClerkExport`

The catalog overload sends a PDF through the press as rendered sheet HTML, with every collocation row open.
Every other format is saved through the portrait port.
The entry overload takes the Livery page beside the portrait page.
HTML is saved through the portrait port's entry sheet, and PDF prints that same sheet through the press.
The HTML keeps the stored row folds.
The PDF opens every row first through `LPortraitPage.LPortraitPageOpen`, since paper cannot unfold one.
Markup, Markdown and Word keep the portrait page alone, through the other overload.

### `LPortraitClerkOpen`

A page is rendered as sheet HTML and loaded into a kept press board.
Every collocation row is open, as on any print.
The entry overload opens the same sheet its print and HTML export write.
The board stays open for the whole print session, so each setting change redraws without reloading the sheet.
The answer is the session clerk that holds the board, so the session never names the board.
The caller owns the clerk and disposes it when the session ends.
The private overload lists no printers, so a broken spooler cannot stop a print window from opening.
Its await keeps the caller's context, so the board's browser stays on the thread that opened it.

### `LPortraitPrinterRead`

Lists the installed printers through `LRigQueue`, each with its sheets made ready.
The print session calls it when its window shows, apart from opening the board.
A listing failure travels to the caller unchanged, so the window can report it.
The private overload turns sheets upright, drops repeats and names standard sheets.
Upright sheets let a sheet a driver lists wide match its standard name.
A sheet keeps the driver's own size and only borrows the standard name.
The driver's size is what the printer actually holds, so a custom print lands on a real form.
The twin only maps the driver's values, so this rule lives here.

### `LPortraitMediumRead`

Every format an entry can be exported to, in the order a chooser offers them.
Each row carries the file suffix the format is written under, and whether it is the default.
HTML is the default.

## Stamp

Commit: `8c403d1df768`
- `LPortraitClerk.cs` `144a027525b32845`
- `LPortraitClerkCard.cs` `4fb506393af6c1af`
- `LPortraitClerkCrest.cs` `5300e9670c77db79`
- `LPortraitClerkLabel.cs` `2d3cf92501455e97`
- `LPortraitClerkPress.cs` `f9f3944e31e417f1`
