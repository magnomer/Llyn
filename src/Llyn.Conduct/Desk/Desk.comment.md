# Desk

Desk holds the seat at one tenure, which every edit area shares through its own deportment.
It owns the held draft, its undo chronicle, its quills and the errands that search while the draft is open.
It stands apart from the panels, which each build a desk and compose areas over it.

## `CDesk`

The Conduct seat at one tenure.
It starts, finishes and cancels the held draft, and applies the requests deferred on the tenure.
The tenure is started over the panel's vista, whose tab and subject name the draft's origin and kind.
The only question it asks is whether an unreadable draft may be swept, through `CEnvoy`.
It shows its own load and save failures through the same envoy, so no driver can lose one.
The chronicle shows the hold failures through that envoy too.
Only the controllers that own a desk build one, so the port never reaches a driver.

`CDeskStarted` fires after the vigil has put the driver's observers on the new tenure.
`CDeskStateChanged` fires when the tenure came, went or changed, so the store and chronicle buttons are read again.
Every chronicle announcement is raised again there, so listeners hear one event.
`CDeskFinished` carries the stored id, raised after the tenure has been let go.
Only the finish without a stored action raises it.

`CDeskId` is the held draft's id, not the stored record's, and zero while nothing is held.
`CDeskStored` reads `CDeskStoredRead`, so a refused read answers false.
`CDeskStoredRead` makes the one tenure call `LTenureStoredRead`, which persists nothing, so pending edits stay pending.
The tenure answers a refused read with null, so the desk catches nothing itself.

### Start

`CDeskStart` drops any held tenure and starts one over the record, or over nothing when the id is null.
A desk with a restored vista starts over it.
Without a vista, a desk built with an origin and a subject starts under them.
The subject reaches the engine through the panel's map by name, never by cast.
The repertoire and corpus desks start this way, since their vistas list the records rather than the tab.
`CDeskStartRun` clears the chronicle's halt, so a new tenure starts unhalted.
The vigil applies its observers to the new tenure before the start is announced.
A refused start is shown under the scope's `LoadFailed` key and leaves the desk empty.
Each start announces the change of state once, because every start clears through `LDeskTenureClear`.
A start that starts nothing still announces when it dropped a held tenure.

`LDeskRun` lets a panel hand in its own engine start, such as an Occurrence linked to its Situation.
The engine starts and links in one call, so the first draft shown carries the link.
A desk with no vista starts nothing.

### Finish

`LDeskChangeCheck` and `LDeskReadyCheck` apply the deferred requests first, then answer.
The tenure writes and answers in one call, and an empty desk answers false.
Only a store asks the ready check, since it writes.

`CDeskFinish` ends the tenure, storing or dropping it as asked, and reports whether it ended.
A failed store is shown under the scope's save key and keeps the tenure, so the user can read why.
The desk chooses the unreadable-drop key, and the tenure asks through `CEnvoy` before it sweeps.
The overload with a `stored` action hands the stored id there instead of the event.
So a caller decides per finish what follows a store, without holding a flag.
`CDeskCancel` announces nothing when no tenure was held.

### Failure routing

`LDeskFailureShow` serves helpers that hold this desk, such as `CErrand` or `CByline`.
The key is the suffix after the scope, such as `.RecordingFailed`, so each area's key keeps its prefix.
It answers a user act, so every failure shows.
`LDeskRepaintShow` serves repaint reads and merges a shown key until the user acts.
The memory belongs to the atelier, not the desk, so the helper hands it in.
`LDeskRepaintRead` runs a read through that memory.
A failure shows the scope's key once and answers the fallback it was given.
A refused typing read thus shows `.FindFailed`, while an absent draft is no fault and answers empty.

### Observers

`CDeskObserverAttach` decides which bulletins refresh the desk, for every tenure it starts.
A tenure bulletin runs the chronicle's resonate, and a draft bulletin runs the `drafted` action.
A draft bulletin that carries a refusal first shows the scope's `.UpdateFailed` notice every time.
Both run through the driver's marshal.
The overload without `drafted` rereads and announces the draft through `CDeskDraft.CDeskDraftResonate`.
The errand takes the same marshal for the steps of its recording search.

## `CDeskChronicle`

