# TInterfaceEngine.cs

## `internal static partial class TInterface`

The relays for the engine operations over an entry as a whole.
That is saving, loading, updating, and deleting one, and the drafts and revisions around it.
The stored view state and the settings the shell pushes a field at a time are relayed here too.
Each relay is transparent and carries no test logic of its own.
