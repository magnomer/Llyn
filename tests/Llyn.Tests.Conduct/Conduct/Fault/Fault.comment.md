# Fault

This folder holds the fault sweep proving no engine fault escapes a Conduct gate.
It also holds the sweep's rows and stages and the reading view's failure routes.

## `TDisplayFailure`

Covers the failure routes of the reading view's reads, on a real workspace with fake ports.
The faked ports answer only the entry load, the collocation layout and a few housekeeping calls.
So each read meets a refusal and shows its key through the fake envoy.
The layout answer passes on the link targets the display hands in, and reaches no store.
Those targets are keyed by card kind and id.
The tests call reads rather than drawing WPF controls.
A failed stamp read shows `Display.StampFailed`.
A failed sentence order read shows `Display.OrderFailed`.
A failed Source line read shows `Display.CitationFailed`.
A failed reflex scan shows `Display.ReflexFailed` and answers no rows.
A failed rime book reading shows `Display.ReadingFailed`.
A failed paradigm language read shows `Display.LanguageFailed`.
A failed sound start shows `Sound.StartFailed` through the voice's mark failure event.
A failed paradigm row read shows `Display.ParadigmReadFailed` through the voice's failure event and answers no slots.
A failed waiting check shows `Sound.PendingFailed` through the voice's failure event.
A failed anchor read leaves the fanqie read answering no groups.
A subsequent direct reflex read shows `Display.AnchorFailed`.
A failed favorite read shows `Favorite.ReadFailed` and answers false.
A favorite read that fails on two repaints shows `Favorite.ReadFailed` once.
Two card reads over a failing order port show `Display.OrderFailed` once.
Two card reads over a fold port the fake does not answer show `Fold.ReadFailed` once.
That assertion covers notice deduplication, not the returned cards' fold values.
A favorite mark the user retries shows `Favorite.MarkFailed` each time, since it is a user act.
A failed grasp read shows `Grasp.ReadFailed` and answers zero.
A failed frequency read shows `Frequency.ReadFailed` and answers no chip.

## `TFault`

The fault sweep proves that no engine fault escapes a Conduct gate returning a task.
Each gate must catch the fault and show it once through the envoy, as `CDisplaySound` does.
One row per gate checks the promise everywhere, not only where a hole was found.
Gates that write without a task are swept apart in `TFaultWrite`.

## `TFaultFlagged`

A pack whose one variety shows a flag by code, so the flag read reaches the language vault.
A rooted flag path would answer from disk and never reach the faulted vault member.

## `TFaultRows`

One row per gate, named `Type.Method` so the completeness fact can match it by reflection.
Port gates fault their port member over `TInterfaceConduct.TAtelierFaultCreate`.
Errand and timbre gates reach the engine through a tenure, not a port.
Their rows therefore fault the vault member the tenure reaches, over the real rig.
Panel list-load rows live in `TFaultList.TFaultListRows` and join the list before the portrait rows.
Courier rows fault the portrait port's courier members over a courier built by `CCourier.CCourierCreate`.
The depot export row faults `LDeckPort.LEngineDeckExport` and expects `Deck.ExportFailed`.
Portrait print and export rows live in `TFaultPortrait` and join the list last.

## `TFaultGates`

The gate names of every row, so each theory runs once per gate under a readable name.
One theory runs a row with its member answering a faulted task, and another with it throwing before any task.

## `EnvoyFailureShow_EveryTaskGate_HoldsASweepRow`

Every public method returning a task on a public Conduct type must hold a row.
A new gate therefore fails the sweep until it is given a row.

## `TFaultRun`

Arranges the row, then clears what the envoy heard so only the gate's own notices count.
The gate call and the await of its task must both raise nothing.
The envoy must then have heard exactly the row's key, once.

## `TFaultEngineStart`

Starts a real engine on a prepared workspace, both disposed with the stage.

## `TFaultAtelierStart`

Starts a real engine and the atelier over its faulted outlets, both disposed with the stage.
Panel rows in `TFaultList` and the other sweep files build their panel on it, so only the stage's member faults.

## `TFaultEditorStart`

