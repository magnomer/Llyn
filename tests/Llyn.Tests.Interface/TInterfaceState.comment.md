# TInterfaceState.cs
Hash: `23703b8485b4e3c9`

## `internal static class TInterfaceState`

The relays for the state values and state anchors every lexical record carries.
They create, resolve, show and read the states a test hands in or checks.
Each relay is transparent and carries no test logic of its own.

## `internal static LStateWritten TStateWrittenRead(this LStateValue value)`

A missing value reads as the empty written state.
A present value keeps its shown text and whether it is unknown.
