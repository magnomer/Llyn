# Atelier

This folder holds the relays that build Conduct's atelier and its areas.
They cover card, desk, dialog, panel, portrait, sound, editor, navigation and area restore.

## `TInterfaceArea`

The relays for each panel area's own vista restore.
An area restores its vistas when it is built, and a workspace change runs the restore again.
A test calls a relay to stand in for that change, without moving the workspace.
Each relay is transparent.

## `TInterfaceConduct`

The relays that build Conduct's atelier and reach its workspace and voyage.
It also keeps the media and settings fakes the other conduct relay classes hand on.
Each relay reaches a conduct member or builds one over outlets, so none builds a WPF object.

## `TAtelierCreate`

The plain overload builds the atelier over real outlets on the engine.
Its detached flag defaults to false, and true builds an atelier that reads and stores no layout.
Its stub player accepts a stop but never plays.
The stub resolves every media location to no address and no screen, so a scenario's media rows still read.
A workspace engine is needed, since disposing sweeps drafts through the real draft outlet.
Disposing also runs each area's registered close, and the corpus's close stops its display's playback.
The media overload builds the atelier with the given media port standing in for the player.
Its draft port is a fake that sweeps nothing, since the fake rig holds no drafts.
`TAtelierMediaCreate` builds it as Host does, over a real draft outlet and the given media port.
The settings overload builds an atelier over a fake settings port, for the ledger's reads and saves.
Its draft port answers the sweep and the observer attach and detach.
The ledger can thus attach through a fake draft port.
The draft port overload lets a fact hold the bulletin observers the port is handed and send a bulletin itself.
The answers overload is for the text gates.
Its draft, bundled entry and bundled sound ports answer from the map.
The settings and portrait ports are outlets on the engine, and the media port answers from the map too.
It adds the leftover sweep and the recording stop, so disposing the atelier needs no answer from the test.
It also relays the side save to the real engine, so a stored side survives a fake vista port.

## `TAtelierVistaStart`

Starts a tab's vista on the entry subject with the given fallback order.
It spares a layout fact from naming the subject.

## `TAtelierFaultCreate`

Builds the atelier over every real port on the engine, each wrapped by `TEngineFault` on `member`.
Only the port declaring `member` faults it, so the fault sweep reaches the real engine everywhere else.
`thrown` picks a throw over a faulted task, as the sweep runs both.

## `TEntryBundleCreate`

The plain overload builds the bundle Host builds, every port the facade on the engine that owns it.
The deck port is the one exception, an `LDeckOutlet` over the engine, as Host builds it.
  The deck facade is internal to the engine, so the outlet relays to it.
The swap overload puts the given fake in place of every port it implements.
A fake made for one port thus stands in for that port alone.
The fault overload wraps every facade port with `TEngineFault`, and only the port declaring the member faults it.
So a fault key names the narrow port.
The answers overload makes every port a fake answering from the map.
So an empty map gives a bundle of bare stubs.

## `TPhonologyBundleCreate`

The engine overloads mirror the entry bundle, over the sound facade ports.
The answers overload makes every sound port a fake answering from the map.
So one map answers each port a fact reaches.
The scale, the cell kind and a paradigm row's text and tip follow the engine's own rule.
A fact overrides any of them by naming it in the map.
Those were rules with no engine state, so a fake keeps them true without a test naming them.
The inflection view answers null unless the map names it, so a fact sees the list alone as before.

## `TMediaCreate`

The engine overload builds the real media outlet in place of the fake port.
The bare overload is a port that only resolves locations and screens, each to nothing.
The card and atlas map relays hand it on, so a fact without an engine still reads its rows.

## `TSettingsCreate`

A settings port that answers only failure notices, text keys and fonts.
A notice reads as the unexpected key it is handed, and a text key as the key itself.
A test thus sees which wording a gate chose.
A font reads as one with nothing set, as for a language whose pack sets none.
The font rule catches no refusal, so without that answer a font read would throw.
A gate over fakes can then show its failure without a real engine behind the notice.

## `TLedgerPortCreate`

A fake settings port that answers the ledger's reads for a Korean localization, over any answers the test sets.
It also answers an empty workspace status and an empty establishment read, so an open shows no failure.
It answers custom analysis on, so the Inflection page summary reads without a test answer.
The ledger facts and the ledger notice facts both build their atelier on it.

