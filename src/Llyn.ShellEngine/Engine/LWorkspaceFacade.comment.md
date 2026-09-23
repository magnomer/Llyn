# LWorkspaceFacade.cs

## `internal sealed class LWorkspaceFacade`

The engine's facade for workspace state and trail operations.

## `public LWorkspaceFacade(LEngine engine)`

Stores the engine and its gate.

## `public LWorkspaceState LEngineStateRead()`

The workspace state row.

## `public void LEngineLeftSave(long? id)`

Remembers the entry shown on the left.

## `public void LEngineRightSave(long? id)`

Remembers the entry shown on the right.

## `private void LWorkspaceStateChange(Func<LWorkspaceState, LWorkspaceState> change)`

Reads, changes and writes the state row under the gate.

## `public Uri? LEngineLocationResolve(string? location)`

A location resolved against the workspace, through the trail clerk.

## `public Uri? LEngineLocationRead(string? location)`

The resolved location, or null when it is a file that does not exist.

## `public void LEngineLocationOpen(string target)`

Opens `target` through the shell usher.
