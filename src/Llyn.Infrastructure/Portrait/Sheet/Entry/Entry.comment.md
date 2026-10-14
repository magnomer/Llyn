# Entry

This folder holds the HTML writers of one exported entry's cascade, header and body sections.

## `LSheetCascade`

It is the appearance of an entry sheet, the HTML, PDF and print export of one entry.
Every value copies the view's XAML or the Deportment line that paints it.
Catalog sheets keep `LSheetStyle`.

### `LSheetCascadeRead`

It writes the custom properties from `LSheetStyle.LSheetVariableFormat`, then every rule, then the icon rules, then `LSheetCascadePaper`.
Each embedded icon is written once per sheet, whatever the count of stars, cards and boxes.
The stars mask `star` over their colour, as `QGraspStar` draws it as an opacity mask.
The heart paints `favorite` or `favorite-marked`, the images `Theme.Favorite.Mark` swaps.
The card hinge, box switch, reflex fold and collocation chevron paint `expand` with their own colour.
Colours are always theme keys, so the sheet follows whichever theme the display shows.
The etymology tree copies `PThemeEtymology.xaml`: line-coloured stems and bars, framed boxes and the accent root.
Its card scrolls sideways on screen, as the view's does.
Its boxes wrap their text, so the tree narrows to fit the page.
The primary accent row's `llyn-axis` block is an inline-flex column.
So the contour plot centres on the delimited reading.
The reading's line keeps the 37-pixel row height.
That row's label cell sits at the top, on the same line.
So the label stays level with the reading while the plot hangs below it.

### `LSheetCascadePaper`

It is the `@page` rule, written after every other rule.
Print runs through `LPressBrowser`, a hidden WebView2 page, and prints with backgrounds.
So print keeps the screen look, with colours exact and cards and the switch as the file opens.
Print drops only the frame padding, since the 12 millimetre page margin replaces it.
The paper is the surface colour, white in the default theme.

## `LSheetEntry`

It is the sheet writer of an entry's HTML export, its PDF and its print.
It reads the Livery page beside the portrait page, so the body follows view mode.
Catalog pages and the Markdown and Word exports keep `LSheet` alone.

### `LSheetEntryFormat`

The document head is written as `LSheet.LSheetFormat` does, with `LSheetCascade` in place of the catalog style.
It adds one style rule per flag the Livery page shows.
Each flag is embedded once there, so the body names it by class instead of repeating its bytes.
The body is one `<main class="entry">` in the order of `PDisplay.xaml`.
The header, sound block, speech line and frequency pill come from the Livery page.
The paradigm box follows, where the view places it.
The lower body runs the fanqie box, the Meaning deck and the Other band.
Then come incoming, etymology, the note and the script box.
Their content still comes from the portrait sections, so only the frame differs from `LSheet`.
The Other band holds the collocations with no home among the Meanings.
It is written through `LSheetSection.LSheetSectionAppend`, so each row matches a homed collocation's row.
The portrait page picks them, so a linked collocation shows once under its home.
The glyph and frequency bands are left out, since the sound block and the pill already show them.
The form and paradigm bands are left out too, since the paradigm box replaces them.
The stamp footer closes the body.

### Speech, frequency and stamp

`LSheetSpeechAppend` writes one chip row in the order `LEntryChip.LEntryChipRead` gives.
It only styles the chips.
Unit and freedom are outlined, with no fill, a border and the muted colour.
The speeches are filled.
Unit and freedom are named through `lookup`.
A draft with no chip writes no speech block.
`LSheetFrequencyAppend` writes one pill with no heading, showing the tier name and the gauge's sources as its `title`.
The band prints lit pips and dim ones.
An unranked gauge shows its tier name alone, as the view hides the band.
The tier class lets the style colour each tier as the view does.
`LSheetStampAppend` writes the rule and the Created and Updated grid, using the Livery page's ready strings.

## `LSheetHeader`

