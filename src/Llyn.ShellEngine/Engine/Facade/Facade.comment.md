# Facade

This folder holds the engine's faces toward the shell, one per concern.
Each reads its clerks from the current staff and serves that concern's ports.

## `LDraftFacade`

The engine facade for drafts, so the shell never opens the drafts folder itself.
The files, the claims and the held set are `LClaimClerk`'s, and every call here reaches them through it.
An id raised here belongs to the workspace that raised it, so a workspace change marks every held id stale.
Every call naming an id is checked against that mark, which is the one fact the clerk cannot know.
The commit round and the dirty check belong to the outcome and citation clerks.
This facade only takes the gate around them.
These are primitives, and `LTenure` in `LTenure.cs` is the session object that sequences them for a panel.
The example, situation, reference and author starts and commits live in those kinds' facades.
Every edit in between is a request, applied in `LRequestFacade.cs` for all kinds alike.
It implements the markdown port itself, so Host hands it to Conduct with no outlet between.
It takes its siblings rather than the engine, so it names only the facades it uses.

## `LEngineDraftStart`

It starts an entry draft through the citation clerk.
A new draft takes the first listed language.
An existing entry is loaded as it stands.
An entry that no longer stands is refused.
The languages are read from the language clerk directly, so the draft facade needs no language facade.

## `LEngineDraftPrepare`

The requests that give a held entry draft the rows the editor always shows.
They are one meaning card and one collocation card, one sentence under each card, and the first transcription row.
The glyph section's own row is added too when the language has one.
The glyph and scheme reads go to the language staff directly, so no sibling facade is needed.
The tenure applies them in its prepare turn and asks again, since a new card needs its sentence.

## `LEngineDraftRead`

It reads one held draft, or null when its file is gone or unknown.
An id from a closed workspace is not a missing file and is refused rather than answered null.
`LEnginePresentRead` answers null for such a stale id instead, and the tenure's present read uses it.

## `LEngineDraftDelete`

It removes one held draft with its claim, after the links that draft owns are settled.
A draft bulletin carrying id zero follows, because the held set shrank and the status bar counts that set.
Zero names no draft, so no editor mistakes it for its own.

## `LEngineDraftCheck`

It answers whether held work differs from the record it was started from.
The overload with `refusal` names the reason a commit would be refused right now.
It gives null when the commit would go through.
The form enables Save from that answer rather than judging the headword itself.
A missing draft file answers false, because there is nothing to lose.
The citation clerk answers for every kind of draft.

## `LEngineDraftSweep`

It drops every unreadable value the held draft carries to unspecified and stores the draft again.
A commit refuses a draft holding unreadable values.
The shell asks the user and then calls this before committing again.
A draft bulletin follows so every view re-reads.

## `LEngineDraftCommit`

The commit round of one entry draft runs in the outcome clerk under the gate.
The stale mark is checked first and the session trove of the draft cleared after.
Every entry the round wrote gets its bulletin and its inflection fetch outside the gate.

## `LEngineDraftCancel`

It discards held work, the court first and the file second.
It also drops what this draft's lookups and audio searches found, because closing is what frees a trove.
A draft bulletin carrying id zero follows, as after a delete, so the status bar recounts the held set.
A stale id is only released, because its file belongs to the workspace the engine has left.
Releasing it is how the stale set shrinks as the old tenures close.

## `LEngineLeftoverSweep`

It cancels every unheld draft that says nothing its origin does not.
A draft with no entry is cancelled when it is blank.
One with an entry is cancelled when the citation clerk finds it equal to the entry.
A draft this engine holds, or another running program claims, is never touched.
It announces nothing, so it moves the engine revision itself.
A swept draft can settle a court into a held draft, so no tenure may keep its state across it.

## `LEngineDraftValidate`

