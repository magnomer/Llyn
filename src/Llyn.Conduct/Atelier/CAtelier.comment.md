# CAtelier.cs

## `public sealed class CAtelier : IDisposable`

Conduct's root: the working session over one workspace, which Host builds and hands to a driver.
It holds the six engine ports and the session's posture.
Its gates are the medium-free actions no single panel owns.
Those are opening, quitting and changing the workspace, bulletins, vista start, volume and split.
The session reads, the recordings and the folder locations sit here too, since the workspace owns them.
GUI-only state such as window geometry and panel widths never reaches it.

## `internal CAtelier(`

Takes the posture and the six ports Host builds over one engine.
It is internal, so no driver can build a root of its own.

## `public CMention CAtelierMention { get; }`

The mention gates, built once over this atelier's ports.

## `public CMarkdown CAtelierMarkdown => new(this);`

The markdown gate over this atelier's entry port.
It holds no state, so each read builds a fresh one and the atelier keeps no slot for it.

## `public CRespelling CAtelierRespelling => new(this);`

The respelling gates over this atelier's phonology port.
They hold no state, so each read builds a fresh one and the atelier keeps no slot for them.

## `public CNavigation CAtelierNavigation { get; }`

The session's one navigation, built once over this atelier.
Every panel area registers its tab here, and every jump to a tab goes through it.

## `public CCatalog CAtelierCatalog => new(this);`

The reference reads over this atelier's ports.
The catalog holds no state, so each read builds a fresh one and the atelier keeps no slot for it.

## `public CLedger CAtelierLedger { get; }`

The settings ledger, built once, since it raises `CLedgerChanged` to the views that subscribed.

## `public CWorkspace CAtelierWorkspace { get; }`

The workspace's life cycle, built once: the open events and the drafts the quit asks about.
The gates that open, quit and close it stay on the atelier.

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
The workspace keeps the editor, so the quit can ask whether it holds unsaved work.

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

## `public void CAtelierOpen()`

Opens the session on the workspace in use, at startup and after every workspace change.
One engine call sweeps the leftover drafts and recordings and answers the stored state.
The sweep runs before any view restores, so nothing already saved is counted as lost work.
`CAtelierWorkspace` then raises its open events, and the stored tab opens last.

## `public bool CAtelierQuitConfirm(CEnvoy envoy)`

Decides whether the session may end over every area that holds a draft.
The workspace asks every area and puts the one question through `envoy`.
The window's closing reaches it, and `CAtelierWorkspaceChange` asks the same workspace quit inside Conduct.

## `public CWorkspaceState? CAtelierWorkspaceChange(string chosen, CEnvoy envoy)`

Moves the session onto the workspace at `chosen`, trimmed, and answers the state of the workspace moved onto.
A blank path or the folder already in use changes nothing, asks nothing and answers nothing.
Otherwise the workspace's quit asks first, since the move drops every open form.
It is the one leave question the window's closing asks, put through `envoy` only when a form holds unsaved work.
The engine records the folder only once the move succeeds, and a failed move throws to the driver.
The driver then calls `CAtelierOpen`, which sweeps and restores the views on the new workspace.

## `public string CAtelierPathRead()`

The workspace folder in use, as a settings view shows it in its path field.

## `private static CWorkspaceState LAtelierStateRead(LWorkspaceState state)`

The plain map from the engine's state row to the entries the duplex wings last stood on.

## `internal CEstablishment LAtelierEstablishmentRead()`

The workspace's size and unsaved work, as the status strip shows it.
The engine judges the singular count and the unit, and the atelier chooses their wording keys.
The amount is written with one decimal in megabytes and as a whole number in kilobytes.
Only the workspace's status raise reads it.

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

## `public void CAtelierClose()`

Ends the session once the window has closed and every view has stopped.
It sweeps the leftover drafts once more, then releases the posture, which lets go of every vista it watched.
Sweeping on the way out as well as on the way in bounds what a long session leaves behind.

## `public void Dispose()`

Closes the session through `CAtelierClose`, for a host or a test that holds the atelier in a `using`.

## `internal Action LAtelierObserverAttach(CSubject subject, Action<CBulletin> observer)`

Hands `observer` only the bulletins about `subject`, and answers the detach.
The Conduct subject is mapped by name to the engine's, and the compare is between engine values.
Only the ledger hears through it, since every view now answers a Conduct event.

## `internal Action LAtelierObserverAdd(Action<LBulletin> sent)`

Hands `sent` every engine bulletin, and answers the detach.
The workspace hears the status through it.

## `internal static CBulletin CAtelierBulletinRead(LBulletin bulletin)`

The one map from an engine bulletin to Conduct's, shared by every attach that bridges one.
