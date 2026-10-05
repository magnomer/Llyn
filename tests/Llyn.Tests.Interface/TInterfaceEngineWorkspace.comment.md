# TInterfaceEngineWorkspace.cs
Hash: `9509ee4f208c3ae1`

## `internal static class TInterfaceEngineWorkspace`

The relays for the engine's workspace, its rig and its state.
That is the workspace start, open, read and format, the folder open and the location reads.
The rescue read, the status bar's establishment read and the revision and state reads are relayed here too.
The observer attach and detach and the recording sweep are relayed here as well.
The rig apply, the usher swap and the three record builders sit here too.
Each relay is transparent and carries no test logic of its own.

## `internal static void TEngineWorkspaceOpen(this LEngine engine, string path)`

Builds a rig for `path` through the real factory, then applies it as the bootstrap does.
Its sources, press, warrant and phonograph are all fakes.
The clock is set here through the `TClockFake` every test rig carries, so time freezes engine-wide.

## `internal static LBulletin TBulletinCreate(LSubject subject, long id) => new(subject, id);`

Builds a bulletin for `subject` and `id`, so a conduct test can send one to the observers it holds.