It refuses an id marked stale by a workspace change, through the claim clerk's static check.
It takes the gate itself, since a workspace change writes the stale set under it.
`LEngineDraftLoad` reads a held draft the caller is about to act on and refuses when it is gone.
It also takes the gate itself, so no caller reaches a clerk the rig is replacing.

## `LLanguageFacade`

The engine facade for language packs.
Everything the shell asks about a language as such is answered here.
That is the packs on disk, a pack's typography, its flag, and its regional varieties.
Each pack is read once through the language clerk's cache, so no lookup parses the file again.
The clerk is rebuilt with the workspace, since a pack's source lists belong to the folder it was read from.
The shell never reaches into the `languages/` folder itself.
The script images live in `LScriptFacade`, since only the script clerk serves them.
It implements the glyph and language ports itself, so Host hands it to Conduct with no outlet between.
The port reads the glyph section from a draft, so that member is implemented explicitly beside the language read.
Except for the language list, the gate is taken only to read the clerk field, which a rig apply replaces.

## `_lLanguageFacadeEnsign`

One flag fill runs at a time, around its missing read, its fetch and its record.
Several panels ask on the same announcement, and each fill is a set of downloads.
Without the gate each would read the same flags as missing and fetch them again.
Both fills share it, so a variety fill and a language fill never overlap either.
It is awaited, never held by a plain wait, so a fill awaiting the shell can always finish.

## `LEngineLanguageRead`

It returns the names of the languages that have a pack on disk, for the UI to offer as choices.
The clerk scans the packs once and keeps the list, so a repeated ask does not scan again.
Every panel asks for it on each entry switch, so a fresh scan each time stalled the UI thread.

## `LEngineGlossRead`

It picks the gloss language of the settings when a pack for it is loaded.
Otherwise it falls back to the first loaded language, and to empty when none is loaded.
A settings file names a language the workspace may lack, so the pick checks the list first.

## `LEngineFontRead`

It returns the typography the pack declares for one role.
The roles are the headword, an example line, a gloss, and a glyph chip.
The glyph role falls back to the example typography when the section names no font.
A blank language answers `LFont(null, 0)` without loading a pack, so the theme's font stands.
This is the one owner of that guard, shared by the view and the Livery page.

## `LEngineFlagRead`

It returns the local path to the given language's flag image.
It returns `null` when the pack declares no flag or the download fails.
The pack declares a country code or a path inside the workspace.
The engine uses a path as it stands and downloads a code from the flag-icons set into the workspace.
So the UI never reaches into the `languages/` folder itself.

## `LEngineEnsignLoad`

It fetches the flag of every language not yet asked of the cache, all at once.
It records what came back.
It hands the rows newly kept to `store`, so the shell draws only those and asks nothing about the rest.
A fill that a workspace change outran records nothing and calls nothing.
The cache is read once before the fetch and kept, since a workspace change builds a new one.
Asking the staff again after the fetch would meet the new cache at a matching age.
It answers the loaded languages it read, so a language menu fills from the same load.
The fetch resumes on the caller's context, so the shell's seam runs on the shell's thread.
It waits its turn at `_lLanguageFacadeEnsign`, so a second fill reads the cache after the first recorded.
A failed download or flag file write answers null inside the loader, so one missing flag never stops the fill.
Any other failure is not caught here and ends the fill.
The overload for one language fills its named varieties, keyed `language/variety`, at the same gate.
The overload for an accent sheet loads the flags the sheet draws.
A pack that labels varieties by name loads nothing.

## `LEngineVarietyRead`

It returns the regional varieties the language pack declares, in the pack's order.
It is empty when the pack declares none, so a language without varieties shows plain rows.
It reads the pack from the engine's cache, so a menu reopened does not reparse the file.
`LEngineFlaggedCheck` reports whether the pack asks the UI to label a reading's variety by flag rather than by name.
The draft overload treats an empty language as unflagged.

## `LEngineAccentRead`

