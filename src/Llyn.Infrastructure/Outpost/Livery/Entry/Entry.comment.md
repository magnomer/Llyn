# Entry

This folder holds the writers of each section of one entry's Joplin note body.

## `LLiveryHeader`

It writes the head and the chips of an entry body.
It reads only `LLiveryPage` and the `lookup` it is handed.

### `LLiveryEscaped`

These are the characters `LLiveryTextFormat` escapes with a backslash, so stored text never turns into Markdown.

### `LLiveryHeaderAppend`

It writes one level-one heading holding the headword, language chip, heart, stars and rating.
The language chip is left out when the draft names no language.
The heart, stars and rating sit in one `llyn-regard` span.
The style lays that span out as a flex row, which drops the spaces between them.
So no 40-pixel heading space widens a gap between two stars.
The heart carries `llyn-on` for a favorite.
The grasp is clamped through `LGrasp.LGraspNormalize` before the stars and rating key are read.
The language chip opens with the variety flag from `LLiveryBannerFormat`, when one reads.

### `LLiveryChipAppend`

It writes the speech chips as one paragraph, then the frequency chip as another.
The speech paragraph holds the chips in the order `LEntryChip.LEntryChipRead` gives: unit, freedom, then the named speeches.
It only styles them: unit and freedom are outlined chips named through `lookup`, and the speeches are filled chips.
The trailing space is trimmed only when a chip was written.
A marked draft with no unit, freedom or named speech writes no speech paragraph.
So text written before this method is never cut.
The frequency chip names its band through the `Frequency.` key of the gauge rank.
A ranked gauge adds a band class such as `llyn-frequency-core`, and an unranked gauge adds none.
Its title holds the gauge source, one line per row.
Each band step draws one pip, and the steps the band reaches carry `llyn-on`.

### `LLiveryTextFormat`

It escapes stored text for Markdown.
Line breaks become spaces, so a value never splits a table row or a heading.
Markdown characters get a backslash first, then HTML characters are encoded.
`LLiverySound` writes every stored value through it too.

### `LLiveryHtmlFormat`

It HTML-encodes text for a raw HTML block, with no Markdown backslash.
Line breaks become spaces first, as in `LLiveryTextFormat`.
The etymology tree writes every text through it, since Joplin parses no Markdown inside HTML.

### Flags and pictures

`LLiveryFlagFormat` is `LLiveryPictureFormat` with class `llyn-flag`.
`LLiveryBannerFormat` takes the banner path `LLiveryPageBanner` holds for a language, and a language with no entry answers empty.
`LLiveryPictureFormat` embeds the file as a base64 data address, which `LLiveryImageApply` later turns into a resource.
It reads through `LPortraitAsset.LPortraitAssetLoad`, so the media type rule and size ceiling are one copy shared with the export.
A null path, or a file that loader refuses, answers empty.

### `LLiveryStarAppend`

It writes one star span per fill `LGrasp.LGraspStarScan` answers.
Core owns the fill rule, so the view's stars and these agree.
A full star carries `llyn-on` and a half star carries `llyn-half`.
At grasp zero every star carries `llyn-unrated`, so the style tells unrated from rated.

## `LLiverySound`

It writes the reading and sound part of an entry body.

### `LLiverySoundAppend`

Written reflexes split into shown and folded rows through `LSheetSound.LSheetReflexDivide`.
The HTML export uses the same helper, so both fold the same rows.
The shown rows form one table and the folded rows a second one.
The second sits inside one closed `<details>` whose summary is the `Reflex.More` text.
The old sounds read every stored fanqie row.
A two-column table then holds the accent rows, the other transcriptions and the glyph row.
An accent row with empty text is skipped.
An accent row is the sheet's opener, text and closer, with tone digits marked when the contour is toned.
Its label is the variety's flag, and the name prints when no flag reads.
The primary row pairs with the draft's first pronunciation, and any other row with the pronunciation of its id.
That recording adds a player at the row's end, where the play button stands in view mode.
The primary row's accent and its contour charts share one axis through `LSheetSound.LSheetAxisFormat`.
So the plot centres on the reading, as in view mode.
The player follows the axis block, outside it, so it never moves the plot's centre.
Each table is left out when it has no row.