It writes the header of an exported entry body, as `PDisplayHeader` lays it out.
Every value comes from the Livery page, and every decision from a shared Core helper.

### `LSheetHeaderAppend`

The headword takes the face and size of the headword font, so it matches the view's lettering.
Only those two values go inline, and every other look is left to the style.
A quote or backslash in the face is escaped, so the CSS string stays closed.
The reading line prints only when the page holds reading text.
The grasp is clamped through `LGrasp.LGraspNormalize` first, the same clamp the Livery uses.
The stars come from `LGrasp.LGraspStarScan`, so the export never counts them itself.
The heart is an empty span marked `on` for a favourite, and the style paints the icon into it.

### `LSheetLanguageAppend`

It writes the language pill with the banner's ensign class, or the globe when no flag reads.
This is the pick `QLecternHeader` makes between the flag image and the globe.
An entry without a language writes no pill.

### `LSheetStarAppend`

Each star is empty, marked full, half or empty, with no glyph.
The style masks the embedded `star` icon over its colour.

## `LSheetBox`

It writes the folded fanqie and script boxes of an exported entry body.
Each is a `details` element, so the reader opens and closes it without script.
The fanqie box follows the paradigm box.
It starts open only when the fanqie fold state holds, the state the view restores.
The script box follows the note and starts open only when the script fold state holds.
The section's heading becomes the summary, so the folded box still names itself.
The section is written without its heading, so the band shows no second title.
A page without the section writes no box.

## `LSheetDeck`

It writes the meaning band of an exported entry body as folding cards.
The card content comes from the portrait section, and only the frame is written here.
`LSheetDeckAppend` writes one headed band of top-level cards, or nothing when the page lacks the heading.
`LPortraitCard` makes one section per draft card in order, so the same index pairs them.
The draft card gives the id the fold state is kept under.
A section past the end of the draft list counts as unstored.

### `LSheetFaceAppend`

A stored card is a `details` element, open unless the fold set holds its id.
Its summary holds what `PLeaf` shows in the card header.
The hinge is no element, since the style paints it after the summary.
The rank always shows, and a titled card shows its title.
An untitled one shows its kind as `caption` and its first line as `peek`.
The view swaps those two as the card folds, which a later style does from the open state.
An unstored card cannot fold, so it is an `article` with a header and no peek or hinge.
The body is the same sense writer in both cases.

### `LSheetPeekRead`

It reads the peek as `QLecternCard` picks it.
A meaning card peeks its meaning, the first sense line, empty when the card has none.

## `LSheetLineage`

It writes the incoming, etymology and note sections of an exported entry body.
They follow the decks in the order of `PDisplay.xaml`.
Incoming and note are the portrait bands as they are.

### `LSheetEtymologyAppend`

It draws the etymology band as the view's tree, `Theme.Etymology.Reading`, in HTML and CSS alone, so print keeps it.
A section without `LPortraitSectionLineage` falls back to its portrait band.
The markup nests as the view does:

```
section.band > h2
  div.lineage > div.tree
    div.stock                 one row of sibling sources
      div.etymon              one column, repeated per source
        div.stock             its own sources, when it has any
        i.stem                the trunk under them
        span.tongue           language, left out when empty
        span.word             the word, .linked adds b "↗" and .borrowed dashes the frame
        span.gloss            left out when empty
        i.stem
        span.relation         left out when empty
        i.stem
    i.stem
    div.root                  the headword
```

Each column's half-bars are its `::before` and `::after`, meeting at its centre.
The first column hides the left half and the last the right, so one source draws no bar.
Each relation comes from the band's lines, in the order `LPortraitEtymologyAdd` writes them.
The linked words are marked on their boxes, so the band writes no bridge of links.
`LSheetAnnotationAppend` writes the note band whole, with its heading and card.
It has its own name because `LSheetNote.LSheetNoteAppend` writes only one Markdown body.
`LSheetEntry` calls it straight after etymology, where the view places the note.

