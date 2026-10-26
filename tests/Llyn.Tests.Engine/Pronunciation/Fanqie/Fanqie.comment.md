# Fanqie

This folder holds the tests of the rime-book placements.
They cover fanqie spellings with their rank and source, rows, diwei and the hypothesis.

## `TDiwei`

Covers the Diwei category's own rules, apart from any store.

### `DiweiRankRead_RimeDivisionOrder`

Divisions rank in table order, an unlisted division after them, and a blank heading last.
Places rank as the hypothesis lists them, an unlisted place after them, and a blank heading last.
With no hypothesis every place is unlisted, and a negative line rank normalizes to last.

### `DiweiSectionScan_TallyUnderHeading_KeepsOnlyTheShownSet`

A section's tally rows carry only the marks of the set the page shows.
A line with no mark in the respelled set is left out of a respelled section.

### Code point ordering tests

Characters, headings and labels list in code point order, never UTF-16 order.
An ideograph beyond the basic plane sorts after the compatibility block.
A character met twice is listed once.
Headings tied on rank follow by code point, and the blank heading still comes last.
Lines of equal rank list by label, and two lines sharing a label list unrounded first.
A tally mark lists its characters in code point order, whatever order they were gathered in.

### `DiweiSectionScan_DivisionHeading_LabelsItByTheLocalizedPattern`

A division section is headed by the localized pattern, filled with the division and its Roman numeral.
Without a pattern the raw division heads the section.
The localizer is handed in, so the label never depends on the catalog another test loaded.

## `TEngineFanqieFault`

A fetch whose source throws hands that fault to the owner once.
The owner records it in the audit log.
Its audit-log reader serves the sibling fault tests.

## `TEngineFanqie`

Covers the engine's fanqie fetch over a fixture pack with two books behind stubbed rime tables.
A start posts each book's form and reads every cell carrying the marked character into one line.
The line joins the row's initial, the cell's rime with its rime heading, and the column's heading.
A cell parted into groups yields one row per marked group.
A book without the part patterns fills the line and the initial, and leaves the other parts empty.
A book that answers with its busy text is skipped for now, and the other book is still read.
A start stores the rows and raises the fanqie bulletin.
A character every book was busy for still raises the bulletin and is asked again on the next start.
A character every book answered for yet none placed is asked once per session and stores nothing.
A language whose pack lists no book reads empty and asks nothing.
The line-book sources live in `TEngineFanqieSource` and the rank presses in `TEngineFanqieRank`.
Both build through the helpers kept here.

## `TEngineFanqieRank`

Covers the rank press that marks a fanqie row as a representative, over the wiki pack with stubbed pages.
A rank press stores the rank the clerk resolves from the held rank and the raise flag.
A press on an unmarked row appends it, and a raising press moves a marked row up.
A plain press on a marked row unmarks it.
Rows ranked against their read order lead the clerk's read, in rank order.
The unranked rows follow in the order they read before.

## `TEngineFanqieSource`

Covers the fanqie sources that read plain text by line pattern, over a fixture pack with stubbed pages.
A book with a line pattern is fetched by GET and read as plain text, one row per match.
Its rows carry its own source label.
Two sources of one book both store, since each counts its positions from zero under its own source.
A not-found answer from such a book counts as reached with nothing, so the character is not asked again.
A pack pattern that times out answers no rows and not reached, so the miss is not remembered.
The wiki pack and its page are kept here for the rank facts in `TEngineFanqieRank`.

## `TFanqieRow`

Covers the formatted columns of a fanqie row and the blocks the engine groups rows and images into.
The headword reading the blocks give is checked here too, since it reads the ranks off them.
Rows handed in out of order list ranked rows first by rank.
The rest follow by pack book, then reading, then id.
A book the pack does not declare follows the declared ones.
A book holding a ranked row leads its character's blocks, and each block keeps the declared row order.

## `THypothesis`

Covers the hypothesis over small fixture tables.
A known initial and rime join into onset and final.
The rounded key wins when the row is rounded, and the 重紐 letter falls away.
A rounded row whose rime has no rounded key falls back to the plain final.
Each tone picks the first class row whose onset pattern matches, and the row's rewrites run over the whole syllable.
The level tone unfolds a voiced obstruent, the rising tone appends a glottal stop, the entering tone rewrites the coda.
A tone outside the table keeps the joined syllable with a blank class.
An initial or rime outside the tables, or an empty one, reads `null`.
A place ranks its initials in pack order across all places, and an unplaced initial ranks -1.

## Stamp

Commit: `c37ca1837ded`
- `TDiwei.cs` `a4edda17d7808789`
- `TEngineFanqie.cs` `e8b027c5e77dc27d`
- `TEngineFanqieFault.cs` `7b5037970ff82b1a`
- `TEngineFanqieRank.cs` `2640d00b351d0c49`
- `TEngineFanqieSource.cs` `34822f7f149db3d9`
- `TFanqieRow.cs` `848333a15eb2c3e4`
- `THypothesis.cs` `b8f7e0bac583dab3`
