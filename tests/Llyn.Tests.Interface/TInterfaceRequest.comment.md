# TInterfaceRequest.cs
Hash: `e69e88557559ec73`

## `internal static partial class TInterface`

Builds the request records a test sends, so no test constructs a logic record itself.
Each factory returns the base type, because the engine takes the base and switches on the kind.
A factory name holds at most three components after the T prefix, counting the closing `Create`.
A one-part record such as `LRequestNote` keeps `Request`, since it fits within the limit.
A two-part record such as `LRequestSentenceAddition` drops `Request` to stay within the limit.
The card records are the exception and keep `Request` but drop `Card` instead.
`TRequestAuthorCreate` is a second exception, dropping `Name` from `LRequestAuthorName`.
It duplicates `TAuthorNameCreate`, which follows the two-part rule for the same record.
