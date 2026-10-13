# Atelier

Atelier holds Conduct's root and the session-wide machinery no single panel owns.
It holds the root session, the ports handed in by Host and the workspace life cycle.
Navigation and its voyage trail live in the child folder.
It stands apart from the panel areas, which each own one tab.
Everything here is medium-free, and every driver reaches it through the same gates.

## `CAtelier`

The working session over one workspace, which Host builds and hands to a driver.
It holds the engine ports, the entry bundle and the session's posture.
Its gates are the medium-free actions no single panel owns.
Those are opening, quitting and closing the session, bulletins, vista start, volume and split.
GUI-only state such as window geometry and panel widths never reaches it.

The constructor is internal, so no driver can build a root of its own.
It takes the marshal Host builds on the UI thread, a construction dependency like a port.
Every editor the atelier or a panel area builds takes that marshal, so no driver attaches one.

The ports and the posture are internal, because each is an engine handle no driver may hold.
The settings port also words every key and reads every failure notice.

`CAtelierCatalog` holds no state, so each read builds a fresh one.
The mention, navigation, ledger and workspace are built once.
The ledger raises `CLedgerChanged` to subscribed views, so it must be one object.

`CAtelierInputCreate` builds the input tab's editor and binds it to the input vista at once.
The input tab has no area of its own.
It registers the editor's change check, finish and vista restore with the workspace, like a panel area.
So the workspace holds no editor, and the quit and a workspace change reach it as any area.
`LAtelierInputRestore` runs again after a workspace change, so the input tab follows the new workspace.

`CAtelierVistaStart` starts the tab's vista on its stored order, filter and split.
The driver names the subject and order in Conduct's enums, so it never names a Core one.
They reach the engine through `CCatalog`'s by-name maps, never by cast.

`LAtelierSplitRead` tells whether the open tab shows its editor rather than its read area.
Only the navigation reads it, when it restores the open tab on every open.

### Volume

`CAtelierVolumeRead` answers the session's one audio level, between zero and one.
`LAtelierVolumeClamp` is the one rule for both read and set.
A level outside the range is pulled to the nearest end, and infinite levels fall the same way.
A level that is not a number reads as full, the posture's default.
So every slider and player can take it unchecked.

`CAtelierVolumeSet` clamps first, so the player never hears an unchecked level.
The posture keeps that same level, so a read after a set answers what the player was given.
The level is written only when `settled`, so a drag costs no write per step.
A driver passes `settled` once the gesture ends, and a key press is settled at once.
A player fault goes to `CWorkspace.LWorkspaceFailureShow` as `Sound.VolumeFailed`.
A settled set still writes the level after a player fault, so the saved level matches what is shown.
A drag fires many sets, so the notice shows once per session and later faults are only recorded.

### Open and close

`CAtelierOpen` opens the session on the workspace in use at startup.
The window's envoy is passed down, so a failed status read is shown rather than dropped.
The window's marshal is passed too, and the layout save failure is heard through it before the open.
So the driver wires that notice and opens with one gate call.

One engine call sweeps the leftover drafts and recordings and answers the stored state.
The sweep runs before any view restores, so nothing already saved is counted as lost work.
A fault there shows `Workspace.OpenFailed` through `CLedger.LLedgerFailureShow` and opens nothing.
That leaves the atelier as a failed `CWorkspaceChange` leaves it, with no open event raised.
Otherwise `CAtelierWorkspace` raises its open events, and the stored tab opens last.

`CAtelierQuitConfirm` decides whether the session may end over every area that holds a draft.
The window's closing reaches it, and `CWorkspaceChange` asks the same workspace quit inside Conduct.

`CAtelierClose` ends the session once the window has closed and every view has stopped.
It runs each area's close registered with the workspace, through `CWorkspace.LWorkspaceClose`.
It then sweeps the leftover drafts once more, then releases the posture, which lets go of every vista it watched.
Sweeping on the way out as well as on the way in bounds what a long session leaves behind.
The window is closing and no envoy stands, so a close or sweep fault is recorded and never shown.
It is recorded through `CLedger.LLedgerNoticeRead`, whose engine read writes the fault record.
A close fault is caught on its own, so the sweep still runs after it.
The posture is released in a `finally`, so neither fault leaves its vistas watched.
`Dispose` closes through `CAtelierClose`, for a host or a test that holds the atelier in a `using`.