### `LLiveryReadingAppend`

It writes one seven-column table with an empty header row.
The columns match the app's reflex rows, ending in the old sound.
The language cell is named on the first row of each run only, through `LSheetSound.LSheetLabelScan`.
The reading is the guise's shown text, which the engine filled from the stored text and respelling.
A row without a guise prints its stored text.
A phonemic guise wraps the reading in slashes and marks its tone digits.
A main reflex's reading carries `llyn-main`.
The note cell stays empty when no note is stored.
The old sound uses the separator shared with the HTML export.
`LLiveryXiesheng` reuses it without fanqie rows, so the old sound stays empty there.

### `LLiveryPlayerFormat`

It writes a space and an `<audio>` player opening with `LLiverySheet.LLiveryAudioHead`.
The sheet finds the player and swaps it for a resource link.
No pronunciation or an empty audio path writes nothing.

### `LLiveryToneFormat`

The tone digit rule lives in `LSheetSound.LSheetToneFormat` once, shared with the HTML export.

## `LLiveryRime`

It writes the paradigm box, the inflection table and the rime card.

### `LLiveryRimeAppend`

It writes the paradigm rows, then the inflection sheet, then the rime card.
That is the order view mode stacks them.
The `link` callback is asked under the entry draft's trimmed language, the name the courier keys its notes by.
A chip with an id is wrapped whole as a link, so the chip keeps its look.
An empty chip, key or id leaves the chip unchanged.

### `LLiveryParadigmAppend`

It writes a two-column table inside a `llyn-paradigm` div.
The right column stays empty for a row without an inflection.
A row with a speech part gets a `llyn-speech` chip row above it.
No rows write nothing.

### `LLiveryFanqieAppend`

It writes one Markdown table inside a `llyn-card` div.
A blank line sits inside both ends so Joplin reads the table.
The header row is empty, since the card shows no column titles.
Each group has a heading row, then its stems in one row after a series chip.
Every row has twelve columns, from the book chip to the source badge.
The book chip and the source badge stand on the group's first row only.
A repeated book has a blank label, so that group shows no chip.
The tone chip shows the stored label, else the tone class formatted through `Display.FanqieTone`.
A row without a rime cell writes the remainder in the rime column.
A closed medial adds `llyn-on` to its chip.
Chips are keyed so they find the category notes the rime-table pages carry.
Stems are keyed by the stem string, and initials and rimes by their cell.
Tones are keyed by the tone class even when a label shows.
The source badge is plain text, since the page carries no address for a fanqie source.

### Row and chip helpers

`LLiveryRowFormat` joins cells into one table row and is internal for `LLiveryYunjing`.
`LLiveryChipFormat` wraps escaped text in a span, or answers empty for empty text.
The escape means a stored pipe never splits a cell.

## `LLiveryInflection`

It writes the inflection sheet.
Its rows come from `LSheetInflection`, which the HTML export's paradigm box shares.
`LSheetInflection.LSheetInflectionResolve` decides between one table and the switch.
A single table sits in the same `llyn-inflection-box` div, so the box stays.
A null view, or two sheets without lines, writes nothing.
The Short/Full switch is a `details` element, which Joplin folds on a click without script.
The box has its own class, since `llyn-paradigm` is the rime box and its fill would leak in.
No class shares the rime prefix, so no rime rule reaches the sheet.
The summary holds the `Paradigm.Short` and `Paradigm.Full` captions in two spans.
The collapsed table follows the details, with the extra class `llyn-inflection-short`.
A closed details shows Short, the state the app's switch starts in.
The block is raw HTML, so Markdown never parses inside it.
Text is therefore HTML encoded only.
No blank line may sit inside the block, since a blank line ends the HTML block.
A blank line follows it, so the next writer starts a fresh Markdown block.

## `LLiveryScript`

It writes the script card.
Every script group goes into one `llyn-card` div, with a blank line inside both ends.
A group opens with its heading character, then its style, then its images in one row.
The gloss follows as a quote line.
The variant group is a style like any other, so its glyphs follow in the same card.
No groups write nothing.

### `LLiveryFigureFormat`

An image is an `<img>` with a PNG data address, which `LLiveryImageApply` turns into a resource.
An image without bytes writes no `<img>`.
The caption holds the epoch through its `Epoch.` key, then the stored caption.
An image without epoch or caption writes no caption.