It builds the pronunciation block of a draft as the reading view prints it.
The pack's flag and respelling verdicts and the primary's contour come in one answer with the rows.
The language clerk resolves each row, so the reading rule has its one owner below.
Only further pronunciations that carry a reading become rows.
The reading view hands the notated rows, and the editor hands every row.
`LEngineAccentLoad` loads the flags the block draws into `store`, then answers the block.

## `LEngineContourRead`

It reads the tone contour syllables of `ipa` in `language`, ready to draw under a reading.
The language clerk owns the rule, so the sheet and the editor share it.
`LEngineContourScale` is the pitch levels a tone contour draws, highest first, passed up from the rule that parses the levels.

## `LEngineSilentCheck`

It reports whether the pack declares the language silent, so the input panel knows to hide its pronunciation rows.
A blank language has no pack and answers false, so an empty desk keeps its pronunciation rows.

## `LEngineVarietyResolve`

It returns the local path to the flag image of one named variety of the language.
It returns `null` when the pack lacks that variety or its flag, or the download fails.
The name is matched exactly, because it is the tag the pack's own readings carry.

## `LEngineLanguageLoad`

It reads the pack named, through the language clerk.
Readers of a pack's declarations go through the language clerk, so its cache alone parses the file.

## `LEngineTranscriptionRead`

It lists the filled transcription rows a reading view shows, without the glyph row.
`LEngineGlyphRead` splits a draft's transcription rows by its glyph section, as an editor lists them.
`LEngineGlyphDivide` gives the cells of the draft's glyph row.
The by-language read gives the glyph section the pack declares, or null when the language shows no glyph row.
The glyph port's read is explicit because the public read of that name answers the split rows instead.
It forwards to the read by language, so both answer the same section.

## `LEngineSoundStart`

It starts every background fetch a reading view of the entry shows, in one fixed order.
The reflexes go first, then the inflections, the script images and the rime-book rows.
The script facade owns the image start, and this facade only calls it in that order.
Each start skips what is stored or pending, so opening an entry again starts nothing twice.
The inflection start itself holds back while the morphology setting is off.

## `LRequestFacade`

The engine's facade for requests against held drafts.
A form sends one edit at a time as an `LRequest`, and the engine applies it, saves, and announces.
The form never assembles an `LEntryDraft` from its controls, so the engine's file is the only truth.
The applying itself is `LDraftClerk`'s, over the ports of the rig.
The undo and redo calls and the court calls of a held draft live here too.
The facade shares the engine gate and uses the draft and entry facades and the hearth bulletin.

## `LEngineRequestApply`

It applies one request to the held draft it names and returns the draft as saved.
A draft the engine marked stale is refused.
The clerk applies the request, and a headword or language change then drops the recordings that no longer fit.
The content is normalized after the change, so anything the change left unnamed is named before the write.
The draft being replaced is recorded in the chronicle first, so the edit can be undone.
A request that leaves the draft equal to what was held writes nothing and raises nothing.
The bulletin is raised outside the gate, after the file is written.

## `LEngineAudioClear`

It drops the recordings a headword or language change made wrong, comparing the applied draft with the held one.
A new language makes every recording on the entry audio in the wrong language, so all of them go.
A new spelling drops only a recording fetched this draft, since a stored one is the entry's own.
It loads the stored entry through the entry facade, so the entry's own recordings survive.

## `LEngineChronicleRecord`

It keeps the draft an edit is about to replace, when the edit is worth a step.
Nothing is kept when the two drafts match by the measure the dirty check uses.
Nothing is kept either when neither draft differs from what it was started from.
An editor opening a draft adds blank cards, blank sentences and its language before the user types.
Those requests move nothing the user could see undone, so they leave no step behind.
The chronicle clerk keeps the step once the engine has judged it.

## `LEngineChronicleUndo`

Undo and redo step the held draft one snapshot and return the draft as saved.
They return null when nothing is behind or ahead.
A step raises a draft bulletin, so every open editor of the draft reloads.
The undo and redo checks tell whether a step exists, so a button can dim before it is pressed.