Starts an editor over an engine whose language and recording vaults forward to the real rig.
Only the stage's member faults, so the draft and its search run as in the app.
The engine is built straight from that rig, and its tenure delay is cleared so the run ends quickly.

## `TFaultRecordingStart`

Starts the recording search and waits until it ends, so the errand holds its foray.

## `TFaultList`

The list-load half of the task fault sweep, holding one row per panel row load.
The rows join `TFault.TFaultRows` there, so the sweep runs them beside every other gate.
`TFaultListRows` is a property, not a field, so the sweep's list reads it whatever the type initialisation order.
Panel row loads all await the settings port's flag load before they read their rows.
Their rows therefore fault `LSettingsPort.LEngineEnsignLoad` and expect the panel's own load-failed key.
Footnote, occurrence and quotation lists are reached through the shelf, repertoire and corpus that own them.
Each panel is built on `TFault.TFaultAtelierStart`, so only the stage's member faults.

## TFaultMark

The second half of the synchronous fault sweep, holding the mark, sound, fold, creation and merge gates.
It keeps `TFaultWrite` within the line limit, and the rows join the sweep there.
`TFaultMarkRows` is a property, so the sweep's list reads it whatever the field order across files.
Port rows fault the port member the gate itself calls, over `TInterfaceConduct.TAtelierFaultCreate`.
Favorite and grasp rows read back the gate's own read, since it answers the written mark.
Card fold rows fault `LCardPort.LEngineFoldSave` and read back the first meaning's fold from a fresh card read.
The gates name the card by kind and id, so the rows pass the meaning kind.
They read the gate's verdict beside the fold, so a refused write must also answer false.
The reading view row reads the leaf, and the editor row reads the held card draft.
Tag and register creation read back the panel's row count, which a created row would raise.
The union select reads back whether the autograph shows, since a merge would close it.
Box fold rows fault `LReflexPort.LEngineBoxSpread` on a held stored entry.
Their read-back is the box's stored state beside the gate's verdict, so a refused write must answer false.
Fanqie set, rebuild and sound start rows read nothing back, since no gate read answers the engine work.
Sound start is reached through the wing open, the display resonate, and the panel open and resonate.
Other gates reach it only through these, so they hold no row of their own.
The display resonate and panel resonate rows open the entry first, so a chosen entry exists to reload.
Esteem, sounding, reflex and box gates act only on a stored entry, so a fresh draft would reach no port.
The consent envoy answers every confirmation yes and the coinage question with a given wording.
It records only failure keys, so the sweep sees the gate's notice alone.

## TFaultPortrait

The portrait half of the fault sweep, holding one row per print or export gate.
It keeps `TFault` within the line limit, and the rows join the sweep there.
`TFaultPortraitRows` is a property, so the sweep's list reads it whatever the field order across files.
Print gates fault `LPortraitPort.LEnginePressOpen` and expect `Print.Failed`.
Export gates fault `LPortraitPort.LEnginePortraitExport` and expect `Export.Failed`.
Each faults the port member its gate awaits inside `CPortrait`, after any envoy question.
A gate that prints only a chosen row is first given one, or it would return before the port.
Corpus and repertoire prints take the quotation or occurrence on display.
Shelf print and export take the footnote entry.
Guild print takes a source of the chosen author.
Xiesheng and yunjing prints need a language pack and a chosen character row.
Their exports, and the favorite, library and phonology gates, need nothing chosen.
The library entry export is given a stored entry, since it selects the row by id before it exports.
The print window's own gates run inside a session opened by `TFaultPressOpen`.
Its system dialog and print fault the board and expect `Print.Failed`.
Its draw faults the board's draw and expects `Print.RenderFailed`.
The sight row draws a proof first and sets the galley's edit and sight delays to zero.
Its page requests then fault the proof's draw and expect `Print.RenderFailed`.
The sight pass is the only page draw, so no other row faults the proof's draw.
The restore row faults the printer listing through the fake queue's own failure.
The fault double cannot reach the queue, so the thrown and faulted runs fault it alike.
The envoy answers the file question unrecorded and records only failure keys.
The faulted port never writes, so the answered file name is never created.

## `TFaultPressOpen`

