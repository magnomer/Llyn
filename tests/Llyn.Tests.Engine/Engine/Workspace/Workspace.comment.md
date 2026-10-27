# Workspace

This folder holds the tests of the workspace.
They cover moving between folders, the clerk open sequence, the stored location rule and the recording sweep.

## `TEngineLocation.cs`

It covers the one rule for where a stored location may point.
A path naming a host or a device is refused, whatever spelling it takes.
A relative path resolves inside the workspace and is refused once it walks out.
A drive path, a hostless `file` address and a web address pass as written.
The read form answers a file address only when the file is on disk, and a web address as written.

## `LocationResolve_HostOrDevicePath_ReturnsNull`

Each row is a spelling Windows would resolve through a network or a device handle.
The lone leading separator is a root of the current drive, which the workspace rule cannot vouch for.

## `TEngineRecordingSweeping.cs`

It covers the sweep that drops recording files nothing names.
A file under `audio` that no pronunciation and no draft names is deleted.
A file a stored pronunciation names survives.
The path is stored relative to the workspace, so the sweep resolves it before comparing.
A file a draft on disk names survives, since a held draft may still commit it.

## `TEngineWorkspace.cs`

It covers the engine moving from one workspace folder onto another.
A move test opens two temporary folders and calls the open alone, since the pointer file lives outside any workspace.
A target with its own settings file is opened on those settings.
An empty target inherits the settings held, and they are written into it at once.
A path that is not fully qualified is refused, and no folder of that name appears.
A target whose database cannot open leaves workspace, settings, state and held drafts as they were.
A draft held before the move is refused as stale when committed after it.
Both workspaces count draft ids from their own floor, so a fresh target would issue the held id again.
The target therefore skips that id, and its new draft reads normally.
Cancelling the stale id releases it, so the engine no longer holds it.
The move raises the workspace bulletin once to an attached observer.
A setting saved after the move lands in the target, and the target reopens on it.
The folder open hands the workspace folder in use to the usher.
The workspace format answers the open folder's name.
A leased engine holds its workspace.
A second leased engine over it throws a lease fault the busy check recognises.
Disposing the engine frees the workspace for a new leased engine.
Moving a leased engine takes the target's lease and frees the old workspace.
A construction that fails after the lease opens frees it too.

## `TUsherOpened`

It is an usher that records every target it is asked to open and touches no disk.

## `TWorkspaceClerk.cs`

It covers the static open sequence of the workspace clerk over the fake rig, without SQLite.
A rig whose doctor cannot open the database throws out of the first open step.
The engine therefore keeps its old clerks, which is the unopenable-target contract.
A blank choice or the folder in use, padded or not, names no move.
Another folder is answered trimmed, as the engine records it.
A rig with no stored settings answers the fallback and says so, so the caller writes it down.
Stored settings win over the fallback.
The refusal reason is found through two wrapping exceptions, and a bare exception answers nothing.

## `TDoctorFake`

It is a doctor that refuses to open, standing in for a database folder that is a file.

## Stamp

Commit: `9377e657b395`
- `TEngineLocation.cs` `9219f6de648be7bf`
- `TEngineRecordingSweeping.cs` `8bd58ff6afe6f6b4`
- `TEngineWorkspace.cs` `88becbae5ff09b43`
- `TWorkspaceClerk.cs` `e0cc1df28517227c`
