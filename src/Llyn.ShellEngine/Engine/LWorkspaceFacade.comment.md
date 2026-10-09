# LWorkspaceFacade.cs
Hash: `f0eb53a0cb7a7ee9`

## `public sealed class LWorkspaceFacade`

The engine's facade for workspace state, trail operations, the workspace folder and failure records.
It is public, because Host reads the rescue and hands the audit record to the deportment.

## `internal LWorkspaceFacade(LEngineHearth hearth, LDraftFacade draft, LPronunciationFacade pronunciation, LRig rig, Func<string, LRig> factory, Action<string> pointer)`

Stores the hearth, its gate, the sibling facades it calls, and the rig factory and pointer writer Host hands in.
The gate, the staff and the shared state are read through the hearth.
It takes its siblings rather than the engine, so it names only the facades it uses.
The folder and the rescue are read from the first rig, so a reader sees them before any switch.

## `public LWorkspaceState LEngineWorkspaceStart()`

The order of opening a workspace is the draft sweep, the recording sweep, then the state row.
Each sweep takes its own facade's lock, so this member holds none.

## `public LWorkspaceState LEngineWorkspaceChange(string chosen)`

Moves the engine onto the folder `chosen` names, trimmed by the clerk, then records it as the workspace.
A choice that names no other workspace throws, since the check comes first.
The pointer is written only after the move succeeds, so a failed folder never becomes the next launch's workspace.
The same open a start runs follows, so the new workspace is swept before any view restores on it.

## `public void LEngineRigApply(LRig rig)`

Moves the engine onto the workspace `rig` was built over, without touching the workspace pointer.
`LEngineWorkspaceChange` builds the rig and writes the pointer after.
The new rig's rescue is read before anything changes, so a folder that cannot open leaves all as it was.
The engine then moves its clerks, caches and settings, and the folder and rescue follow under the same gate.
The move is then announced, so every surface holding a stored record learns that all of it is stale.

## `public LDoctorRescue LEngineRescueRead()`

Reports what the workspace doctor had to do to the database this engine opened.
The facade holds the answer rather than raising it, because the shell asks once the engine exists.
The answer is replaced when `LEngineRigApply` opens another workspace.

## `public string LEngineWorkspaceRead()`

Returns the current workspace folder, where the user's settings and database are stored.

## `public string LEngineWorkspaceFormat()`

The workspace folder's own name, for the settings ledger, and the full path when the root has none.

## `public string? LEngineAuditRecord(Exception exception)`

Writes one unexpected fault into the open workspace's audit log through the workspace clerk.
It answers with the file the fault went to, or `null` when nothing could be written.

## `public string? LEngineNoticeRead(Exception exception)`

The reason key of a refusal standing anywhere inside the failure, or null for a fault.
The workspace clerk walks the inner chain, since the shells name no exception type.

## `public (string LEngineFailureNotice, string? LEngineFailureLabel, string? LEngineFailurePath) LEngineFailureRead(Exception exception, string unexpected, string recorded)`

The ready notice of a failure, in the order the reasons are tried.
A refusal answers its own reason key alone.
A fault is written to the audit log and answers `unexpected`, with `recorded` and the file when the write succeeded.
Conduct hands both wording keys down, so the engine chooses no wording of its own.

## `public bool LEngineWorkspaceCheck(string chosen)`

Whether `chosen` names a workspace other than the one in use, by the workspace clerk's rule.
Conduct asks it before the leave question, so a blank or unchanged path asks nothing.

## `public LWorkspaceState LEngineStateRead()`

The workspace state row.

## `public void LEngineLeftSave(long? id)`

Remembers the entry shown on the left.

## `public void LEngineRightSave(long? id)`

Remembers the entry shown on the right.

## `private void LWorkspaceStateChange(Func<LWorkspaceState, LWorkspaceState> change)`

Reads, changes and writes the state row under the gate.

## `public Uri? LEngineLocationRead(string? location)`

The resolved location, or null when it is a file that does not exist.

## `public (Uri, string?)? LEngineScreenRead(string? location)`

The resolved address of a video location and its hosted film id, by the Application trail clerk.

## `public void LEngineLocationOpen(string target)`

Opens `target` through the shell usher.

## `public void LEngineFolderOpen()`

Opens the workspace folder in use through the shell usher.
The path is read here, so the caller hands nothing it gathered.

## `private readonly Func<string, LRig> _lWorkspaceFacadeFactory;`

Builds a rig over a chosen folder, handed by Host so the engine names no infrastructure.

## `private readonly Action<string> _lWorkspaceFacadePointer;`

Records the chosen folder as the next launch's workspace.

## `private string _lWorkspaceFacadeFolder;`

The workspace folder in use, replaced under the gate on every rig apply.

## `private LDoctorRescue _lWorkspaceFacadeRescue;`

What the workspace doctor did to the database in use, replaced under the gate on every rig apply.
