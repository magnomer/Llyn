# Tool

These are the facades the engine offers its shells, one per tool.
The tools are Joplin push, fanqie, livery, markup, portrait, pronunciation, reflex and stem.
Each takes the hearth and the sibling facades it calls, all built by `LEngine` before it.
It takes siblings rather than the engine, so it names only the facades it uses.
The gate, the staff and the shared state are read through the hearth.
The staff is read afresh at each call, so a call after a workspace switch reaches the new workspace.
Facades that implement a port hand themselves to Conduct from Host, with no outlet between.

## `LCourierFacade`

The facade for the Joplin push and its access grant.

`LEngineCourierSend` forwards to the courier clerk, which guards its own gate.
It hands over `LLiveryFacade.LEngineLiveryRead` with `pending` false as the page reader.
A note is not rewritten when a fill ends, so it never shows a fill as pending.
The language reader is the other `LEngineLiveryRead` overload over `LSettingsFacade.LEngineTextFind`.
That source answers null for a missing key, as the rime-table page builder expects.
The Rime table panel passes the same source, so a category reads the same in Llyn and Joplin.
A push already running finishes against the workspace it started in.
A refused token, as `LCourierWarrantCheck` judges it, is cleared before the refusal goes on.
So the shell offers Connect again instead of a connection that no longer works.

`LEngineCourierAttach` awaits the grant, then stores the hidden token in the settings.
`LEngineWarrantClear` is its reverse.
A settings bulletin follows either only for a real change, so an unchanged token repaints nothing.

## `LFanqieFacade`

The facade for fanqie, wrapping the fanqie, diwei and tally reads of the fanqie and diwei clerks.
It implements the fanqie and diwei ports itself.
The vista-shaped finds stay here, since a vista is an engine handle.

`LEngineBookFind` reads the languages from the language clerk under the gate, so no language facade is needed.
`LEngineBookCheck()` is the verdict the yunjing tab is shown by.

`LEngineFanqieRead` and `LEngineReadingRead` start the fetch of every character still missing first.
The start skips a character whose rows are stored, so a character fetched long ago is never fetched twice.
`LEngineFanqieStart` starts the phonetic-series fetch too, since one box prints both.
`LEngineFanqieRebuild` forgets the misses and fetches them again, series along with rows.
`LEngineFanqieDivide` groups the rows by book without starting any fetch.

`LEngineDiweiRead(bool, string)` leaves the rule to the diwei clerk.
The facade only flips the initial flag into the clerk's final one.
`LEngineDiweiResolve(long?, Func)` shows respellings under the respelling setting for the diwei's language.
The tally shows under its own setting.

`LEngineDiweiResolve(long?, string)` finds or makes the entry of a character in the page's language.
The entry resolve is the entry facade's own, so a glyph chip anywhere lands on the same entry.
The diwei is read under this facade's gate, and the resolve takes the entry facade's gate afterwards.
With no diwei the page's language is blank, which the resolve refuses.

`LEngineDiweiFind(string, string, string)` answers null for a diwei never stored.

`LEngineDiweiFind(LVista, long?, bool)` lists the initials or the rimes as one column.
The column lists the language of the chosen diwei, the one the panel reads.
A chosen id no longer listed clears the choice.
Nothing is listed while no language declares a book.

`LEngineXiaoyunFind` finds the entries filed under the matching diwei, built as vista rows.
They follow the vista's order as the catalog entry list does, or headword order without a vista.
The chosen-pair overload reads in the language of the diwei the panel reads, and lists none when neither is chosen.

`LEngineLanguageRead` answers the chosen diwei's language, else the first language declaring a book.
It answers empty while neither exists.

## `LLiveryFacade`

`LEngine` builds it once and holds it as `LEngineLivery`.
`LCourierFacade.LEngineCourierSend` hands both its reads to the Joplin push.

`LEngineLiveryRead(long, bool)` holds `LEngineGate` for the whole read, so every part comes from one moment.
It answers null when no entry is stored.
It hands every answer to `LLiveryClerk.LLiveryClerkBuild` and returns the page that builder makes.
It calls no read that starts a fetch.
The frequency rows come from `LFrequencyClerkRead` with fetch off.
The reading line comes from `LFanqieClerkFormat`, the rule `LEngineReadingRead` applies, minus that read's fanqie start.
The folded card ids come from `LFoldClerkRead`, inside the same lock.
The etymology tree comes from `LLineageClerkRead` over the stored draft's boxes, with the relation list beside it.
The flags come from `LLanguageFlagFind` and `LVarietyFlagScan`, which read stored files only.
The accent sheet is read once and passed to both the page and `LVarietyFlagScan`.
The builder is handed `LLanguageFlagFind` itself and asks it once per named language.
The inflection table comes from `LVocabularyFacade.LEngineInflectionRead` with `held` off, since a note never holds a fill.
Its `enabled` is `LEngineMorphologyCheck`, the setting the display reads.
The same pending state and `enabled` pass to the builder, which checks each paradigm row with them.