## `TLedgerShowRead`

Attaches a view that keeps every state it is shown, opens the atelier through `TAtelierStubOpen`, and answers that list.

## `TAtelierStateOpen`

Opens the atelier through `TAtelierStubOpen` and returns the workspace state it announced, or nothing when none came.
The subscription is removed again before it returns.

## `TAtelierStubOpen`

Opens the atelier with a stub envoy and a marshal that runs at once.
So a test opens it without a window.
The stub answers every call with nothing.

## `TAtelierOpen`

Opens the atelier with the given envoy and a marshal that runs at once.
So a test reads the notices the open shows, a layout failure among them.
A test that opens twice hands a second envoy, and sees which one later notices reach.

## `TLedgerFailureShow`

Relays a gate's failure policy over the atelier's own settings port, so a test can show a chosen key twice.
The repaint relay sends a repaint read's failure through the atelier's repaint memory, so a test can watch repeats merge.

## `TWorkspaceDraftAdd`

Hands the workspace a draft's pending question and its closing step.
A test can then see how the workspace asks and closes drafts.

## `TInterfaceConductCard`

The relays for Conduct's card area.
They build the card, card list and sentence gates over a desk and reach the folio's internal maps.
Each relay is transparent.

## `TCardCreate`

Builds the card gates over the desk, the engine's reference facade and a real settings outlet, as the editor does.
The envoy is where the card gates show their failures, such as a failed translation.
It gives the card a fresh repaint memory, as the atelier would.

## `TCardListCreate`

Builds the card list gates over the desk, as the editor does once.
Its stub card port supplies no real fold persistence.

## `TSentenceCreate`

Builds the sentence gates over the desk with the engine's vocabulary facade and a real draft outlet.
Its settings port is the shared fake, and its envoy answers no to every question.
It holds a fresh `CLedgerNoticed` of its own.

## `TCardImageRead`

Relays the internal image map, so a fact can hand it a row nobody located.
A fact hands its own media port, so it can watch what the map asks and what the address becomes.

## `TCardEntryRead`

Relays the entry draft map, so the facts pass engine drafts through the boundary.
It hands no folded ids, so every card maps unfolded.
It hands no link menu, so no card carries meaning links.

## `TInterfaceConductDesk`

The relays for Conduct's desk area, its session and its errand.
They build a desk or a session and reach the desk's starts, its tenure and the errand's maps.
They read the held draft's sentence ids and build the editor's marks over a grasp port a fact fakes.
They build the editor and run the recording search that the errand facts share.
Each relay is transparent.

## `TDeskCreate`

Builds a desk over real draft and settings outlets on the engine, as the Conduct areas that own one do.
The long overload builds it under its own origin and subject, as the corpus and the playwright build theirs.
The start relays begin a fresh entry already linked to something.
The link is a Situation, an Example, a Reference, a Tag or a Register.
Each stands in for the matching new-record action of a panel.

## `TDeskVistaRestore`

Relays the restore of a saved vista into the desk, which only the navigation runs in production.

## `TDeskDefer`

Hands the held tenure a request to defer, and does nothing when no tenure is held.
The variety, foray and held-draft relays likewise do nothing or answer nothing when no tenure is held.

## `TDeskSentenceRead`

The ids of every sentence row on the held draft's meaning and collocation cards, in ascending order.
It answers none with no draft held, so a fact names no engine draft member itself.

## `TEsteemCreate`

Builds the editor's marks over a real desk holding the stored entry, as the editor does.
The desk is an Input desk bound to a fresh library vista, over a real draft outlet.
Its display rules ask the grasp port, so a fake lets a fact answer the grasp reads with hostile values.
The other display ports are the engine's own facades.
Its envoy answers no and records nothing, and its media port is a bare stub.

## `TErrandClipStart`

Opens a new entry with a headword in the language, and waits until its recording search ended.
A pack without sources ends the search at once, so later steps are the test's own.
The clip facts and the errand fault facts both start their errand here.

## `TErrandClipPrepare`