The undo and redo steps of one desk's held draft, and the halt the tenure reports.
It reads the tenure through the desk's `CDeskDraft`, so the draft keeps the only hold.
It shows its hold failures through `CEnvoy`, so no driver can lose one.
`_cDeskChronicleHalted` records the halt at the last bulletin, and a start clears it.
`CDeskChronicleHalted` tells the edit area to go dead, and `CDeskChronicleRunning` keeps it live.
`CDeskChronicleResonate` shows the scope's `HoldFailed` notice once per halt, then announces.
A failed undo or redo step is shown under `HoldFailed` and still announces.
The desk raises its `CDeskStateChanged` on the chronicle's change, so drivers hear one event.

## `CDeskDraft`

The held tenure of one desk, with the quills and engines built over it.
Every quill getter answers null while no tenure is held, so no gate reaches a dropped tenure.
The quills are also null while the desk fills its controls, so a fill's echo writes nothing back.
`CDeskDraftSpeech` is the exception.
It stays open during a fill, since the field read runs then, and each edit gate checks the fill itself.
The speech quill keeps the pending part typed into the field, so the desk holds one per tenure.
The reference, author, example, sentence, mention and transcription quills are built fresh for each use.
The example quill also takes the desk's draft port.
The errand, easel, chip and speech quills are built when a tenure starts and dropped when it ends.

It raises the draft announcements, so the fill flag and the echo it guards live together.
`_cDeskDraftFilling` is read through its verdict and never branched on by name, so the walker sees a verdict.
`CDeskDraftPrepared` fires before `CDeskDraftChanged`, so a controller's own announcement reaches its driver first.
A driver receives only the Conduct copy.
`CDeskDraftAltered` reports the tenure's last verdict and applies nothing first.
`CDeskDraftStorable` is the tenure's verdict, so the holder combines no state fields itself.
`CDeskDraftTenure` is the one place every state and step is read through.

`LDeskDraftSet` runs once per successful start, before the vigil applies its observers.
`LDeskDraftClear` announces nothing, and the desk decides what follows.
`CDeskDraftResonate` rereads and announces the draft, unless the announcement itself is what is running.
A failing read is shown under `LoadFailed` and announces nothing.
`CDeskDraftPrepare` runs the tenure's completion inside its prepare turn, so the rows it adds raise no draft bulletin.
`CDeskDraftPersist` applies the deferred requests as a field is left, unless the controls are being filled.
`CDeskDraftShow` raises the filling flag while it announces, so the controls' echo is ignored.

## `CEditor`

An editor session composes every area over one desk, keeping drivers behind shared Conduct gates.
It opens, stores and drops drafts without exposing its ports to drivers.
Every desk uses the `Input` notice scope, regardless of its tab.
Each child receives only its required ports, while repaint-read failures share the supplied notice memory.
The desk's finish handler precedes the entry area's draft subscription.
Listening areas receive the marshal, so engine notifications need not depend on driver-owned thread dispatch.
Settings notifications resonate the held draft because its presentation depends on settings.
Each editor receives its own session over the atelier's shared dependencies.
Workspace opening replaces the held entry with a fresh draft.

The speech, field, list and display areas follow the shared desk across drafts.
The image, video and transcription areas are stateless and can be built afresh over the same desk.
`CEditorFold` is built over the desk's stored entry, which it asks on every read and write.
It listens to nothing itself, since `CEntry` supplies draft notifications when the fold bulletin arrives.
`CEditorClose` cancels the held desk session before cancelling its errands.
`CEditorEtymology` edits the held entry's etymology map over the desk and the ancestor port.

`CEditorOwned` is true only for an input vista, and an unrestored vista answers false.
`LEditorVistaRestore` gives the desk and display the same vista, keeping their tab context aligned.
`CEditorEntryOpen` with null opens a fresh draft.
A requested entry that leaves no held draft falls back to a fresh draft.
`CEditorEntryUndo` reopens the stored entry, or a fresh draft when none exists.
`CEditorEntrySave` does not finish an unchanged draft, and a changed draft uses the same finish path as leaving.
It answers whether the store ended the tenure, so an unchanged draft and a failed store both answer false.
`LEditorFinish` captures the fresh-entry verdict before finishing.
So post-store reopening uses the draft's former identity.
`CEditorStoredShow` reopens a freshly stored input draft blank and every other stored entry as itself.

## `CEntry`

The head fields of the entry an editor's desk holds: headword, reading, note, language, unit and freedom.
It announces each draft the desk prepares, so every driver writes its controls from one event.
It listens to the desk's prepared drafts from the start, so no draft is prepared unheard.
It hands each field edit to a quill.
A desk that holds no tenure or fills its view takes no edit.

