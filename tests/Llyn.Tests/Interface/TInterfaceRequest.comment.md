# TInterfaceRequest.cs

## `internal static partial class TInterface`

Builds the request records a test sends, so no test constructs a logic record itself.
Each factory returns the base type, because the engine takes the base and switches on the kind.
The list factories drop `Request` from their names, because four components after the prefix is one too many.
