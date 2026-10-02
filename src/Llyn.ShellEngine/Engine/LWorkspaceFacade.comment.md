# LWorkspaceFacade.cs
Hash: `d49c7a558c5f681e`

## `internal sealed class LWorkspaceFacade`

The engine's facade for workspace state and trail operations.

## `public LWorkspaceFacade(LEngine engine)`

Stores the engine and its gate.

## `public LWorkspaceState LEngineWorkspaceStart()`

The order of opening a workspace is the draft sweep, the recording sweep, then the state row.
Each sweep takes its own facade's lock, so this member holds none.

## `public LWorkspaceState LEngineWorkspaceChange(string chosen)`

The order of a workspace switch is the move onto `chosen`, then the same open a start runs.
So the new workspace is swept before any view restores on it.

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