## `LEngineCourtSave`

It writes one court row, a link from a held draft to a target that is not an entry yet.
It returns the row so the caller can drop it again by id.
Start makes a tentative target and records the row naming it, as one call.
The word and the language are written straight into the target's file before the row is made.
A failure anywhere after the target is started cancels it before the refusal leaves.
So the workspace holds either the target with its row or neither.
Delete removes one court row for a chip the user took back.
Find answers the row one draft holds against one target, or null.

## `LScriptFacade`

The engine facade for script images, the pictures of an entry's characters in each style its pack names.
A script style is a fact of the pack.
The fetch, the store and the grouping belong to the script clerk alone.
It implements the script port itself, so Host hands it to Conduct with no outlet between.
It names no sibling facade, since the script clerk answers every call.
The gate and the staff are read through the hearth at each call.
So a workspace switch is seen at once.
Read groups the images by style after starting the fetch of every character still missing, while divide only groups.
Rebuild drops the stored images of the entry's characters and fetches them again.

## `LSettingsFacade`

The engine's facade for settings.
The user's persisted settings are read here and changed one field at a time.
Every change is written to the open workspace at once, and a failed write restores the previous settings.
The record itself is loaded when the workspace opens.
A workspace moved onto keeps its own settings, and only one without any inherits the current record.
It calls no sibling facade, so `LEngine` builds it among the first.

## `LEnginePostureLoad`

The posture stored under a name is read through the workspace clerk.
False means a vault fault, recorded and handed back as `fault`, and the caller keeps what it has.
Save answers whether the write succeeded.
A vault fault is recorded by the clerk and handed back as `fault`, never thrown.
`fault` is a plain `Exception`, so the shell names no Core type.

## `LEngineFileFormat`

The file name a headword is offered under, made safe through `LEngineTrailNormalize`.
A blank or missing headword is offered as `entry`.
The vista's export name and the galley's PDF name share it, so both offer the same name.
`LEngineFileFormat` is the one caller of the normalizer, so every offered file name passes through it.

## `LEngineLocalizationRead`

The stored interface language is normalised to a listed one, so a deportment never names the localization.
The text reads answer the key itself, or null for the find form, when no text is loaded.
The scan lists the interface languages the build embeds.
`LEngineCultureRead` answers that language's culture through `LLocalizationCultureRead`, the one owner of the mapping.
  A gate words dates and numbers with it, so a figure follows the interface language and never the machine's culture.

## `LEngineLocalizationSave`

It persists the chosen interface language and keeps it current.
Only that field is written, so a switch flipped by another part of the shell survives the change.
A settings bulletin is raised when the language changed, so the shell applies the catalog from the record.

## `LEngineRespellingSave`

It persists whether readings are shown and edited in their respelled form and keeps it current.
Every reading is stored in both forms, so a flip only changes which one each surface shows.
A settings bulletin is raised when the switch changed, so every open reading re-reads itself at once.
The check says respellings show for a language when the setting is on and the pack declares respelling groups.

## `LEngineMarkRead`

It tells whether readings of a language show their respelling, and the brackets around them.
A respelled phonemic pack writes its readings between slashes.
Every other reading stands between square brackets.
This is the one owner of the bracket rule, so the editor and the reading view print alike.

## `LEngineEpithetSave`

It turns the epithet after every listed headword on or off.
A settings bulletin is raised when the switch changed, so the settings panel rewrites its summaries.
The frequency, morphology, analysis and notebook saves raise the same bulletin on a change.

## `LEngineTallySave`

It persists whether the tally lines of a category page print the respelling set and keeps it current.
The page reads the switch on every fill, so the choice survives a restart and a change of category alike.

## `LEngineFrequencySave`

It persists whether an entry's frequency is fetched from the pack's web source and keeps it current.
The next fill reads the switch, so a flip neither refetches what is stored nor drops it.

## `LEngineMorphologySave`

