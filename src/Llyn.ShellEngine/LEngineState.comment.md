# LEngineState.cs

## `public sealed partial class LEngine`

The shell's own view state as the engine keeps it.
The shell reads the whole state once when a window attaches.
It pushes one field back each time the user changes it.
Every push is a field of its own.
Two panels changing two different things in one session both keep their change.
A shell that read the state, changed a field and wrote the whole record back would lose work.
Whatever another panel had written in between would be gone.

The Entry each duplex side stands on names a row of the workspace database, so it is stored beside it.
The open tab and its split name only panels, so they go to the settings file.
Each panel's ordering and hidden languages go there too, but through the panel's vista rather than from here.
The vista saves under the tab name the shell started it with, so no tab constant lives here any more.

## `public LWorkspaceState LEngineStateRead()`

Reads the stored view state of the open workspace.
It is the one call a window makes on attach, before any panel is shown.

## `public void LEngineLeftSave(string? id)`

Stores the Entry the left duplex side stands on, or nothing when that side is cleared.

## `public void LEngineRightSave(string? id)`

Stores the Entry the right duplex side stands on, or nothing when that side is cleared.

## `private void LEngineStateChange(Func<LWorkspaceState, LWorkspaceState> change)`

Reads the stored state, applies `change` to it, and writes it back, all under the engine gate.
The read and the write are one step.
A field pushed from one panel never overwrites a field pushed from another.
