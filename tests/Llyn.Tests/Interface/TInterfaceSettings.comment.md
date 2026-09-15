# TInterfaceSettings.cs

## `internal static partial class TInterface`

The relays for the settings file and the stored view state.
The settings loader is relayed here, so the file contract can be checked without an engine.
The view state and the settings the shell pushes a field at a time are relayed here too.
Each relay is transparent and carries no test logic of its own.
