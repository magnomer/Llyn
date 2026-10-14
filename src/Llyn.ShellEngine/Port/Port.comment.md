# Port

This folder holds the four engine slices the conduct sees that span several facades.
Each port has a sealed outlet, which Host builds on the engine and hands to Conduct.
An outlet reaches the owning facades through the engine and holds no other state.
A port stands apart from the engine so the layers above never name the engine itself.

## `LDraftOutlet`

The engine's face for `LDraftPort`, handed by the host to the conduct in place of the engine itself.
Every member forwards to the engine part that owns it and holds no state.
It rejects a null engine and keeps it for every forward.
The meaning scan forwards to the card facade.

## `LDraftPort`

The slice the conduct sees when it holds or links a draft.
It starts a tenure, drops a draft, attaches an observer, and answers the link lookups a card edit asks.
It also divides a sentence around its Mentions, converts text offsets, and checks or joins anchors.
`LDraftOutlet` implements it today, and the draft clerk takes it over when the parts are dismantled.
Member names keep the engine's `LEngine*` form until that hand-over renames them once.

The vista tenure start holds the record `id` names in the vista's subject, or a fresh one for null.
The origin start holds a record of any holdable subject under a named origin, as the desk does.
The occurrence, quotation, footnote, membership and cohort starts open a fresh entry.
It comes already linked to the catalog row passed, if any.
`LEngineDraftDelete` removes the court links the held draft owns, then drops the draft and its claim.
A draft bulletin with id zero follows, since the held set the status bar counts shrank.
Attaching the same delegate twice still calls it once.
`LEngineLeftoverSweep` cancels every unheld draft that says nothing its origin does not.
A swept draft can settle a court into a held draft, so no tenure may keep its state across it.
`LEngineCourtStart` starts a tentative target and records the court row naming it, as one call.
A failure after the start cancels the target, so the workspace holds both or neither.

The find calls serve the chip fields a card makes as the user types.
`LEngineBylineFind` lists the Authors a typed credit may name, left out those the draft credits.
Each name comes split around the typed word, ready to paint.
`LEngineTagFind` offers stored Tags, and the register find offers stored Registers, ready to show.
The held draft names the language, so the register caller passes none.
`LEngineSituationFind` offers stored Situations, and the held draft names the card's links, so those are left out.
`LEngineReferenceFind` offers stored Sources for a sentence's citation field.
The held draft names the Source the sentence cites, so typing its byline offers nothing.
`LEngineCitationResolve` gives the id of the Source a typed title names, minting it when none matches.
The held draft names the Source the sentence cites now, so an unchanged title keeps it.

`LEngineMentionResolve` gives the chip line of every sentence row on every card of the held draft, keyed by the row.
Each chip carries the words it covers, the linked headword and the chosen sense.
The card and sentence overload addresses one Example, and card 0 with sentence 0 addresses the draft's own Example.
`LEngineMentionDivide` is static, since Conduct divides a row's text while it maps the row and no port is at hand.
Mention spans count code points while a text box counts UTF-16 units, so the engine converts.
`LEngineUnitRead` gives the UTF-16 index of a code-point offset, and `LEngineOffsetRead` is its inverse.
`LEngineSpanRead` measures a field's selection in code points.
`LEngineSpanCheck` says whether it covers any, so a link command may run.
`LEngineSenseRead` lists the Meanings the sense menu offers for the linked Mention under the selection.
Pending typing is persisted before the find, and no linked Mention answers null.
The meaning scan lists every Meaning of the held draft at any depth, in reading order, each with its path.
It feeds the link picker of a collocation card and the entry folio.
The caller passes the localization key that names an unnamed Meaning.
A tenure that holds no draft answers an empty list.
`LEngineAnchorCheck` says whether the reflex rows of a headword may carry anchors, judged on the entry's own rime-book rows.
`LEngineAnchorFormat` joins the readings of the anchored rows, empty when the rows cannot be anchored.
`LEngineTranslationFind` gives the offer a typed translation opens.
It carries one row per match and the languages a new Entry is offered in.
`LEngineProspectFind` lists the Entries a mention of a word may name, in the draft's own language.