## `LSheetEnsign`

It embeds the flags of an exported entry page, each once as a style rule.
A flag the page shows many times then costs its bytes only once.
The body names a flag by its class, which `LSheetEnsignRead` answers.
`LSheetEnsignScan` lists the distinct flag paths in the order their classes are numbered.
The entry's own banner comes first, then the other language banners, then the variety ensigns.
A path that cannot be loaded is left out, so no class points at a missing picture.
`LSheetEnsignFormat` writes one `.ensign-N` rule per path as a background data address.
A path that no longer loads gets no rule but keeps its number.
`LSheetEnsignRead` answers empty when the scan left the path out.

## `LSheetParadigm`

It writes the paradigm box of an exported entry body, as `QParadigm` draws it in view mode.
`LSheetParadigmAppend` writes one box without a heading, as the view shows none.
The slot rows come first, one per paradigm row, with the paired shown text.
The shown list runs parallel to the rows, so both share one index.
A shown tip is a localization key, worded through `lookup` into the cell's `title`.
The inflection sheets follow.
Two sheets that differ get the switch, then the Short table, then the Full table.
`LSheetInflection.LSheetInflectionResolve` decides between one table and the switch.
`LLiveryInflection` asks it too, so the export and Joplin never disagree.
A page with no rows and no sheet lines writes no box.

### `LSheetSwitchAppend`

It writes two radios, Short checked as the view starts, then their labels.
The radios sit directly in the box, siblings of the two tables.
So a style rule can hide the table whose radio is not checked, without script.
The labels sit in a switch holder, the pill `Theme.Paradigm.Switch` draws.
The ids are fixed, since an entry sheet holds one paradigm box.

## `LSheetInflection`

It writes the rows of one inflection sheet, the grid `QParadigm` draws under its switch.
The HTML export's paradigm box and `LLiveryInflection` both call it.
So the two keep one copy of the row and cell rules.
The `llyn-` class names stay, so Joplin output is unchanged.
Text is HTML encoded only, since the Joplin block is raw HTML too.

### `LSheetInflectionResolve`

It decides whether two formatted sheets need a Short/Full switch.
It returns the one sheet to show when they do not, or null when they do.
One empty sheet, or two that render the same, need no switch, since it would change nothing.
The non-empty sheet wins, preferring the expanded one.
Two empty sheets return an empty string, so the caller writes nothing.

### `LSheetInflectionFormat`

It writes the rows without the table tags, so callers wrap them and can compare two sheets as text.
A null table or a table without lines yields an empty string.
Headers, groups and labels are localization keys, so each passes through `lookup`.
An empty key writes an empty cell, as the app leaves its grid cell empty.
The header row exists only when the sheet has headers, after two empty cells for group and label.
The first line has no rule above it.
A later line with a group gets `llyn-close`, the full rule the app draws before a group.
Any other later line gets `llyn-rule`, the dotted rule that skips the group column.

### `LSheetCellFormat`

It reads the cell's ready text and tip, which the Core view build resolved for the app box too.
A cell with a tip is a `llyn-muted` span of its stand-in mark, titled with the tip text.
That is how the app shows a muted cell with a tooltip.
A text form writes the pieces the form scan cuts, the same pieces `QParadigm` paints.
A marked piece becomes a `llyn-marked` span.
A piece followed by the split adds a `llyn-cut` hyphen after it.
A text form with empty text writes an empty cell, the gap where the layout has no slot.

## `LSheetSound`

It writes the reading and sound block of an exported entry body.
It also owns the reading decisions that the Joplin writer `LLiverySound` shares.
Those that write markup take an escape function, so HTML and Markdown escape their own way.
The anchor separator is a middle dot with a space on each side.
The tone joiner stays inside a tone run when a digit follows it, as in a sandhi contour.

### `LSheetSoundAppend`

