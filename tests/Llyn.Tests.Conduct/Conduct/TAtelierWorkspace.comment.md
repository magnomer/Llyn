# TAtelierWorkspace.cs
Hash: `713cd91271db51c1`

## `public sealed class TAtelierWorkspace`

Covers the atelier's workspace change gate over a real posture on the fake rig.
It builds its atelier through `TInterfaceConduct.TAtelierCreate`.
A workspace change moves the engine, writes the pointer and answers the folder now in use.
It opens the new workspace's state after the views, and restarts every area's vista before the views restore.
A blank path or the folder in use moves nothing, asks nothing and opens nothing.
Unsaved work is asked about once, as the quit asks, and a cancelled leave moves nothing and finishes nothing.
A stored answer finishes every area before the move.
A folder that fails is shown as `Workspace.OpenFailed`, leaves the engine where it stood and writes no pointer.
The folder question starts from the folder in use, and its answer is moved onto.
A declined folder question moves nothing, and a failing one is shown as `Workspace.OpenFailed`.

## `private static CEnvoy TAtelierEnvoyCreate(Func<string, string?> folder, List<string> asked)`

An envoy whose folder question records the folder it starts from and answers through `folder`.
A shown failure records its key in the same list.
