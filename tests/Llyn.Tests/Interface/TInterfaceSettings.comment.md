# TInterfaceSettings.cs

## `internal static partial class TInterface`

The relays for the settings file and the posture the shell keeps beside it.
The settings loader is relayed here, so the file contract can be checked without an engine.
The posture loader is relayed the same way, so the posture text contract can be checked without a keep.
The posture itself is started over an engine and pushed a field at a time, as the window pushes it.
Each relay is transparent and carries no test logic of its own.
