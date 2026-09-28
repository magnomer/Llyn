# CAtelier.cs

## `public sealed class CAtelier : IDisposable`

Conduct's root: the working session over one workspace, which Host builds and hands to a driver.
It holds the six engine ports and the session's posture.
Its gates are the medium-free actions no single panel owns.
Those are workspace change, leftover sweep, bulletins, vista start, volume, split and the open view.
The session reads, the recordings and the folder locations sit here too, since the workspace owns them.
GUI-only state such as window geometry and panel widths never reaches it.

## `internal CAtelier(`

Takes the posture and the six ports Host builds over one engine.
It is internal, so no driver can build a root of its own.

## `public CMention CAtelierMention { get; }`

The mention gates, built once over this atelier's ports.

## `public CMarkdown CAtelierMarkdown { get; }`

The markdown gate, built once over this atelier's entry port.

## `public CRespelling CAtelierRespelling { get; }`

The respelling gates, built once over this atelier's phonology port.

## `public CNavigation CAtelierNavigation { get; }`

The session's one navigation, built once over this atelier.
Every panel area registers its tab here, and every jump to a tab goes through it.

## `public CCatalog CAtelierCatalog => new(this);`

The reference reads over this atelier's ports.
The catalog holds no state, so each read builds a fresh one and the atelier keeps no slot for it.

## `public CLedger CAtelierLedger => new(this);`

The settings ledger over this atelier's settings port.
The ledger holds no state, so each read builds a fresh one and the atelier keeps no slot for it.

## `internal LPosture CAtelierPosture { get; }`

The session's posture, for the Conduct areas built over it, such as the navigation.
It is internal, since the posture is an engine handle.

## `internal LDraftPort CAtelierDraftPort { get; }`

A transitional handle for the deportments not yet moved into Conduct.
The five other port handles below it stand for the same reason.
job14-30 deletes all six.


## `internal void CAtelierInputRestore(CEditor editor)`

Binds `editor` to the input tab's vista, which lists entries by headword.
It runs again after a workspace change, so the input tab follows the new workspace.

## `internal LVista CAtelierVistaStart(string tab, CSubject? subject, CCatalogOrder fallback, bool blank = false)`

Starts the tab's vista on its stored order, filter and split.
The driver names the subject and order in Conduct's enums, so it never names a Core one.
It is internal, since the vista it answers is an engine handle.
The subject and order reach the engine through `CPanel`'s by-name maps, never by cast.

## `internal bool LAtelierSplitRead()`

Whether the open tab shows its editor rather than its read area.
Only the navigation reads it, when it restores the open tab at startup.

## `public double CAtelierVolumeRead()`

The session's one audio level, between zero and one.

## `public void CAtelierVolumeSet(double volume, bool settled)`

Sets the one audio level and hands it to the player.
The level is written only when `settled`, so a drag costs no write per step.
A driver passes `settled` once the gesture ends, and a key press is settled at once.

## `public void CAtelierLeftoverSweep()`

Drops the draft files no editor holds any more, and the recordings no entry keeps.
A driver calls it once the session opens.

## `public bool CAtelierQuitConfirm(IReadOnlyList<Func<bool>> pending, IReadOnlyList<Func<bool, bool>> closures, CEnvoy envoy)`

Decides whether the window may close over every editor it holds.
Every editor is asked whether it holds unsaved work.
Each is asked even after one answered yes, because the asking writes a pause-held keystroke down.
Unsaved work is put to the user through `CEnvoyLeaveConfirm`.
Nothing unsaved closes every editor without a question, as a discard.
Storing commits every draft and discarding cancels every one.
Staying closes nothing and answers no.
Every editor is told before the answers are read, so one refusal leaves no other draft held.
The window may go only when all of them are finished.
A save the engine refuses answers no, so the window stays over the entry it failed to store.

## `public CWorkspaceState? CAtelierWorkspaceChange(string chosen, CEnvoy envoy)`

Moves the session onto the workspace at `chosen`, trimmed, and answers the state its view restores to.
A blank path or the folder already in use changes nothing, asks nothing and answers nothing.
Otherwise `envoy` is asked first, since the move drops every open form.
The engine records the folder only once the move succeeds, and a failed move throws to the driver.
The engine's workspace bulletin tells every attached view to show itself again.

## `public string CAtelierPathRead()`

The workspace folder in use, as a settings view shows it in its path field.

## `public CWorkspaceState CAtelierStateRead()`

The entries the duplex wings last stood on.

## `public CEstablishment CAtelierEstablishmentRead()`

The workspace's size and unsaved work, as the status strip shows it.
The engine judges the singular count and the unit, and the atelier chooses their wording keys.
The amount is written with one decimal in megabytes and as a whole number in kilobytes.

## `public Action CAtelierEstablishmentAttach(Action<CEstablishment> show)`

Hands `show` the status at once and again after every bulletin, and answers the detach.
So a status strip makes one call to open and never polls.

## `public bool CAtelierRecordingExist(string? file)`

Whether the recording file `file` names exists in the workspace.

## `public Task<string> CAtelierRecordingPrepare(CRecording recording, CancellationToken cancellation)`

Downloads a remote recording into the workspace and answers the local path it was saved to.

## `public Uri? CAtelierLocationRead(string? location)`

The resolved address of a media location, or nothing when it is a file that does not exist.

## `public CScreen? CAtelierScreenRead(string? location)`

What a video screen plays from a location, or nothing when the location resolves to nothing.
The engine resolves the address and reads the film id in one call, and the atelier only maps the pair.

## `public void CAtelierLocationOpen(string target)`

Opens `target` through the engine's shell usher, so the driver starts no process itself.

## `public static string CAtelierAboutRead()`

The wording key of the version line the product's about notice shows.
It is static, since every session words the notice alike.
The build number stays with the driver, because Conduct is framed away from reflection.

## `public static string CAtelierRefusalRead(bool busy)`

The wording key of the notice a workspace that failed to open shows.
A database another program holds says so, since closing that program is the cure.
It is static, since no atelier stands when the workspace fails to open.

## `public static string? CAtelierRescueRead(bool done)`

The wording key of the notice that the database was set aside, or null when it was not.
Without the notice the workspace would appear empty and the old data would look lost.
It is static, since the host reads the rescue before it builds the atelier.

## `public void Dispose()`

Sweeps the leftover drafts once more, then releases the posture, which lets go of every vista it watched.
Sweeping on the way out as well as on the way in bounds what a long session leaves behind.

## `public Action CAtelierObserverAttach(CSubject subject, Action<CBulletin> observer)`

Hands `observer` only the bulletins about `subject`, and answers the detach.
The Conduct subject is mapped by name to the engine's, and the compare is between engine values.

## `private void CAtelierEstablishmentShow(Action<CEstablishment> show)`

Reads the status and hands it to `show`.
A read that fails is skipped, so a busy or closing workspace leaves the strip as it stood.

## `internal static CBulletin CAtelierBulletinRead(LBulletin bulletin)`

The one map from an engine bulletin to Conduct's, shared by every attach that bridges one.
