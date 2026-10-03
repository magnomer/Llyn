# TInterfaceEngineWorkspace.cs
Hash: `63033da1653811c7`

## `internal static class TInterfaceEngineWorkspace`

The relays for the engine's workspace, its rig and its state.
That is the workspace start, open, read and format, the folder open and the location reads.
The rescue read, the status bar's establishment read and the revision and state reads are relayed here too.
The observer attach and detach and the recording sweep are relayed here as well.
The rig apply, the usher swap and the two record builders sit here too.
Each relay is transparent and carries no test logic of its own.

## `internal static void TEngineWorkspaceOpen(this LEngine engine, string path)`

Builds a rig for `path` through the real factory with fake sources and applies it, as the bootstrap does.
The clock is set here through the `TClockFake` every test rig carries, so time freezes engine-wide.
