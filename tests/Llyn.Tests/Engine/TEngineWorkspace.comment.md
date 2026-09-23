# TEngineWorkspace.cs

## `public sealed class TEngineWorkspace`

Covers the engine moving from one workspace folder onto another.
Every test opens two temporary folders and calls the open alone, since the pointer file lives outside any workspace.
A target with its own settings file is opened on those settings.
An empty target inherits the settings held, and they are written into it at once.
A path that is not fully qualified is refused, and no folder of that name appears.
A target whose database cannot open leaves workspace, settings, state and held drafts as they were.
A draft held before the move is refused as stale when committed after it.
Both workspaces count draft ids from their own floor, so a fresh target would issue the held id again.
The target therefore skips that id, and its new draft reads normally.
Cancelling the stale id releases it without touching any draft of the target.
The move raises the workspace bulletin once to an attached observer.
A setting saved after the move lands in the target, and the target reopens on it.

## `private sealed class TWorkspaceObserver : LObserver`

Collects every bulletin the engine raises, so a test can count them and read their subjects.