Builds an editor over the engine whose notices nobody reads.
Both overloads restore the input tab's vista, ordered by headword.
The overload with a list adds each notice the editor shows to it.
It answers the fixture, so a test reads only the entry and desk facets.

## `TSessionCreate`

Builds a session over the desk alone, with no editor, as the guild does.
The overload over an editor records each editor finish in `seen`, as `Finish` or `Drop`, and answers yes.
Both hand a fake envoy that answers false, since no session test asks the leave question.

## `TInterfaceConductDialog`

The relays for Conduct's dialog rules.
They build the markup import's customs sheet and reach its rows.
They reach the coinage wording rule and every gate of the print window.
No relay builds a WPF window, so the rules are tested without a dialog or a printer.
Each relay is transparent.

## `TCustomsModeSet`

Relays the customs sheet's mode change of one row.
It answers whether the whole sheet is then ready, and the target change does the same.
A sheet is ready when every row not imported fresh has a target.

## `TPressSettingRead`

Relays the print gate's read of the current setting.
Most press relays are named `TPress` plus the gate's words after `CSPress` or `CSGalley`.
The lens relays are named `TLens` plus the words after `CSLens`.
The delay, dispose and attach relays are the exceptions.
The restore lists the printers first.

## `TPressPrinterSelect`

Finds the place of the printer named in the current setting and relays that index.
A null name finds Save as PDF.
An unlisted name finds -1, which the gate ignores.
Tests name printers, while the window hands the picker's raw index.
The index overload relays the raw index as the window hands it.

## `TPressPageSelect`

Relays the selection of a proof page by index through the internal helper `LSPressPageSelect`.
The helper answers nothing, so the relay reads the setting after it.
The sight gate reaches the same helper, and the relay skips that gate's wait.

## `TPressDraw`

Relays the proof draw of the press's galley, which answers null when a newer draw supersedes it.

## `TPressSightSet`

Relays the sight a driver hands over after a scroll, with the page, visible pages and monitor scale.
The task ends once the deferred page requests are answered, so a test awaits it.

## `TPressDelaySet`

Sets the galley's wait after an edit and after a sight to the same delay in milliseconds.
It sets the time of the two delays apart, since each wait has its own.
Zero runs a deferred draw at once, and `Timeout.Infinite` keeps it pending until a newer request or close.

## `TPressGalleyDispose`

Closes the galley as the print core does once the window closes.
So a test checks that no event follows.

## `TPressSystemShow`

Relays the request for the system print dialog, answering a failure key or null.

## `TPressPrint`

Relays the print gate, which prints, or saves a PDF after asking the envoy for a file.
It answers a failure key, or null on success.

## `TPressClose`

Relays the one closing gate with no earlier refusal, as a plain window close hands it.
It answers false while a print runs, and keeps the setting when it answers true.

## `TPressSettingAttach`

Subscribes to the gate's setting event.
So a test sees the setting a print reports at its start and its end.
The state attach hears the galley's state event, raised when a deferred redraw finishes.
The page attach hears the pages a sight delivers with a picture, and passes on no drop.
So a fact counting deliveries sees only deliveries.
The drop attach hears the pages raised without a picture.
Those are the delivered pages the galley stops holding, handed on as their index.

## `TInterfaceConductPanel`

The relays for Conduct's panel area and the catalog maps its panels read.
They reach the panel, the oeuvre, the occurrence list, the atlas, the example list, the footnote and the imprint.
They also reach each panel's offered file name, its leave question and its startup editor restore.
Each relay is transparent.

## `TCatalogArticulationRead`

Builds an IPA chart from the headers, sides and cells and relays the catalog's map of it.
A fact thus feeds a hostile chart without building the engine record itself.

## `TPanelCreate`

Builds a bare panel over the fake settings port, with the given delete scope and change and finish seams.
The engine's vista facade is its vista port, so a restored vista loads and deletes for real.
Its load-failed key is fixed and its last seam always answers yes.
A test thus varies only the seams it passes.

## `TPanelVistaRestore`

Relays the restore of a saved vista into the panel, which only the navigation runs in production.
The oeuvre, occurrence, atlas, anthology and footnote restores bind their lists through the internal helpers the owning area calls.
The chosen read, station attach and scribe restore are likewise run only by the navigation in production.

## `TPanelVistaRead`