The caller's `pending` decides whether the page reads the in-memory lacuna mark `LEngineInflectionCheck`.
The export passes true, so the page shows each row and table as the view shows them at that moment.
The Joplin note passes false, since the lacuna mark lives only in memory and a note keeps stored data.
An in-flight fill would freeze as pending in Joplin, since nothing rewrites the note when the fill ends.

`LEngineLiveryRead(string, Func)` holds `LEngineGate` for the whole read, so every list comes from one moment.
It hands the stem clerk only while `LShengfuRuleRead` answers a rule for the language.
It hands the diwei clerk only while `LEngineBookCheck` finds a fanqie book for the language.
It hands `LSettingsRespelled` too, so the series members get the guises the entry note gets.
It never fetches and never starts background work.

## `LMarkupFacade`

The facade for markup cargo reads, imports and entry matching.
It calls no sibling facade, so `LEngine` builds it among the first.

`LEngineMarkupRead` parses the file into a cargo, blanks and notes an unknown language, and applies twin names.

`LEngineMarkupStart` runs a whole import in its order.
It reads the file, finds the targets, asks for the intakes, then stores the cargo.
The question gets each parsed entry beside its ready targets, so no caller finds them again.
The read, the find and the store run on a worker thread, so the window stays live while parsing.
The hops live here because the shell starts no task of its own.
The question runs back on the caller's thread, since it may put up a window.
A declaration answered null stores nothing and answers null.
The cargo never leaves the engine, so no caller holds it between the two steps.

`LEngineMarkupImport` runs the intake clerk under the gate and announces nothing, so it moves the engine revision itself.
A replaced entry clears senses that other entries' sentences cite, and a held draft may compare against those.

## `LPortraitFacade`

The facade for portrait, covering the entry page and the kind pages, their print and the entry's export.
The entry page is composed by the portrait clerk, the kind pages by the example, reference and situation clerks.
`LEngine` builds it after `LEngineLivery`, since the entry export reads the Livery page.
The settings facade words the entry sheet's labels and formats the file name a print session offers for saving.

`LEnginePortraitRead` composes under the gate, each realm under the engine's lock, so screen and page show one thing.
A missing kind row is an error, because a caller asked to portray one that no longer stands.

`LEnginePortraitExport` writes markup through the markup clerk under the gate.
Every other format composes the portrait page and hands it to the press clerk.
HTML and PDF also hand the Livery page, read with `pending` true.
So the file shows a fill in flight as view mode does.
A missing Livery page throws, as a missing kind page does.
Markdown and Word keep the portrait page alone.
The press clerk prints a PDF through the press and saves each other format itself.
The vista overload does nothing when the vista holds no entry.

`LEnginePressOpen` opens a print session on the same sheet its print and HTML export write.
It reads the Livery page with `pending` true, and a missing one throws.
The PDF name is formatted before the board opens, so a failure there leaves no board behind.
No `ConfigureAwait(false)` is used, because the board's browser is bound to the opening thread.
The press clerk answers a session clerk around the board, so the facade names no Core contract.
The relay owns that session clerk, and the galley it answers reads it.
The galley keeps the press clerk to list printers when its window shows.
It also keeps the print setting clerk, which runs every setting rule and the store.
The kind-page overload is offered for saving under the page title and keeps the same clerks.
The vista overloads answer null when the vista holds no choice.
The legend overload throws for any subject but example, situation or reference, as a caller mistake.

## `LPronunciationFacade`

The facade for pronunciation rows, recordings, lookups, transcriptions and frequencies.
Most calls take the gate and call the pronunciation, recording, transcription or frequency clerk.
The session trove that remembers a lookup or harvest stays here, since a session is an engine fact.
It implements the pronunciation port itself.

`LEnginePronunciationFind` with a filter lets the pronunciation clerk apply it, so the shell holds no filter rule.
The filter needs no clerk state, so only the unfiltered read takes the gate.
The respelling setting and the language pack decide per entry language whether a row shows its respelling.
The vista overload applies twinned names and the chosen mark through the vista build.

The lookup find replays a session's earlier answer from the trove instead of searching again.
The caller builds the relay, so after a failure it can read whether the lookup ended it.
The lookup is started under the gate and awaited outside it.
`LEngineRecordingFind` does the same for a harvest, filtered to the variety of the draft row `target` names.
A fresh harvest is saved to the trove once it finishes.
`LEngineTroveSave` remembers the candidates or the recordings a finished search found under the session.
`LEngineTranscriptionFind` throws for a blank scheme and replays from the trove when the session already asked.
`LEngineVarietyResolve` answers the variety of the row `target` names, the first row's for zero, empty outside a session.

`LEngineRecordingSave` stores a harvested recording under the workspace and answers its path.
`LEngineRecordingPrepare` fetches one to a playable local file without storing it.
A failed fetch or file write reaches the caller as the clerk's vault fault, still unrecorded.
`LEngineRecordingSweep` deletes every stored recording no row and no draft names.

`LEngineRecordingPlay` plays under the gate and plays nothing for a file that does not exist.
It answers the play's ticket, or zero when nothing played.
`LEngineRecordingStop` and `LEngineVolumeSet` skip the gate, since they touch only the phonograph.
They never wait on a long engine call.