Opens a print window over a fake board wrapped in the fault double, and runs the gate in it.
The board also wraps each proof it hands out, so a proof member can fault too.
The returned call opens the session, so the stage's heard list starts empty before it.
Every ledger key the session shows lands in that list.

## `TFaultAtelierCreate`

Builds the atelier over a given engine's faulted outlets, disposed with the stage.
Rows that save their own rows first need the engine in hand.

## `TFaultTenorStart`

The tenor, taxonomy, corpus, repertoire and shelf starters each open one object on display.
That is an entry in a register cohort or tag membership, or a quoting entry.
It may also be an occurrence or a selected footnote entry.
So the print and export reach the port instead of returning at their guard.

## `TFaultRow`

One gate of the task sweep, with the fault it meets and the notice it must show.
It names the gate as `Type.Method` and the faulted member as `Interface.Member`.
It carries the notice key the envoy must hear once.
Its arrange step builds the gate's owner, meets its preconditions, and answers the gate call.

## `TFaultStage`

Carries the faulted member, the fault mode and the heard notices into a row's arrangement.
It disposes what the arrangement held in reverse order, the engine before its workspace.
The fault mode says whether the member throws before it answers any task or answers a faulted task.
The heard list holds the notice keys the envoy heard, which the sweep compares with the row's key.
Adding a disposable holds it for the stage's end and answers it, so arrangements build and hold in one step.

## `TFaultWrite`

The synchronous half of the fault sweep, holding one row per gate that writes.
A gate that writes must show the fault once and leave the state it reads back unchanged.
A gate that only records its fault must raise nothing and show nothing.
A settings change once landed in memory before its write failed, so the read-back guards that hole.
Rows are named `Type.Method` like the task rows.
The two workspace change overloads carry their parameter types, so each runs under its own name.
Port rows fault their port member over `TInterfaceConduct.TAtelierFaultCreate`.
Settings rows fault `LSettingsVault.LSettingsSave`, so the engine's own rollback is under test.
Their read-back is the settings field the gate writes, read from the engine.
A switch row saves the opposite of the held value, since an unchanged snapshot never reaches the vault.
The tab select faults `LPostureVault.LPostureSave` and shows `Layout.SaveFailed`.
The volume set faults `LMediaPort.LEngineVolumeSet` and shows `Sound.VolumeFailed` through the workspace's held envoy.
Neither reads back, since each failure keeps the state the screen shows and tells the user once.
Volume, tab select and close open the atelier first, so the workspace holds an envoy.
The close row keeps its atelier off the stage, since a second close would fault again.
The close row expects no key, since the window is closing and the fault is only recorded.
The entry, display, panel and sound rows join from `TFaultMark`, keeping this file within the line limit.

## `TFaultWriteRun`

Arranges the row, reads the state back, then clears what the envoy heard.
The gate call must raise nothing, and the envoy must have heard exactly the row's key, once.
A row with no key expects the envoy to have heard nothing at all.
A row with a read-back must read the same value after the failed call as before it.

## `TFaultVaultStart`

Starts an engine on a prepared workspace whose settings and posture vaults forward to the real ones.
The vault member the stage names throws a vault fault, as the real vaults wrap their failures.
A first engine stores a posture beforehand, so the faulted engine's posture load saves nothing.
Only the gate's own save then meets the fault, not a save made at load.

## `TFaultFolderCreate`

An envoy that answers the workspace folder question with a chosen path and records failure keys.
The workspace change without a path asks it before it reaches the port.

## `TFaultWriteArrange`

Each record of `TFaultWrite` is one gate of the synchronous sweep.
It names the fault the gate meets and the notice it must show.
Its key is null for a gate that only records its fault.
This arrange step answers the gate call and an optional read-back of the state the gate writes.

## Stamp

Commit: `36e31eb71c98`
- `TDisplayFailure.cs` `f0c7be12f4b663f4`
- `TFault.cs` `fa397c15b685eb43`
- `TFaultList.cs` `e993e6b5885af5a2`
- `TFaultMark.cs` `49690789b8e6973c`
- `TFaultPortrait.cs` `548737d4700eceaf`
- `TFaultRow.cs` `94662f8cc5a1e773`
- `TFaultStage.cs` `2894647223e0a2fc`
- `TFaultWrite.cs` `2689e72430601ed8`