### Static wording reads

The three static reads answer wording keys for notices shown where no atelier stands.
- `CAtelierAboutRead` is static since every session words the notice alike.
  The build number stays with the driver, because Conduct is framed away from reflection.
- `CAtelierRefusalRead` says so when another program holds the database, since closing that program is the cure.
  It is static since no atelier stands when the workspace fails to open.
- `CAtelierRescueRead` answers null when no database was set aside.
  Without the notice the workspace would appear empty and the old data would look lost.
  It is static since the host reads the rescue before it builds the atelier.

### Bulletins

`LAtelierObserverAttach` hands the observer only the bulletins about its subject.
The Conduct subject is mapped by name to the engine's, and the compare is between engine values.
The ledger and the courier hear through it.
`LAtelierObserverAdd` hands every engine bulletin to the workspace's status.
`CAtelierBulletinRead` is the one map from an engine bulletin to Conduct's.

## `CEntryBundle`

The entry ports Host hands to Conduct in one piece, one port per narrow slice of the engine.
Each facade implements its own port, so no forwarding outlet stands between Conduct and the engine.
An area builder reads only the ports its constructors call and hands each one down.
It is internal, so Host and Conduct reach it and no Deportment driver can name it.
The constructor rejects a missing port, and only Host builds a bundle.

## `CPhonologyBundle`

The sound and script ports Host hands to Conduct, built and guarded like `CEntryBundle`.
It carries the rime-book, diwei, script, language, reflex, paradigm, sentence and stem ports.
The pronunciation areas read through it.

## `CEnvoy`

The port through which Conduct asks the user.
Each driver implements it in its own medium, so a gate never shows a dialog itself.
Conduct chooses the text key, and the driver looks up its wording.

`CEnvoyConfirm` with a tally names the label of the count, so the driver words both from its own catalog.
`CEnvoyUnionConfirm` shows the dropped and kept names as given in the fold's direction.
`CEnvoyFailureShow` with a notice carries the detail Conduct read once.
Conduct recorded any fault already, so the driver only words the keys it carries.
`CEnvoyLeaveConfirm` answers true to store, false to discard, and null when the user stays.

`CEnvoyMarkupRead` is the first question of the library import and answers null for a cancelled pick.
`CEnvoyCustomsRead` asks how each parsed entry of a markup file enters, one row per entry in file order.
A decline stores nothing.
`CEnvoyPressRead` puts up the print window and returns when it closes.
The window prints or saves through the gate's own calls, so nothing is handed back.
Conduct disposes the session either way.
`CEnvoyOmissionShow` lists what an import could not place, and the library asks it only when something was left behind.

`CEnvoyFileRead` answers no file when the user declines, and the gate then exports nothing.
The gate chooses the offered name and the formats, so the question needs no read of its own.
`CEnvoyWorkspaceRead` answers null on a decline, and the gate then moves nothing.
The gate asks it inside its failure policy, so a dialog that fails is reported as `Workspace.OpenFailed`.
`CEnvoyCoinageRead` answers the wording as typed, or null when the user retreats.
The clerk behind the gate trims the wording, so the driver hands it back untouched.

## `CScreen`

What a video screen plays, as the engine read it from a stored location.
It carries a resolved address, a file or a web address, and the hosted film id for a web address.

## `CWorkspace`

The workspace folder's life cycle within the session: opening it and leaving it.
Opening raises the events every view restores on.
Leaving gathers pending-work verdicts before asking the user, then tells every area whether to store or discard.
The atelier builds it once and keeps the open and quit gates.

Each area registers its draft verdict, vista restore and close in its constructor, in registration order.
The input tab's editor registers from the atelier.
So no driver hands a seam up.
A workspace change runs every vista restore, so every area stands on the new workspace's vistas.
Only the window's exit runs the closes.

The ledger and status bulletins attach on the first open.
An atelier is also built over fake ports that answer no attach.
`_cWorkspaceEnvoy` is the latest open's envoy.
Status bulletins and failure notices use the latest envoy, so later opens do not keep the first one.
`_cWorkspaceUnshown` keeps a layout save failure heard before any open.
The next open shows it through its envoy and clears it, so the notice is never lost.
`_cWorkspaceShown` gains a key only when shown, so a fault before any open never spends the one notice.

