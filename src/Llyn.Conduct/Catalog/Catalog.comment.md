# Catalog

Catalog holds the reference reads every driver shares, and the Conduct records they answer.
Each engine record is mapped into its Conduct record, so a driver never names a Core type.
The maps are by name, never by cast, so a reordered engine enum cannot shift a meaning.
It stands apart from the panels, which list and edit one kind of record each.

## `CCatalog`

Reads sentence orders, glyphs, languages, flags, meanings and articulation charts.
It stands on the atelier's ports and holds no state beyond the atelier.
`CAtelier` hands one out on each read, and only the atelier builds it.

`LCatalogEnsignLoad` is the one ordering rule for an area's rows that show flags.
The flag fill runs first into the driver's store, then the area's read.
So each row painted afterwards finds its flag drawn.
The settings port already answers the languages without flags when a flag file cannot be written or read.
Any other fill failure shows the caller's `key` through the envoy, and the fill answers no languages.
The rows are still read, so the area paints its list without flags.
Drivers await this from event handlers, so a faulted fill would end the app.
Each panel hands its own load key, so the notice names the list that failed.
A failure inside `read` is not caught here and still reaches the caller.
`CCatalogEnsignLoad` does the same alone and shows `Language.LoadFailed`.
`CCatalogEnsignRead` is the one map of flag rows, shared by the errand, display and timbre loads.

`CCatalogOrderRead` maps the engine's sentence order to the Conduct shape.
The two slots are always 0 and 1, one each, so no two fields share a column.
Any other pair falls back to `CSentenceOrder.CSentenceOrderPlain`, as the engine's default order does.

`LCatalogGlyphOpen` is the one failure owner of every glyph chip that opens a character's entry.
A refused resolve shows `Glyph.OpenFailed` through the panel's envoy and answers null.

`CCatalogAidRead` builds both IPA charts of the input aid in one read.
They come through the atelier's pronunciation port, so each chart keeps its one owner in the engine.
`LCatalogArticulationRead` takes the chart as a parameter, so a test can feed a hostile chart.
It pads a missing cell row with an empty one and drops any row past the last side.
It cuts each row to the header count, so no cell falls outside the grid.
It does not pad a row shorter than the headers.
Drivers place each cell by its own column and walk only the row they get, so padding would add nothing.

`LCatalogOrderRead` maps the ordering both ways, by name.
An unknown member is a caller's error, and a driver that chose none maps to null.
`LCatalogSubjectRead` maps the subject member by member.
The series fold subject maps to its own engine subject, never to the plain fold.
`LCatalogRowRead` is the one home of the vista row copy, for entry lists and the translation offer.
It copies the engine's empty epithet as it stands, so the map holds no fallback.
`LCatalogMeaningRead` maps meaning rows with no rule, for the shared sense read in `CMention`.

## Result rows

Each `CCatalog*` record is one row a search found, in the shape one panel lists.
Pronunciation rows carry selection in their nested vista row, while the other result records expose their own flag.
Counts and credits arrive worded by Core, blank or an unknown wording when nothing applies.
- `CCatalogAuthor` is a roll and union row.
  Id zero is the row of uncredited sources.
  It wears the unlink mark where stored authors wear the guild mark.
- `CCatalogExample` carries a display text, worded when unknown or unwritten, and a separate name the search matches.
- `CCatalogPronunciation` pairs a vista row with the primary IPA and the sound ready to show in brackets.
- `CCatalogReference` carries credit and year as wordings, or the unknown or unset key in their place.
- `CCatalogRegister` carries an icon key chosen by Core from the register's name.
- `CCatalogSituation` and `CCatalogTag` are plain rows for their lists.

## Stamp

Commit: `bd299c638b2c`
- `CCatalog.cs` `9bd012c60f9a52bc`
- `CCatalogAuthor.cs` `b46f14e4512ea3a0`
- `CCatalogExample.cs` `a3c4c5ca7f06fa47`
- `CCatalogPronunciation.cs` `e74014415ee6721f`
- `CCatalogReference.cs` `6655818475029685`
- `CCatalogRegister.cs` `970abba4732aab26`
- `CCatalogSituation.cs` `2be1f43f5ab654e1`
- `CCatalogTag.cs` `0e9bba49508a93e1`
