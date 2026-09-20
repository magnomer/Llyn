# LEngineWorkspace.cs

## `public sealed partial class LEngine`

The workspace state and location facades, forwarding to the workspace and trail clerks.

## `public LWorkspaceState LEngineStateRead()`

The workspace state row.

## `public void LEngineLeftSave(long? id)`

Remembers the entry shown on the left.

## `public void LEngineRightSave(long? id)`

Remembers the entry shown on the right.

## `private void LEngineStateChange(Func<LWorkspaceState, LWorkspaceState> change)`

Reads, changes and writes the state row under the gate.

## `public Uri? LEngineLocationResolve(string? location)`

A location resolved against the workspace, through the trail clerk.

## `public Uri? LEngineLocationRead(string? location)`

The resolved location, or null when it is a file that does not exist.

## `public void LEngineLocationOpen(string target)`

Opens `target` through the shell usher.