## `LMediaOutlet`

The engine's face for `LMediaPort`, handed by the host to the conduct in place of the engine itself.
Every member forwards to the same-named member of the engine part that owns it and holds no state or logic.
It rejects a null engine and keeps it for every forward.

## `LMediaPort`

The slice the conduct sees when it shows or plays media.
It resolves a stored location to a playable address, says whether a recording file exists, and prepares one for play.
It plays, stops and sets the level of a recording through the phonograph, so the veneer owns no player.
Each play answers a ticket, and a stop acts only for the ticket of the latest play.
It opens a folder or a link through the usher, so the veneer starts no process.
`LMediaOutlet` implements it today, and a media clerk takes it over when the parts are dismantled.
`LEngineLocationRead` gives the playable address of a stored location, or null when it names a file that does not exist.
`LEngineScreenRead` gives a video location's address and hosted film id in one read.
So the video row parses no address and makes no second request.
`LEngineRecordingPlay` plays nothing for a file that does not exist.
It answers the play's ticket, or zero when nothing played.
The draft overload plays the audio of the draft's first pronunciation.
`LEngineSpanRead` is static, since it reads only the row the caller holds.
It gives the moments a video row plays from and until, so the row parses no timestamp.
`LEnginePlaybackRead` says whether the draft's own recording exists and whether a reading view has anything to play.
`LEngineAudioRead` gives the recording while its file exists and whether the editor's playback tray shows.
`LEngineAudioResolve` gives the playable address of a stored recording, or null when its file is gone.
`LEngineRecordingStop` stops the playing recording only while the ticket names it, so a stale stop never cuts a newer play.
It skips the engine gate, so it never waits behind a long engine call.
`LEngineVolumeSet` skips the gate for the same reason.

## `LPortraitOutlet`

The engine's face for `LPortraitPort`, handed by the host to the conduct in place of the engine itself.
Every member forwards to the same-named member of the engine part that owns it and holds no state or logic.
The press and export calls go to `LEnginePortrait`, and `LEngineMarkupStart` goes to `LEngineMarkup`.
The courier calls go to `LEngineCourier`.
`LEngineCourierSend` forwards the wording `lookup` the Joplin bodies are written with.
`LEngineCourierCheck` forwards the token check to the courier facade, so no view reads the settings for it.
It rejects a null engine and keeps it for every forward.

## `LPortraitPort`

The slice a view sees when it prints, exports, imports or syncs to Joplin.
Printing and exporting portray the record a vista chose, so the caller hands the vista and never the id.
The Joplin check, push and attach each run as one call.
The markup import runs as one call, which reads the cargo and stores it under declared intakes.
`LPortraitOutlet` implements it today, and a portrait clerk takes it over when the parts are dismantled.
`LEnginePressOpen` with a label opens a print session on the chosen entry, and answers null when the vista holds none.
The legend overload opens one on the chosen Example, Situation or Source, and answers null when none is chosen.
Any other subject is a caller mistake and throws.
The caller owns the relay and disposes it when the session ends.
`LEnginePortraitExport` exports the chosen entry to a path, and does nothing when the vista holds no entry.
`LEngineMarkupStart` reads the markup file, asks `declare` for the intakes, then stores the cargo.
The read and the store run on a worker thread, while `declare` runs back on the caller's thread.
A `declare` answering null stores nothing and answers null.
Otherwise it answers what the read and the import left behind.
`LEngineCourierCheck` says whether a Joplin token is stored, so the shell words its Joplin control as connected.
`LEngineCourierSend` pushes every entry into Joplin one way, worded through `lookup`.
The receipt counts what was saved, kept and removed, and lists what failed.
A token Joplin refuses is dropped from the settings before the refusal is thrown.
`LEngineCourierAttach` asks Joplin to grant access and keeps the hidden token in the settings.
The caller never sees the raw token, so it cannot leak into a view.
The static reads build the records a print or export takes from plain values, so Conduct builds no engine record.
They are the label from words in the record's order and the export formats with suffix and default flag.
They also give the Source kind words.
They also build the intake one declared row stands for, through the intake clerk.
A new entry carries no target, which the Core factory enforces.
The legend read words a catalog page and each Source kind by its stored word.

