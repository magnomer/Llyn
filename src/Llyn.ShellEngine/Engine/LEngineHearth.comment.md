# LEngineHearth.cs
Hash: `4e01284d73088905`

## `internal sealed class LEngineHearth`

The engine's shared state: the gate, the staff, the settings, the trove, the stale drafts and the observers.
Every facade reads it, so no facade needs the engine itself for state.
It swaps the staff as one unit when the engine moves onto another workspace.

## `internal LEngineHearth(LRig rig)`

Builds the clerks over `rig`, then reads the settings through them.
It leaves the workspace closed, since the workspace facade creates the database first.

## `internal object LEngineGate { get; } = new();`

The shared lock every facade and fetching clerk uses for engine state.
Callers hold it while reading or changing the state below.

## `internal LTrove LEngineTrove { get; } = new();`

The engine session cache, shared with the facades that read and clear it.

## `internal LSettings LEngineSettingsHeld { get; set; }`

The current settings snapshot.
Callers read and update it under `LEngineGate`.

## `internal HashSet<long> LEngineDraftStale { get; } = [];`

Draft ids claimed in a former workspace, kept so a surviving tenure cannot write into the new one.

## `internal long LEngineRevision { get; set; }`

A count that moves on every announced change, so a tenure knows when its kept state went stale.
Callers read and move it under `LEngineGate`.
A facade that writes without announcing moves it by hand, so no kept state outlives the write.

## `internal LEngineStaff LEngineStaffHeld`

Returns the current staff record for engine facades that need a clerk.
The record changes as one unit when `LEngineRigApply` moves to another workspace.
It is read under the gate, so a caller outside the gate still sees one whole record.

## `internal LSettings LEngineSettingsRead()`

The current settings snapshot, read under the gate.
The fetch clerks hold this reader, so it exists before any facade does.

## `internal void LEngineWorkspaceOpen()`

The four steps a bound workspace needs before the first read.
The controlled vocabularies come from the language packs on disk and are written into the workspace.
The vocabulary clerk imports them directly, so the hearth needs no facade.
`LEngine` calls it once its facades exist, and `LEngineRigApply` calls it after the swap.
The 音韻地位 categories are derived again from the stored fanqie rows and the hypothesis on disk.
The phonetic series of every pack that declares a series source are built again.
`LLanguageStaff.LLanguageStaffApply` runs those two steps on the language group.
A workspace rebuilt from an older schema has its derived strings filled once through the workspace clerk.

## `internal void LEngineRigApply(LRig rig)`

Moves the engine's clerks, caches and settings onto the workspace `rig` was built over.
`LWorkspaceFacade.LEngineRigApply` calls it under the gate, and keeps the folder, the rescue and the bulletin.
The new rig's settings are read through the clerk's static open step before anything here changes.
A folder that cannot be opened therefore leaves the old clerks and caches untouched.
A workspace that already holds a settings file is opened on its own settings.
One without any receives the current settings and has them written.
That write goes through the clerk's `LWorkspaceFallbackSave`, which records a vault fault and swallows it.
So the switch never stops halfway, and the next settings save tries again.
The drafts this engine claimed are forgotten with the old folder and each id is marked stale.
The old recording clerk stops its playback, since the shared phonograph outlives it.
The pending fetches of the old clerks are cancelled, and the flag cache and the trove are cleared.

## `internal void LEngineStaffClear()`

Cancels every pending fetch of the staff held now, so a closed engine leaves no task writing into the workspace.
`LLanguageStaff.LLanguageStaffClear` does the cancelling, since the language group holds every fetching clerk.
It runs under the gate, so a workspace switch cannot swap the staff halfway.

## `internal void LEngineObserverAttach(Action<LBulletin> observer)`

Subscribes `observer` to future announcements.
An already attached delegate is not added a second time.
It names no subject or id, so it hears every announcement.

## `internal void LEngineObserverDetach(Action<LBulletin> observer)`

Stops announcing to `observer`, so a closed surface is never called again.

## `internal void LEngineBulletinRaise(LSubject subject, long id)`

Announces one stored change to every subscriber.
The revision moves first, even with nobody listening, because a tenure reads it whether or not it subscribed.
The `LBulletinRoster` is copied under the gate, then dispatched outside it.
So a subscriber can detach without disturbing iteration.