### Events

`CWorkspaceOpened` is raised on every open once the workspace is swept.
A driver subscribes each view in its introduce, so one gate call fans out without a second request.
`LWorkspaceStateOpened` follows with the entries the duplex wings last stood on.
The views restore first, so a wing lists before it opens its entry.
Each `CWing` subscribes itself.
`CWorkspaceEstablishmentChanged` is raised on every open and after every bulletin.
A read that fails raises nothing, so a busy or closing workspace leaves the strip as it stood.
The failure is shown under `Workspace.EstablishmentFailed`.

### Change

`CWorkspaceChange` moves the session onto the raw `chosen` path and answers the folder then in use.
The engine judges the path, so a blank path or the folder already in use asks nothing and moves nothing.
Otherwise the quit asks first, since the move drops every open form.
It is the one leave question the window's closing asks, put through the envoy only for unsaved work.
One engine call moves, records, sweeps and answers the new workspace's state.
A move that fails shows `Workspace.OpenFailed` through the envoy, and the old workspace stays.
The restore plan follows a move: every area's vistas, then the open events, then the stored tab.
The driver paints the answered folder, so a declined or failed move puts the field back.

The overload without a path asks the envoy for the folder, starting from the folder in use.
The question is asked inside the failure policy, so a dialog that fails shows `Workspace.OpenFailed`.
A declined question moves nothing and answers the folder in use.

### Open order

`LWorkspaceOpen` runs after the sweep.
Its order is the views, the ledger, the status, the wings' state, then the stored tab.
A layout failure kept from before the open is shown first through the envoy, then dropped.
Each open keeps its envoy as the latest.

### Quit

`LWorkspaceQuitConfirm` decides whether the session may end over every area that holds a draft.
Each area is asked even after one answered yes, because the asking writes a pause-held keystroke down.
Unsaved work is put to the user once, through `CEnvoyLeaveConfirm`.
Nothing unsaved closes every area without a question, as a discard.
Storing commits every draft and discarding cancels every one.
Staying closes nothing and answers no.
Every closure is called despite earlier refusals, so one failed area never prevents another from finishing.
The session may end only when all of them are finished.
A save the engine refuses answers no, so the window stays over the entry it failed to store.

### Close and failures

`LWorkspaceClose` runs every area close in build order.
It then detaches the posture's save failure observer, before the atelier releases the posture.
Only `CAtelier.CAtelierClose` calls it, before the leftover sweep.

`LWorkspaceObserverAttach` shows the posture's `LPostureSaveFailed` as `Layout.SaveFailed` through the latest envoy.
The driver's marshal puts the notice on the driver's thread whichever thread saved.
The posture raises it once per open workspace, so a drag that keeps failing shows one notice.
The clerk already recorded the fault, so the notice goes to `CEnvoyFailureShow` with its key alone.
A second attach replaces the first, so the notice is never heard twice.
In product code only `CAtelierOpen` calls it.

`LWorkspaceFailureShow` serves gates that take no envoy, such as `CAtelier.CAtelierVolumeSet`.
It shows once per session for each key, since a volume slider fires many sets while dragged.
The first fault with an envoy held goes through `CLedger.LLedgerFailureShow`, which records and shows it.
Every later fault, and any before an open, is only recorded through `CLedger.LLedgerNoticeRead`.
So each fault is recorded exactly once, here where it ends.

`LWorkspaceEstablishmentRead` gives the size and unsaved work for the status strip.
The engine judges the singular count and the unit, and the workspace chooses their wording keys.
The amount is written with one decimal in megabytes and as a whole number in kilobytes.

## Stamp

Commit: `c2f335c331fe`
- `CAtelier.cs` `f47a17e14661cb82`
- `CEntryBundle.cs` `bf4bf14472f5898b`
- `CEnvoy.cs` `ba575c886117535b`
- `CPhonologyBundle.cs` `c969d9a57ef9c34d`
- `CScreen.cs` `060eb21146468883`
- `CWorkspace.cs` `e3533e69d610ae75`
