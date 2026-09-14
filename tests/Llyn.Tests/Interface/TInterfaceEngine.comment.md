# TInterfaceEngine.cs

## `internal static partial class TInterface`

The relays for the engine operations over an entry as a whole.
That is saving, loading, updating, and deleting one, and the drafts and revisions around it.
The stored view state and the settings the shell pushes a field at a time are relayed here too.
The settings loader is relayed here as well, so the file contract can be checked without an engine.
The frequency fill, its band resolution, and its read and start are relayed here too.
The grasp read and save are relayed here as well.
The script read, its pending check and its find are relayed here too.
The markup read, find, import and export and the portrait export are relayed here too.
The page portrait read, both prints and the press hand-in are relayed here as well.
Each relay is transparent and carries no test logic of its own.