It turns the morphology fetch on or off.
Turning it off cancels every pending inflection fetch through the lacuna clerk.
The check tells a reading view whether to offer its paradigm.

## `LEngineAnalysisSave`

It turns custom analysis on or off.
It changes only what the inflection box shows, so nothing is analysed again.
An inflection bulletin for every entry follows, so the reading view and the editor repaint their box.

## `LEngineNotebookSave`

It persists the title of the top Joplin notebook and answers the title it stored.
The settings record trims it as it is set, and a blank title stores `Llyn`.
So the settings field shows what the next sync will write, not what was typed.
Nothing is sent to Joplin here, since the next push reads the title afresh.

## `LEngineDepotSave`

It persists the deck sync folder and answers the folder it stored.
It relays only, so the trail clerk decides the path rule and the settings change stores it.
An absolute folder outside the workspace root raises `Refusal.PathRooted` before anything is written.
A relative folder that does not resolve inside the root raises it too.
One inside the root is stored relative to it.

## `LEngineSettingsChange`

It applies one change to the settings snapshot under the gate and writes it through the workspace clerk.
An unchanged snapshot writes nothing and answers false.
A write that throws puts the previous snapshot back before the exception leaves, still under the gate.
The caller then raises no notice, so a rolled-back change tells no one.
The courier facade also calls it, so the hidden token is stored the same way.

## `LTenureFacade`

The engine's facade for Tenure, which starts a draft hold for a vista or named subject and commits it.
Alone among the facades it takes the engine, since its tenures reach every facade.
The tenure delay is the quiet a tenure waits for before writing what was deferred, in milliseconds.
It is read and written volatile, so a change from one thread reaches the deferrals made on another.

## `LEngineTenureStart`

The vista overload starts a tenure over the vista's tab and subject.
A vista with no subject cannot name what to hold, so it is refused.
The subject overload starts a draft of the kind the subject names, on a stored record or on nothing.
Only an entry, example, situation, reference or author can be held, so another subject is a caller's error.
A missing stored record refuses as the underlying start does.

## `LEngineOccurrenceStart`

The occurrence, quotation, footnote, membership and cohort starts each make a fresh entry over the vista.
Each links one catalog row to the entry's first card before anyone sees it.
The row is a Situation, an Example, a Source, a Tag or a Register.
The link is one pick or citation request applied at once, so the clerk's own rule applies unchanged.
A null row leaves the fresh entry blank.

## `LEngineTenureCommit`

It commits the draft through the part the subject names and answers the stored record's id.
It mirrors the start beside it, so one owner maps each subject to its engine part.
A tenure is only ever started for these subjects, so another one is a broken invariant, not a caller's error.

## `LVocabularyFacade`

The engine facade for vocabulary, inflections and paradigms.
Every call hands the work to the vocabulary, paradigm or lacuna clerk.
Every read of stored rows takes the gate first, and the lacuna clerk holds it for the fetch calls.
The facade stays because the shell calls the engine, and the engine alone holds the gate and the pending fetch.
It implements the paradigm and sentence ports itself, so Host hands it to Conduct with no outlet between.
It calls no sibling facade, so `LEngine` builds it among the first.

## `LEngineSpeechAdd`

It makes a typed part of speech one of the language's presets.
Or it returns the one that already carries the name.
The speech read lists the parts of speech in the vocabulary clerk's order.
The particle and dependence reads list the markers and roles already saved under entries in the language.
The order read says which of a sentence's two frame fields the pack writes first.

## `LEngineParadigmScan`

It joins the entry's paradigm slots into the rows a paradigm box shows.
Slots on the part a layout draws are left out, since the inflection view shows them.
The language resolve answers empty when the entry has no slots.

## `LEngineInflectionRead`

The entry's inflection view is built by the paradigm clerk under the gate.
It reads the custom analysis setting under the same gate.
That setting picks the sheets and whether marks and root splits show.
The held flag passes through to the clerk unread.
The facade adds no rule of its own.