Reads the vista the panel's aperture currently holds, or nothing when it holds none.

## `TPanelOrderRead`

Relays the catalog's map of a catalog order, in both directions.
A missing order stays missing.

## `TAtlasRowsRead`

Reads the atlas rows through its internal helper, without the repertoire's stale-selection close.
The fail read uses a situation port whose situation find throws, and answers what the read answers.

## `TAtlasDraftRead`

Wraps the Situation in a bare draft and maps it through the atlas's internal helper.
A null Situation reads as no draft at all.
The situation read maps it to the vignette's ready page.
A markdown port runs the real parser, so the description's blocks are real.

## `TAnthologyCreate`

Builds the example list through its internal factory, always shown and always finishing.
The rows read goes through the internal helper, without the corpus's stale-selection close.
A failed read throws, so a test never mistakes a failure for an empty list.

## `TAnthologyTextCheck`

Asks the anthology's internal text match whether a field showing the text already shows the value.

## `TFavoriteFileRead`

Relays a panel's internal offered file name, as the relays below do for the other panels.
The xiesheng panel reads its name through its kindred list, and the yunjing panel through its xiaoyun list.

## `TShelfDraftFinish`

Relays the sources tab's finish of its draft, stored or dropped as asked.
It answers whether the finish went through.
The leave and change relays of the shelf, repertoire and corpus relay the questions the navigation and the session ask.

## `TCorpusTranscriptRead`

Relays the transcript's read of the Example the corpus desk holds, through `CCorpusTranscript`.
`CTranscriptDraftChanged` hands that read on, marked kept when the field already shows its text.
`CCorpusTranscriptChanged` hands it on marked shown, or hands the blank Example.

## `TCorpusEntryResonate`

Relays the corpus diptych's answer to the chosen entry's notice, which the quotation observers run.

## `TInterfaceConductPortrait`

The relays for Conduct's portrait maps and its shared export and print cores.
They are apart from `TInterfaceEnginePortrait`, which relays the engine's side of a likeness.
Each relay is transparent.
The print openers alone compose a workspace, so Conduct facts reach a real print session.

## `TPortraitFileExport`

Relays the shared export core with its question and failure policy.
The fixed file is handed on as the core's name read.
The settings port is the shared fake from `TInterfaceConduct`.
The long overload takes a given settings port and name read.
A fact can thus fail the name read or pass no settings.

## `TPortraitPrint`

The first overload relays the shared print core with its failure policy over the shared fake settings port.
The board overload opens a print session through the route a panel's print gate takes.
A fresh workspace engine saves the entry `water` and chooses it in the library vista.
Its fake press answers the board, and its fake queue answers the two fixed printers.
A failure makes the fake queue raise it instead.
A vault replaces the workspace's own setting store, so two sessions can share one.
The print core then opens the session through `LPortraitPort.LEnginePressOpen` with the settings label.
So a Conduct fact never builds the engine's print session itself.
The workspace and engine live until the window closes and the core disposes the board.
The suggested file name is `water`, the entry's headword.

## `TPressWindowOpen`

Opens a print session over the board and runs `check` against its gate.
The board may be a fault double, so the fault sweep reaches a gate through this route.
The envoy callback first sets the galley's edit and sight delays to `Timeout.Infinite`.
So no deferred draw fires on its own, and a fact that tests the deferral sets the delays to zero.
The callback then restores the kept setting, as the driver does when the window shows.
That restore lists the printers, so `check` sees them as a reader would.
A failure makes the listing raise it, so `asked` records the ledger key `Print.Failed`.
A vault handed to two calls carries a setting from the first window to the second.
The envoy callback runs `check` on the thread pool and waits for it.
So a fact can hold a board task open and release it without a deadlock.
A null path declines the Save as PDF file question.
Every question the envoy hears lands in `asked`.

## `TInterfaceConductSound`

The relays for Conduct's sound area and the display's sound section.
They reach the reflex lead rule, the display sound and its flag load.
They reach the display's favorite, grasp and frequency reads, so a fact can drive their failure routes.
One builds a display over a port bundle a fact fakes in part, with an entry chosen.
They also reach the sound sheet, the entry-stored box folds, the respelling rule and the sound facts.
Each relay is transparent.