`CEntryDraftChanged` carries the content the desk prepared, never a second read of the draft.
The chip quill resolves the cards' link targets for that same content, keyed by card kind and id.
Each card also carries its stored fold and whether it can fold.
Each sentence of a card carries its line key, which `LEntryDraftRead` asks the draft port for.
`LEntryDraftRead` is the one shaping, used by both the prepared draft and the fold bulletin, so the two never drift.
It is internal so the test relay reads the draft the way the event hands it.

`LEntryObserverAttach` hears the fold bulletin of the held entry through the editor's marshal.
So a fold made in the reading view or another editor repaints this editor's chevrons.
A draft never stored hears nothing, since none of its cards can hold a fold yet.
A fold is not an edit, so the bulletin never touches the tenure or its undo.
`LEntryFoldRead` reads every folded card in one read, each as its kind and id.
A draft never stored answers an empty set.
A refused read shows `Fold.ReadFailed` once until the user acts, and answers an empty set so the cards still draw.
A refused read of the link targets shows `Input.FindFailed` the same way and draws the cards without targets.

`CEntryUnitSet` and `CEntryFreedomSet` clear the value already held, a rule the entry quill keeps.
Their menus are read with the part-of-speech field.
`CEntryNoteCheck` tells whether the held note equals what the typed text resolves to.

## `CErrand`

The recording and reading searches one desk runs over its held tenure.
The desk builds it over itself, so no driver ever holds a foray.
It keeps the clip popup's state, which the recording search fills.
The notation popup's state, which the reading search fills, sits beside it.
`_cErrandMarshal` runs a step at once until the desk attaches the driver's marshal.
Each engine step reaches the popup's state through the marshal, before any state changes or event rises.

`CErrandClipChanged` and `CErrandNotationChanged` carry the ready state after every change.
A starting source, a found result and the search's end each raise once.
Preview and taking stages also raise the clip, since each changes a recording's look.
So the popup repaints and needs to tell no cause apart.

`CErrandRecordingStart` and `CErrandTranscriptionStart` take a nullable target.
Null means no row, the headword's own search.
Conduct maps null to the engine's row 0, so the driver never sends a magic id.
`CErrandTranscriptionStart` also takes a transcription row's id, and reads that row's scheme from the held draft.
An id with no row in the held draft, or no held draft, starts the search with an empty scheme.
So the driver names no scheme, and the nullable form stays for a row not yet stored.
Both stop both searches and clear the popup first.
The tenure reads the draft's headword itself and starts nothing for a blank one.
The popup is marked searching before the start, since a step may land during it.
A start that answers nothing or fails leaves the popup finished with its empty notice.
A failed start is also shown under `RecordingFailed` or `TranscriptionFailed` through `LDeskFailureShow`.
A schemed start picks the transcription notice, and a plain one the reading notice.

`CErrandEnsignLoad` and `CErrandFlagLoad` load flags for the running search's varieties, after the start.
The foray loads nothing when its language shows no flags, and for a schemed reading search.
The foray already leaves the flags out when a flag file cannot be written or read.
Any other failure shows the search's notice, and the state is still answered.
So readings that landed before the flags repaint with them.

`CErrandReadingSet` does nothing while no reading search runs or the desk fills.
The quill routes the reading by the search's target and writes it at once.

`LErrandLookupResonate` takes the step's own foray, even before the start has returned it.
A step whose foray was cancelled belongs to a replaced search, so it changes nothing and raises nothing.
The foray's own mark decides it, since a step may land before the start has returned the current foray.

`CErrandPreviewStart` marks the recording fetching, and the foray fetches it into the workspace.
Streaming the remote, token-bearing address through the media stack is unreliable, so a local file plays.
It answers nothing when another preview took over or the popup closed during the fetch.
A failed fetch marks the recording refused and shows the recording notice, so a refusal is not mistaken for silence.
The foray answers no path only when nothing was prepared, and that is refused without a notice.
A path that is no absolute address is refused the same way.
Only one preview is marked at a time, so starting another clears the last.
`CErrandPreviewFinish` raises the clip only when a preview was marked, since the player is shared with other sounds.

`CErrandRecordingSave` answers whether the popup closes, and only an attached recording closes it.
The download is reported on the recording taken, not on the popup's notice, which belongs to the search.
A taken recording then reads saved, even when the draft moved on and nothing was attached.
A failed download offers another try and shows the recording notice.
The foray answers null only when nothing was saved, which offers the retry without a notice.