## `LLiveryCard`

It writes the meanings, collocations, incoming usages and stored note of an entry body.
It reads only `LLiveryPage`, the `note` map and the `lookup` it is handed.

### `LLiveryCardAppend`

Meanings go under `Display.MeaningPlural`, each receiving the entry's collocations.
So the collocations a meaning is home to nest inside it.
Collocations with no home among the meanings go under `Display.CollocationOther` as rows.
Incoming usages are written last.

### `LLiveryDeckAppend`

It writes a level two heading, then one card per meaning.
No cards write nothing, not even the heading.

### `LLiveryFaceAppend`

It writes one card as a `llyn-card` block, followed by a blank line.
A stored card opens through `LLiveryFoldFormat`, open unless `LLiveryPageFold` holds its id under the Meaning kind.
Its head sits inside a `summary`, so a folded card keeps its number and title.
A blank line follows the head, so Markdown parses the body inside the HTML block.
A card with no stored id stays a plain div, since it has nothing to fold.
Nested child cards follow the same rule.
The title is the stored title, else the expression, else a blank placeholder with `llyn-blank`.
A card with both a title and an expression writes the expression under the head.
The meaning text goes through `LLiveryMentionFormat`, so its line breaks survive.
The content follows in view mode's order.
  - Fields, then children numbered after their parent, then body fields.
  - Then the collocations at home here, then a stub for each further link.

### Field writers

`LLiveryFieldAppend` writes what a card shows above its children.
It writes situations, registers, then translations above the examples, as in view mode.
`LLiveryBodyAppend` writes what a card shows below its children.
It writes examples, tags, then images and videos.
`LLiveryCollocation` reuses both for a row body.

### `LLiveryFoldFormat`

It opens a foldable `details` block of a class, with `open` when asked.
A `llyn-card` draws its chevron at the top right of the summary.
A `llyn-collocation` row draws its chevron before the phrase.
`LLiveryXiesheng` reuses it for each series member with reflex rows.

### `LLiveryNumberFormat`

The number is the stored position, or the place plus one when none is stored.

### `LLiverySentenceAppend`

It writes one example as a Markdown list line.
The particle and dependence open it, then the text with each linked mention as a note link.
The byline of the example's reference follows.
Each gloss follows a backslash break, opening with its language flag when one reads.
An empty sentence writes nothing.

### `LLiveryTargetAppend`

Translation targets are chips in one paragraph.
Each links its headword and adds its language, opening with the flag when one reads.
A card with no targets writes nothing.

### `LLiveryBadgeAppend`

Every non-blank text becomes a span in one paragraph.
No such text writes nothing.

### `LLiveryMediaAppend`

Each stored image, then each stored video, is its own paragraph.
A web address becomes a Markdown link, and nothing is fetched.
An image file goes through `LLiveryPictureFormat`.
A relative or unreadable image path writes nothing.
A video file becomes a player that `LLiverySheet` later swaps for a resource or drops when unreadable.
A stored video span follows as a chip.

### `LLiveryIncomingAppend`

Incoming usages go under the `Display.Translated` heading inside one `llyn-card` div.
Each usage is one paragraph linking its name to the note of its entry.
The epithet, owner chip and language follow, the language opening with its flag.
No usages write nothing, not even the heading.

### `LLiveryAnnotationAppend`

A stored note writes its `Display.Note` heading and its stored Markdown.
An empty or blank note writes nothing, not even its heading.
The sheet calls it apart from `LLiveryCardAppend`, so a section can stand between the usages and the note.
It shares the portrait's `Annotation` word, since `LLiveryNote` already names the whole Joplin note.

### Address helpers

`LLiveryAddressFormat` percent-encodes angle brackets in the address, so the link target cannot close early.
`LLiveryAddressCheck` accepts `http://` and `https://` in any case.

## `LLiveryCollocation`

A collocation at home under a meaning card is a folding row, and a further link is a one-line stub.
The collocations linked to no meaning are the same folding rows under their own heading.

### `LLiveryListAppend`

The home is found by `LCollocation.LCollocationHomeRead`, so a collocation shows in full only there.
No homed collocation writes nothing, not even the heading.
The block opens with a `llyn-heading` span worded by `Display.CollocationHeading`, with no count.