## `TDisplayChosenCreate`

Builds a display over the entry bundle and real outlets on the engine, bound to a fresh library vista.
The vista chooses the given entry, so the display's own reads ask the bundle about that entry.
A fake port in the bundle lets a fact answer the grasp and frequency reads with hostile values.
A given phonology bundle replaces the real sound ports, so a fact can fault one.
Its envoy answers no and records failures in the supplied list, or a discarded one.
Its media port is a bare stub.

## `TSoundingCreate`

Builds the editor's sound sheet over a real desk and ports a test may fake.
The fake ports let a test refuse an engine call and watch the envoy.
The settings port is `pack`, or else `TInterfaceConduct.TSettingsCreate`, so a refusal reaches the envoy with a notice.
The waiting checks go through a display over the same phonology bundle, with a fresh repaint memory.

## `TContourRead`

Relays the contour map over a given scale, so a fact feeds levels outside it.

## `TFoldCreate`

Builds the editor's box folds.
They run over the stored entry of the editor's desk and the reflex port of the phonology bundle.
A fault bundle whose box write throws lets a test watch the failure reach the envoy.
The envoy notes every failure in `asked`.
It holds its own repaint memory, so a refused read shows once per fold.

## `TKindredCreate`

Builds the editor's reflex block over the desk and display facets of the editor, with ports a test may fake.
The fake draft port lets a test answer the anchor rule, and the fake bundle's reflex port the reflex guises.
Its settings port is `TInterfaceConduct.TSettingsCreate`, so a refused anchor read reaches the envoy with a notice.
It gives the block a fresh repaint memory, as the atelier would.
Its envoy answers no and uses a discarded list when `asked` is absent.

## `TRespellingReflexScan`

Relays the shared reflex scan's rows, so a fact can hand it a fake reflex port and stored rows.

## `TInterfaceEditor`

The relays that build an editor gate over outlets or fakes and reach its internal members.
It is a class of its own, apart from `TInterfaceConduct` and its sibling conduct relay classes.
Each relay is transparent.
An editor built here gets a fresh repaint memory, since no atelier stands behind it.

## `TEditorCreate`

The plain overload builds the editor over every real outlet on the engine, with a fake envoy that answers no.
Its marshal runs each notice at once.
The marshal overload lets a test choose how the editor's notices reach it, such as holding them back.
The envoy overload lets a test read each notice the editor shows.
The bundle overload also takes a given entry bundle.
So a test fakes an entry port and still reads the notices.
The phonology overload fakes the sound bundle alone, and its media port is a bare stub.
The port overloads build the editor over given ports, so a test fakes any one of them.
The fixture overloads of the vista restore and the finish relay into the editor a fixture wraps.
A test holding only the fixture thus needs not name the editor.

## `TEditorDraftRead`

The held draft of the editor mapped through the entry area's own read, or null without one.
So each card carries the fold the store holds, as `CEntryDraftChanged` hands it.
Tests inspect the mapped draft without subscribing a driver.

## `TEditorVistaRestore`

Relays vista restoration without requiring a navigation driver.

## `TInterfaceNavigation`

The relays for the navigation's internal rules and the arrival opens it runs on each area.
Each relay is transparent.
The tab add registers a tab as a panel area does, and the landed record goes to `arrival`.
The diwei and stem relays stand in for the display and sounding callers, which are the only production askers.
The arrival opens relay a panel's open as the navigation runs it.

## Stamp

Commit: `52c21d68b4c4`
- `TInterfaceArea.cs` `bf5825a6e7d0f03b`
- `TInterfaceConduct.cs` `f849d43f7cf1ffa9`
- `TInterfaceConductCard.cs` `678838c33733ba58`
- `TInterfaceConductDesk.cs` `85c44229ca35484b`
- `TInterfaceConductDialog.cs` `3c2d58ee2694a179`
- `TInterfaceConductPanel.cs` `c7d7de34ba294c4c`
- `TInterfaceConductPortrait.cs` `c0b5da50cc1382f2`
- `TInterfaceConductSound.cs` `c5ce10ee393a1d21`
- `TInterfaceEditor.cs` `707ba5799d8fcb9d`
- `TInterfaceNavigation.cs` `860a179cae6ebe17`
