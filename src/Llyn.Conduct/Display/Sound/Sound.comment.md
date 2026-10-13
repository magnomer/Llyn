# Sound

This folder holds the reading view's sound area and its rules.
It carries the ready shape of each glyph, reflex, anchor, rime-book, script and paradigm block.

## `CDisplaySound`

The sound area gives gates and reads for the glyph, reflex, rime-book, script and paradigm blocks.
It is split from the header by role, and holds no state of its own.
The shown draft stays in `LDisplaySound`.
The engine stores each entry's reflex opening.
Each block reads through its own narrow port, so no block reaches a slice it never shows.
Block reads answer empty while nothing is shown, so late notices cannot restore closed sections.
Repaint reads share the display's failure memory, while user acts report failures separately.
The header supplies the displayed language and headword for anchors, navigation and fonts.

`CDisplayGlyphRead` chooses the heading key through `CScheme.CSchemeKeyRead` and maps the engine's cells by name.
A language without a glyph section, or a refused read, answers the hidden row.
A refused read also shows `Sound.LoadFailed` once.
`CDisplayTranscriptionRead` answers the rows without the glyph row, which the engine filters.
`CDisplayReflexRead` takes its rows from the one reflex scan the editor shares, `CRespelling.LRespellingReflexScan`.
The fold verdict is read from those rows by `CReflex.LReflexFoldCheck`.
`CDisplayReflexResonate` reloads the stored rows, then reads the block again.
The reload only reaches the reflexes, so the header and the other blocks keep the draft on screen.
`CDisplayFanqieRead` and `CDisplayScriptRead` show their list and pending refusals through the voice's failure event.
`CDisplayParadigmRead` takes its font from the paradigm's own language, which the engine resolves.
The slots carry their ready text and tip key, so the driver reads neither verdict.
The inflection fetch starts when the entry opens, never by this read.
The inflection box has a separate ledger-backed read using the same failure key.
A refused box read shows that notice and answers a null view.
The box is asked with `held` off, so a lost cell reads as lost.
`CDisplayFoldOpened` is whether the folded reflexes are shown, as the engine stores it for that entry.
Nothing shown answers false.
`CDisplayFontRead` uses the one font rule `CFont.CFontRead` holds, so no driver hands a language back.

`CDisplayGlyphOpen` opens the entry the character stands for in its language.
The engine finds or makes that entry, and it is raised for the navigation to open in the library tab.
A refused resolve shows `Glyph.OpenFailed` through the catalog's one failure owner and opens nothing.
`CDisplayReflexToggle` stores `opened` for the shown entry with one engine call.
The engine then raises the fold bulletin, so the editor and this view repaint from the store.
True means the port returned without throwing, not that a stored row changed.
Nothing shown answers false and writes nothing.
A refusal shows `Reflex.SpreadFailed` every time and answers false, so the driver puts the hinge back.
`CDisplayFanqieSet` hands the held rank and the raise flag for the shown entry.
The fanqie clerk resolves the new rank, as behind the editor's gate.
A refusal shows `Display.FanqieRepresentativeFailed` every time.
`CDisplayDiweiOpen` raises the cell in the shown entry's language, and the engine names the cell kind from the initial flag.
Nothing shown, or a key the engine names no kind for, opens nothing.
`CDisplayStemOpen` raises the series in the shown entry's language, and nothing shown opens nothing.
`CDisplayStemChosen` carries two values, with a null key allowed for navigation without a selected series.

`LDisplayReflexScan` hands the written reflex rows as stored to the shared scan.
A refused scan shows `Display.ReflexFailed` once and answers no rows.
`LDisplayAnchorRead` uses the shared anchor map `CReflex.LReflexAnchorRead`, the one the editor's block uses.
It hands the map this view's envoy, settings and repaint memory, so a refusal shows `Display.AnchorFailed` once.
`_cDisplayBare` is the font with nothing set, so a block with nothing shown keeps its theme.
`_cDisplayUnanchored` is the anchors while nothing is shown or the engine refused.

## `CLecternAnchor`

How reflex rows anchor to the fanqie rows, ready to show.
The reading view and the editor's reflex block both carry it.
`CLecternAnchorOffered` is whether the headword's fanqie rows let a reflex row anchor at all.
The texts are keyed by the reflex row id.

## `CLecternFanqie`

The rime-book block for the shown entry, ready to show.
The groups are the fanqie blocks as the engine divides them.
`CLecternFanqiePending` is whether the rows are still being fetched.
The reading is the headword's representative reading, empty when refused.
The font is the pack's glyph typography for the shown language.

## `CLecternGlyph`

The glyph row for the shown entry, ready to show.
`CLecternGlyphShown` is whether the row has any cell, which shows its section and its shared label column.
The key is chosen by `CScheme.CSchemeKeyRead`.
The name is the glyph scheme's own name, used as the heading when the key has no text.
The cells are one per character, in headword order.