## `LSettingsOutlet`

The engine's face for `LSettingsPort`, handed by the host to the conduct in place of the engine itself.
Every member forwards to the same-named member of the engine facade that owns it, adding no logic.
The engine is its only state.
The notebook save forwards too, and hands back the title the facade stored.
It rejects a null engine and keeps it for every forward.

## `LSettingsPort`

The slice the conduct sees for settings, the workspace and the interface texts.
It reads and saves the settings, the workspace path, the workspace state and the interface language.
It also words a failure into a notice, and loads fonts and flags.
The interface texts are read through it, so the conduct names no localization.
Nothing here changes an entry or a draft.
`LSettingsOutlet` implements it by forwarding to the owning engine facades.
The saves and checks behave as `LSettingsFacade` documents them.

`LEngineWorkspaceChange` moves the engine onto the raw folder, records it, sweeps it and answers its state.
Conduct changes the workspace in one call, without the engine itself.
`LEngineWorkspaceStart` opens the current workspace for the session in one call.
It sweeps leftover drafts and recordings first, then answers the workspace state.
`LEngineFolderOpen` opens the workspace folder through the shell usher.
Conduct opens it in one call, without reading the path first.
`LEngineWorkspaceCheck` takes the raw path, so Conduct asks nothing for a blank or unchanged one.
`LEngineLocalizationScan` lists the embedded languages, so the language box lists no language named in markup.
`LEngineLocalizationLoad` gives the strings of one language parsed for the resource dictionary.
`LEngineTextFind` returns null when no text is loaded, so a caller can tell a missing text.
`LEngineGroupFind` matches by the localization's shared match, and blank text names every group.
`LEngineAnalysisSave` turns custom analysis on or off.
On shows the pack's custom sheets with marked letters split at the root, and off shows the default sheets plain.
The inflection box of the reading view and the editor repaints after a change.
`LEngineNotebookSave` answers the trimmed title it stored.
A blank title stores `Llyn`, and the next sync renames the same notebook to the stored title.
`LEngineFailureRead` gives a refusal's reason key, or else `unexpected`.
`recorded` and the audit file come with it once the fault is written.
One call reads the reason and writes the fault, so Conduct decides nothing about the exception.
`LEngineFontRead` gives a blank font for a blank language, so the theme's own stands.
Otherwise it gives the language pack's font for the role.
The glyph role falls back to the example typography.
`LEngineEnsignLoad` fetches the flag of every language the cache has not yet asked, then hands the new rows to `store`.
A fill that a workspace change outran records nothing and calls nothing.
It resumes on the caller's context, so `store` runs on the shell's thread.
A flag file that cannot be written or read answers the languages without flags.
`LEngineEstablishmentRead` counts the held drafts that differ from their origin, the stored entries and the database bytes.
Each held draft is measured as the leave dialog measures it, so the two cannot disagree.
`LEngineLanguageRead` keeps its list until the workspace changes, so an entry switch never scans the disk again.
`LEnginePhonemicCheck` lets the reading field show a phonemic respelling between slashes.
`LEngineRespellingCheck` needs the setting on and respelling groups in the pack.
`LEngineEnsignFormat` is static and gives the key a variety's flag is kept under, through the ensign's one key format.
A blank variety has no flag, so its key is empty and finds nothing.

## Stamp

Commit: `c6e60141a172`
- `LDraftOutlet.cs` `3c00d5a269978c47`
- `LDraftPort.cs` `b5d61d945bfd7907`
- `LMediaOutlet.cs` `cc595890330f42a2`
- `LMediaPort.cs` `ba9007f05060ea3d`
- `LPortraitOutlet.cs` `4b7d31ff3f0aa3c2`
- `LPortraitPort.cs` `e9d21be77d9ef27b`
- `LSettingsOutlet.cs` `8616ce696d985f3d`
- `LSettingsPort.cs` `96ab98b7a2a6a4f5`