`LEnginePlaybackRead` answers whether the draft's own recording exists and whether the view has anything to play.
The second holds for the own recording or any accent row that is notated and carries audio.
`LEngineAudioRead` answers the own recording while its file exists, and whether the editor has anything to play.
The editor shows every accent row, so unlike the reading view it counts rows without a notation too.
`LEngineAudioResolve` answers null when the file does not exist.

`LEngineConsonantRead` and `LEngineVowelRead` build the IPA charts with no gate.
They read no workspace state, so the input aid can lay them out at any time.

`LEngineFrequencyResolve` gathers the entry's frequency into one answer, or null when it has none.
The `once` text words a word interval.

## `LReflexFacade`

The facade for reflex, wrapping the clerk's anchors and rules, the fetch the clerk holds, row guises and tones.
It implements the reflex port itself.
It also keeps each entry's "More readings" state and editor box state, through the fold clerk.

The anchor reads work on a stored entry's own rime-book rows, or on rows handed in.
`LEngineAnchorScan` marks the rows held and estimated for the anchor dropdown.
The tone rules of the entry `language` resolve the classes of the `reflex` language and `tone`.
`LEngineAnchorFormat` answers empty when the rows cannot be anchored.
`LEngineAnchorRead` starts the fetch of missing characters, then returns the clerk's flat list.
The flat list runs character by character, each character's rows in the declared book order.
Flattening the book groups would put book before character instead.

`LEngineGuiseRead` answers one guise per row in `reflexes`, in order.
Each carries the row's ready shown text, so no caller applies the respelling rule.
The fold set is the one the pack of the entry's `language` declares.
The guises come from `LLanguageGuiseScan`, their one owner, under the held respelling setting.

`LEngineReflexStart` fetches only for an entry that has none.
`LEngineReflexRebuild` clears and fetches again.

`LEngineReflexSpread` and `LEngineBoxSpread` write the opened state through the fold clerk under the gate.
The fold bulletin is raised after the gate is released, as the card fold writes do.
It is raised even for a refused id, since the clerk alone owns that rule.
`LEngineSpreadCheck` and `LEngineBoxCheck` read the state the same way.

`LEngineDescentRead` loads the pack through the language clerk under the gate and calls it outside.
It answers none for a blank language.

## `LStemFacade`

The facade for stem, the phonetic series the xiesheng panel browses by.
It implements the stem port itself.
It reads the series and the entries they reach, and composes one series page.
The links themselves are written where a series is stored, so nothing here writes them.
Its own write is a member's fold, through the fold clerk.
The character resolve may make an entry, but through `LEntryFacade`.

`LEngineStemFind()` answers the language of the first loaded pack that declares a series source, or null.
The panel is shown only while one answers, as the yunjing panel waits for rime books.
`LEngineStemCheck()` is the verdict the xiesheng tab is shown by.
`LEngineStemFind(string, string?)` answers null for a blank key or a series never stored.

`LEngineStemFind(LVista)` narrows the column's series by the query, sorts by the order and marks the chosen.
Nothing is listed while no pack declares a series source.
A chosen series that the narrowing dropped is unchosen, so the column never points at a missing row.

`LEngineStemResolve(long?)` answers the page of the chosen series, or the blank page when nothing was chosen.
Its members come from `LStemClerk` with their own folds and the guises of their rows.
The clerk reads the guises under the held respelling setting, as the entry page's reflex rows do.
So the reader maps the whole page from this one call and asks the engine nothing more.

`LEngineStemResolve(long?, string)` finds or makes the entry of a character in the page's language.
The entry resolve is the entry facade's own, so a glyph chip anywhere lands on the same entry.
The series is read under this facade's gate, and the resolve takes the entry facade's gate afterwards.
With no series the page's language is blank, which the resolve refuses.

`LEngineStemSpread` reads the series, finds the member's entry, then writes the fold.
`LStemClerk.LStemEntryFind` finds the entry, and `LFoldClerk` writes it under the series key.
A StemFold notice is raised on the entry afterwards, outside the gate, as `LReflexFacade` raises its own.
Its own subject keeps entry page and card folds from redrawing the series page.
No series or a bare member writes nothing and raises nothing.

`LEngineKindredFind` lists the entry rows of the series the column chose, narrowed by the list's own query.
Nothing is listed while no series is chosen.
The internal overload builds catalog rows with the chosen one marked.
They follow the vista's order as the catalog entry list does, or headword order without a vista.

`LEngineLanguageRead` answers the chosen series' language, else the first pack declaring a series source.
It answers empty while neither exists.

## Stamp

Commit: `c6e60141a172`
- `LCourierFacade.cs` `316cb119faa9202c`
- `LFanqieFacade.cs` `19ca044c7f98d17f`
- `LLiveryFacade.cs` `930588ef8cfd2c99`
- `LMarkupFacade.cs` `68847305501ca292`
- `LPortraitFacade.cs` `77018d523cca1d13`
- `LPronunciationFacade.cs` `665456dd10052ea4`
- `LReflexFacade.cs` `95d3d3fbd6345d56`
- `LStemFacade.cs` `8ea42b65c4f6631e`