## `CLecternParadigm`

The paradigm block for the shown entry, ready to show.
The slots are the first slot of every paradigm row, each with its ready text and tip key.
The font is the headword typography of the paradigm's own language.
The view is the inflection box, null when no layout or slot gives one or the read fails.

## `CLecternReflex`

The reflex block for the shown entry, ready to show.
The rows are the written reflex rows, each resolved by the shared reflex scan.
`CLecternReflexPending` is whether a reflex fill still runs, which shows the loading line.
`CLecternReflexFoldable` is whether any row folds, which shows the fold toggle.

## `CLecternScript`

The script block for the shown entry, ready to show.
The groups are the script images grouped by style.
`CLecternScriptPending` is whether the images are still being fetched.

## `CGlyphCell`

One character cell of the glyph row, mapped from the engine cell.
The language is blank for a cell that opens no entry.
`CGlyphCellLinked` is Core's verdict that the cell opens an entry.

## `LDisplaySound`

The reading view's sound state holds the shown draft, its entry and the latest play ticket.
The gates and reads over it stand on `CDisplaySound`, which holds no state of its own.
Caught list-read and check failures answer empty or false, so their sections can still draw.
`LDisplayReflexRead` does not catch failures itself.
Every refused read it catches raises `LDisplaySoundFailed` with its text key, so none passes unseen.
`LDisplay` passes it on through the repaint memory, since a lasting refusal shows once until the user acts.
Every refused send raises `LDisplayMarkFailed` instead, since a send follows a show the user caused.
`LDisplay` shows it through the panel's envoy every time.
The engine owns the player, so the view holds no player of its own.
`LDisplayTicket` is the ticket of this view's latest play, and a stop sends it.
So the engine stops this play alone and never another view's sound.
The editor's `CKindred` reads the opened state and the reflex check here, and asks for a rebuild.
It holds no opened state, since the engine stores it per entry.
The editor's `CSounding` reads the morphology verdict here too.
Only Conduct reads it, so the drivers are offered no Core draft.
`LDisplaySoundShow` holds the draft and its entry id as the entry the verdicts and reads work on.
Reflexes loaded for an earlier entry are dropped, so the rows follow the new draft.
A present entry id starts background sound fetches through one engine call, leaving their order to the engine.
The start runs on every show of a draft.
A refused start raises `LDisplayMarkFailed` with `Sound.StartFailed`.
`LDisplaySoundClear` drops the draft, its id and the loaded reflexes, and stops its own playback.
A sound another view started plays on, and clearing a display that shows nothing stops nothing.
`LDisplaySpreadCheck` answers whether "More readings" is opened, as the engine stores it for that entry.
No entry answers false, and so does a refusal, which raises `LDisplaySoundFailed` with `Reflex.SpreadReadFailed`.
The reading view and the editor both read here, each for its own entry.
`LDisplayReflexLoad` reloads the shown entry after a reflex fill, and keeps the stored draft for the reflex rows alone.
Every other read still works on the draft on screen.
A refused or empty load keeps the rows as they were, and a refusal raises `LDisplaySoundFailed`.
`LDisplayReflexRead` gives the written reflexes as the engine filters them, and a draft reloaded after a fill wins.
`LDisplayReflexCheck` answers false for no entry or when the engine refuses to say.
A refusal raises `LDisplaySoundFailed` with `Sound.PendingFailed`, as for the fanqie, script and paradigm checks.
`LDisplayReflexRebuild` asks the engine to drop the reflex rows and fetch them again.
A refusal raises `LDisplayMarkFailed` with `Display.ReflexRebuildFailed`.
The fanqie, script and paradigm checks tell whether rows are still being fetched, false for no entry or a refusal.
`LDisplayMorphologyRead` answers false when the settings cannot be read.
That refusal raises `LDisplaySoundFailed` with `Sound.MorphologyFailed`.
`LDisplayPlaybackStop` stops this view's play, and leaves a later play from another view running.
`LDisplayListRead` runs a read for the shown entry, answering no rows for no entry or a refusal.
A refusal raises `LDisplaySoundFailed` with the caller's key, so each section names its own failure.
`LDisplayMarkSend` sends a fetch request that answers nothing, so a refusal only raises `LDisplayMarkFailed`.

## Stamp

Commit: `c2f335c331fe`
- `CDisplaySound.cs` `48f2ca458bdac787`
- `CGlyphCell.cs` `2f4dded0bc252b1c`
- `CLecternAnchor.cs` `654f4dc6482c4df1`
- `CLecternFanqie.cs` `d5583ba94123fa95`
- `CLecternGlyph.cs` `00744b0b4ab090a8`
- `CLecternParadigm.cs` `3df03bf13a16f5b7`
- `CLecternReflex.cs` `57f11b902d913ebb`
- `CLecternScript.cs` `b793b552979ff773`
- `LDisplaySound.cs` `3b183f53f5877bd0`
