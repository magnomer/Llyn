# Sound

This folder holds the slices that read an entry's sound facts.
They cover language facts, pronunciations, glyph, reflex, rime-book, script, paradigm, diwei and phonetic-series rows.

## `LDiweiPort`

The slice a deportment sees when it browses the rime cells of a rime book.
`LFanqieFacade` implements it, since the diwei clerk sits beside the fanqie clerk.
`LEngineDiweiFind` by key names the rime cell a pressed key names.
It names nothing when no cell of that kind carries the key.
The vista form lists the onset or rime cells a column shows, narrowed by the column's own query.
The page resolve localizes the labels through the given lookup.
The character resolve makes the entry of a character on a cell first when none exists, in the cell's language.
`LEngineXiaoyunFind` lists the entries under the chosen onset and rime, none when neither is chosen.
They are read in the language of the diwei the panel reads.
`LEngineDiweiRead` names the kind of the cell a pressed fanqie key opens, or none for a blank key.
Both reading views' gates ask it before the navigation opens.

## `LFanqiePort`

The slice a deportment sees when it shows an entry's rime-book rows.
The rows are fetched in the background, so the port has a check beside each read.
`LFanqieFacade` implements it.
`LEngineFanqieDivide` groups the stored rows by book and starts no fetch.
`LEngineFanqieRead` groups them after starting the fetch of every character still missing.
A character that already has stored rows, or a fetch under way, is never fetched again.
`LEngineReadingRead` forms the headword's representative reading from the grouped rows.
It starts the fetch of missing characters first, as `LEngineFanqieRead` does.
`LEngineFanqieRebuild` fetches every character of the entry again, its phonetic series along with its rows.
`LEngineFanqieSet` moves one row up or down among the entry's representatives.
The fanqie clerk resolves the new rank from the held rank and the raise flag.
`LEngineBookCheck` says whether a language's pack declares a rime book.
Without a language it says whether any loaded pack does, the verdict the yunjing tab shows by.

## `LGlyphPort`

The slice a deportment sees when it shows an entry's glyph row and transcriptions.
`LLanguageFacade` implements it, since the glyph section belongs to the language.
The glyph read answers none and the cell divide answers empty when the language declares no glyph section.
The transcription read lists the filled rows without the glyph row.

## `LLanguagePort`

The slice a deportment sees when it asks the language facts of a pronunciation.
It starts the sound fetches and reads the tone contour, the silent flag and the accent block.
`LLanguageFacade` implements it.
The contour scale is passed up from the rule that parses the levels, highest first.
The contour read is empty when nothing is drawn.
The silent check lets the input panel hide its pronunciation rows, and a blank language answers false.
The accent load fetches the variety flags the block draws, then answers the block.

## `LParadigmPort`

The slice a deportment sees when it shows an entry's inflection paradigm.
The inflections are fetched in the background, so the port has a check beside the scan.
`LVocabularyFacade` implements it.
`LEngineParadigmScan` joins each slot with its stored inflection.
Slots on the part a layout draws are left to `LEngineInflectionRead`.
`LEngineParadigmCheck` gives the text and tip key that stand in a paradigm row for its form.
The wording is `LParadigmShown`'s in Core, so the list and the inflection view never disagree.
The held flag is true for the editor, whose draft holds the missing forms.
Conduct receives the ready answer and only lays it into its own record.
`LEngineInflectionRead` gives the inflection view in both shapes, or null without a layout or a slot on its part.
The pending, enabled and held flags feed each cell's text and tip, as they do for the check.
Each cell arrives ready, so a reader maps it without a rule.

## `LPronunciationPort`

The slice a deportment sees when it lists pronunciations or shows how often an entry is heard.
It also reads the IPA charts the input aid lays out.
`LPronunciationFacade` implements it.
The find applies twinned names and the chosen mark.
`LEngineFrequencyResolve` gathers the entry's frequency into one answer, or null when it has none.
The `once` text words a word interval.
The consonant and vowel charts need no workspace, so the input aid reads them at any time.

## `LReflexPort`

The slice a deportment sees when it shows an entry's reflex readings.
The readings are fetched in the background, so the port has a check beside the rebuild.
It also reads and writes whether an entry's "More readings" rows are opened.
It reads and writes the Fanqie and Script box state of an entry too.
That state is fold state of one entry, which this port already serves.
`LReflexFacade` implements it.
`LEngineGuiseRead` answers one guise per reflex row, in order.
Each answer carries the row's ready shown text, so no caller applies the respelling rule.
The fold set is the one the pack of the entry's language declares.
`LEngineReflexSpread` and `LEngineBoxSpread` open or close a fold and keep that state for the entry.
Each write raises the fold bulletin on the entry, so other open views re-read it.

## `LScriptPort`

The slice a deportment sees when it shows an entry's script images.
The images are fetched in the background, so the port has a check beside each read.
`LScriptFacade` implements it.
The divide groups stored images by style and starts no fetch.
The read groups them after starting the fetch of every character still missing.
The rebuild fetches the images of every character again.

## `LStemPort`

The slice a deportment sees when it browses phonetic series.
`LStemFacade` implements it.
`LEngineStemFind` by key gives the series a pressed key names, or none for a blank or unknown key.
The vista form lists the series a column shows, narrowed by the column's own query.
`LEngineStemCheck` is true while a loaded pack declares a phonetic series source, the verdict the xiesheng tab shows by.
`LEngineStemResolve` gives the page of the chosen series, or the blank page when nothing was chosen.
The character form gives the entry of a character, in the language of the series, made first when none exists.
`LEngineStemSpread` opens or closes the "More readings" fold of one member, apart from its entry page.
A bare member or no series writes nothing.
`LEngineKindredFind` lists the entry rows of the series the column chose, in its language, narrowed by the list's own query.
Nothing is listed while no series is chosen.

## Stamp

Commit: `c2f335c331fe`
- `LDiweiPort.cs` `e787435741edfb65`
- `LFanqiePort.cs` `90813a8d5788c0cb`
- `LGlyphPort.cs` `eadbd2da3f745279`
- `LLanguagePort.cs` `a38e3623c2731155`
- `LParadigmPort.cs` `69a99b994b18c278`
- `LPronunciationPort.cs` `56a634253dc58350`
- `LReflexPort.cs` `6ad575ab8075b1ee`
- `LScriptPort.cs` `cf36885a53e2af51`
- `LStemPort.cs` `cc8daf850adaf2c5`