`CErrandCancel` stops both searches and forgets the marked preview, so a fetch that lands later plays nothing.
The static run helpers take their handles as parameters, so no field decides the start.
The recording and candidate copy maps convert engine shapes both ways, since the clip saves and plays what it listed.

## `CEsteem`

The user's marks on the stored entry an editor holds, its favourite flag and its grasp step.
The frequency chip is read here beside them, since all three stand on the stored id.
The editor builds it over its desk and its display, which holds every favourite and grasp rule.
It only supplies the stored id, and a refusal reaches the window through the display's envoy.

Favourite and grasp read false and zero for a fresh draft or a failed read.
The grasp is never negative and never above `CEsteemGraspStep`, since the stars draw only that range.
`CEsteemGraspStep` is never negative, since `CRule` raises a negative engine limit to zero.
It is the star control's limit, so the control names no engine constant.
The fill, unrated and hit tables are handed to the control once, so drawing and pointing request no logic.

`CEsteemFavoriteChanged` is raised only by the set gate, refused or not, so the star reads the store again.
No bulletin raises it, unlike `CEsteemGraspChanged`, which a grasp bulletin about the held entry also raises.
That bulletin is marshalled onto the driver's thread.
Each set announces a re-read so a refusal redraws.
`CEsteemGraspMove` shifts the step by a signed amount, clamped to zero and `CEsteemGraspStep`.
A move onto the standing step sets nothing.
So a key at either end neither clears nor rewrites the grasp.
Pressing the step that stands clears the grasp, by the display's one rule.
`CEsteemGraspRead` words a step through the engine, so the driver names no localization key.
The read with no step answers the standing step and its wording together, so the driver asks once.
It is empty for a fresh draft.
`CEsteemFrequencyRead` takes the localized word for a one-off figure, so Conduct stays out of localization.
It answers null when no source ranks the entry.

## `CSession`

One side panel's draft session over its desk, shared by the repertoire, corpus, guild and shelf.
It starts, finishes, saves, undoes and redoes the desk, and defers to the entry editor while it shows.
It also asks before leaving unsaved changes.
It holds only the editor's desk, so the panel that owns the editor closes it.
Each variant of a panel enters as a constructor parameter or a seam, never as a second copy.

The pending checks are asked in order.
The editor's desk takes over while the shown seam answers true, and the guild hands no editor desk.
The ready seam may refuse a store, and the stored seam shows what a store kept.
`CSessionHeld` is raised after a start or a cancel, so the owner announces what the desk now holds.
`LSessionFinish` finishes the editor while it shows, else the desk, and shows a stored record again.
A store the ready seam refuses leaves the desk open and answers false.
`CSessionClose` leaves a stored record unshown, because a row click shows its own row next.
`CSessionSave` asks the ready seam first for the desk, so a blank guild name is refused even unchanged.
A held desk with no changes stays open and answers true.
Undo and redo go to the editor while it shows, else the desk.
Each desk shows its own failed step, so the session catches nothing.
`LSessionLeaveConfirm` asks only when a panel holds unsaved changes.
A cancelled question answers false, and so does a store the ready seam or the desk refuses.

## `CVigil`

The observers one desk keeps by subject and puts on every tenure it starts.
A driver registers once and never sees the tenure.
The vigil re-attaches its observers on every start, and a tenure already held gets the observer at once.
There is one attach for the tenure-wide, draft-level and entry-level subjects.
An observer is a delegate over a Conduct bulletin, which the vigil copies from each engine bulletin.
The subject maps through `CCatalog.LCatalogSubjectRead`, so a subject it does not list throws.
`CVigilApply` puts every observer on the tenure before the desk announces the start.

## Stamp

Commit: `36e31eb71c98`
- `CDesk.cs` `9f036c7e380e9b97`
- `CDeskChronicle.cs` `fc6b9a85af7d4b71`
- `CDeskDraft.cs` `a573193758b55776`
- `CEditor.cs` `efc620f117523040`
- `CEntry.cs` `b002c6eb80602df8`
- `CErrand.cs` `8f73391eb2be162b`
- `CEsteem.cs` `b08f4e56adee1280`
- `CSession.cs` `b50233150aac7538`
- `CVigil.cs` `d5d70700eb8084d2`