The block follows the order of `PDisplay.xaml`.
The shown reflex rows come first, then the folded ones in a closed `<details>` summarised by `Reflex.More`.
Then one table holds the accent rows, the other transcriptions and the glyph cells.
The primary accent row comes first, with its contour charts under its reading through `LSheetAxisFormat`.
An accent row with empty text is skipped.
The sheet's opener and closer, when set, wrap each accent text as brackets.
Accent text marks its tone digits when the contour is toned.
Each accent row shows its variety's ensign class, else the variety name.
The flag keeps the name as its `title`, so the name still reads on hover.
The other transcriptions are labeled by scheme.
Recording players stay out, as audio controls are not part of the body.
A page with nothing to show writes no block.

### `LSheetReflexDivide`

It splits the written reflexes into shown and folded rows, in draft order.
A reflex's guise decides whether it folds.
The page's folded language set decides when no guise stands at its index.

### `LSheetLabelScan`

It returns the language cell of each reading, in order.
The language is named on the first row of each run of one language, as the view leads a run.
Every later row of the run gets an empty cell.
This export and `LLiverySound` both read it, so the rule has one owner.

### `LSheetSlashFormat`

It wraps a phonemic reading in slashes, unless it already starts with one.

### `LSheetAxisFormat`

It puts the delimited reading and its contour plot on one vertical axis.
View mode centres `PContour` on the reading the same way.
The reading markup arrives written, so each channel keeps its own brackets and spans.
With no contour it returns the reading unchanged.
Otherwise one `llyn-axis` span holds the reading in a plain span, then the `LSheetContourFormat` charts.
The centre is the reading's alone: a Joplin audio player stays outside the span, so it never shifts the axis.
A plot wider than the reading overhangs it on both sides, as in the view.
The block itself is never centred in the page.
It keeps its cell's left alignment.

### `LSheetContourFormat`

It writes one `llyn-contour` span holding one chart image per syllable, in order.
Joplin's sanitizer drops every inline `<svg>`, so each chart is a standalone SVG in an `<img>` data address.
An image sees no page CSS, so every part carries its colour from the theme.
Each chart has a grid line for every level of the scale, top level first.
Only the first chart prints the level numbers, as view mode shows them once at the left.
The syllable's levels, clamped to the scale, form one polyline with a dot at each point.
A single level draws a flat line across the chart.
Each level takes its colour role from `LContour.LContourRoleRead`, the pick view mode draws from.
A line through several colours strokes a `linearGradient`, so the colours fade like the view's line.
Each image is its own document, so the gradient id needs no index.
The face name and the syllable text pass through `escape`.

### `LSheetToneFormat`

It escapes the text through `escape`.
When toned, each run of digits sits inside a `llyn-tone` span.
A joiner between two digits stays inside the run.

### `LSheetReflexAppend`

It writes one reflex table with the columns of the view's reflex row, ending in the old sound.
An empty list writes no table.
Each row's language cell is its label from `LSheetLabelScan`.
The reading is the guise's shown text, or the stored text when no guise stands.
A phonemic guise slashes the reading and marks its tone digits.
A main reflex's row carries the `main` class.

### `LSheetRoleFormat`

It maps a contour colour role to its theme colour key by name.
An unknown role throws, so a new Core role cannot pass uncoloured.

## Stamp

Commit: `aae5349dbb94`
- `LSheetBox.cs` `b32b74e843b28e08`
- `LSheetCascade.cs` `c1ae5e1cfac739da`
- `LSheetDeck.cs` `b6f318017bcde83c`
- `LSheetEnsign.cs` `27a403c779ddd2cc`
- `LSheetEntry.cs` `e2969340032e811c`
- `LSheetHeader.cs` `51370f27bc7b37bc`
- `LSheetInflection.cs` `e0cadba77db7f1cb`
- `LSheetLineage.cs` `e1c16782c82b9d2c`
- `LSheetParadigm.cs` `210fb09dca0eec79`
- `LSheetSound.cs` `83315197f8ecdfed`