### `LLiveryOtherAppend`

It writes a level two heading, then rows in one block without a `llyn-heading` span.
The level two heading names the block.
No collocations write nothing, not even the heading.

### `LLiveryRowAppend`

It is the one row writer for a homed list and the unlinked band alike.
Each row is a `details`, open unless `LLiveryPageFold` holds its id under the Collocation kind.
Its summary holds the phrase, then any stored meaning, so a folded row still reads as phrase and meaning.
A row carries no number and no child cards, like the Portrait row.

### `LLiveryStubAppend`

It writes a stub for each collocation that links to the card as a further link.
The stub words `Display.CollocationStub` with the phrase and its home meaning's number.
A collocation with no home or no path writes nothing.

### `LLiveryPhraseFormat`

The export prints no unknown mark, so a phrase marked unknown gives way to the title.

## `LLiveryEtymology`

It writes the etymology section of an entry body, the full tree of the page's Lineage.
A page with no Lineage writes nothing, not even the `Display.Etymology` heading.
The tree repeats the entry sheet's lineage as one HTML block, `div.lineage > div.tree`.
The block has no blank line inside, so Joplin keeps it whole.
A `div.stock` holds one `div.etymon` per box.
A box with sources nests a stock of them above itself, joined by `i.stem`.
Sources are written depth-first in list order, `LLiveryStockAppend` recursing per level.
Each box writes its `span.tongue` language, its `span.word`, its `span.gloss` and its `span.relation`, in that order.
Each span but the word is written only when it has text.
A relation with a key shows that key worded through `lookup`, and one without a key shows its name.
A box with no relation, or one the page does not list, writes no relation span.
A `span.word` carries `linked` when the box points at an entry.
It carries `borrowed` when the box is read from another entry's map.
A linked word is an HTML anchor through `LLiveryLink.LLiveryEtymonFormat`, followed by an arrow in a `b`.
The anchor is HTML because Joplin parses no Markdown inside HTML.
Every text in the block is HTML-encoded only, through `LLiveryHtmlFormat` and `LLiveryEtymonFormat`, never the Markdown escaper.
`LLiveryPieceAppend` writes one optional span.
The headword closes the tree in `div.root` under a last `i.stem`.
The `## ` heading line stays above the block, and the style rules sit in `LLiveryStyle` under `.llyn`.

## `LLiveryLink`

It writes the links an entry body makes to other notes.
`LLiveryCard`, `LLiveryEtymology` and `LLiveryXiesheng` share it, so every note link has one shape.

### Link writers

`LLiveryLinkFormat` writes the escaped text as a Markdown `:/` link to the Joplin note id.
`LLiveryAnchorFormat` writes an HTML anchor, for raw HTML lines such as a card `<summary>`.
Joplin parses no Markdown there.
`LLiveryEtymonFormat` writes the same anchor with the text HTML-encoded only, for the etymology tree.
An empty note id keeps the text plain, so a target outside the workspace stays plain text.
`LLiveryTargetRead` reads an id of zero or less, or a missing note id, as empty.

### `LLiveryMentionFormat`

It swaps each linked mention in the text for a link.
Leading and trailing white space is left out, so no break opens or closes the result.
A mention that is unlinked, empty, overlapping or outside the trimmed text stays plain.
Cards hand it meanings with no mentions and examples with their mentions.

### `LLiveryBreakFormat`

Each line is escaped and joined with Markdown backslash breaks.
So a stored line break survives inside one paragraph without inline HTML.

## Stamp

Commit: `8c403d1df768`
- `LLiveryCard.cs` `d50b57d93a292313`
- `LLiveryCollocation.cs` `bbed45e8cb02d38b`
- `LLiveryEtymology.cs` `8037c0b43dfd6fec`
- `LLiveryHeader.cs` `759bcc84399d7e72`
- `LLiveryInflection.cs` `41638ef2745a54c8`
- `LLiveryLink.cs` `fe209a81b5072d60`
- `LLiveryRime.cs` `4db006890840f32c`
- `LLiveryScript.cs` `ff9a864059db6a9f`
- `LLiverySound.cs` `57129c2f5e3fc34f`