## `LEngineParadigmCheck`

It gives the text and tip key that stand in a paradigm row for its form, through the paradigm clerk.
It takes no gate, since the rule reads only the row and the three flags.
The inflection check and start answer and begin the fetch through the lacuna clerk.

## `LWorkspaceFacade`

The engine's facade for workspace state, trail operations, the workspace folder and failure records.
It is public, because Host reads the rescue and hands the audit record to the deportment.
It takes its siblings rather than the engine, so it names only the facades it uses.
The folder and the rescue are read from the first rig, so a reader sees them before any switch.
Both are replaced under the gate on every rig apply.
The rig factory and pointer writer come from Host, so the engine names no infrastructure.

## `LEngineWorkspaceStart`

The order of opening a workspace is the draft sweep, the recording sweep, then the state row.
Each sweep takes its own facade's lock, so this member holds none.

## `LEngineWorkspaceChange`

It moves the engine onto the folder `chosen` names, trimmed by the clerk, then records it as the workspace.
A choice that names no other workspace throws, since the check comes first.
The pointer is written only after the move succeeds, so a failed folder never becomes the next launch's workspace.
The same open a start runs follows, so the new workspace is swept before any view restores on it.
The check asks whether the choice names a workspace other than the one in use, by the workspace clerk's rule.
Conduct asks it before the leave question, so a blank or unchanged path asks nothing.

## `LEngineRigApply`

It moves the engine onto the workspace `rig` was built over, without touching the workspace pointer.
`LEngineWorkspaceChange` builds the rig and writes the pointer after.
The new rig's rescue is read before anything changes, so a folder that cannot open leaves all as it was.
The engine then moves its clerks, caches and settings, and the folder and rescue follow under the same gate.
The move is then announced, so every surface holding a stored record learns that all of it is stale.

## `LEngineRescueRead`

It reports what the workspace doctor had to do to the database this engine opened.
The facade holds the answer rather than raising it, because the shell asks once the engine exists.
The answer is replaced when `LEngineRigApply` opens another workspace.

## `LEngineWorkspaceFormat`

It gives the workspace folder's own name, for the settings ledger, and the full path when the root has none.

## `LEngineAuditRecord`

It writes one unexpected fault into the open workspace's audit log through the workspace clerk.
It answers with the file the fault went to, or `null` when nothing could be written.

## `LEngineNoticeRead`

It gives the reason key of a refusal standing anywhere inside the failure, or null for a fault.
The workspace clerk walks the inner chain, since the shells name no exception type.

## `LEngineFailureRead`

It builds the ready notice of a failure, in the order the reasons are tried.
A refusal answers its own reason key alone.
A fault is written to the audit log and answers `unexpected`, with `recorded` and the file when the write succeeded.
Conduct hands both wording keys down, so the engine chooses no wording of its own.

## `LEngineLeftSave`

Left and right save remember the entry shown on each side.
Both go through one private change that reads, changes and writes the state row under the gate.

## `LEngineLocationRead`

It resolves a location, or answers null when it is a file that does not exist.
The screen read resolves the address of a video location and its hosted film id, by the Application trail clerk.
The location open and folder open go through the shell usher.
The folder path is read there, so the caller hands nothing it gathered.

## Stamp

Commit: `bd299c638b2c`
- `LDraftFacade.cs` `e6ad399867a18e7e`
- `LLanguageFacade.cs` `22bf9bcec6db6495`
- `LRequestFacade.cs` `9a08f1ac9c85e708`
- `LScriptFacade.cs` `0868062d56c4daf9`
- `LSettingsFacade.cs` `53cfb98424acac7c`
- `LTenureFacade.cs` `c7d7ee4457051f6e`
- `LVocabularyFacade.cs` `f1299bc555399645`
- `LWorkspaceFacade.cs` `f0eb53a0cb7a7ee9`
