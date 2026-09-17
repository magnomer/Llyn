# TInterfaceVista.cs

## `internal static partial class TInterface`

The relays for a vista, the engine's view state for one catalog tab.
The start and the row finds are relayed over the engine, and every change over the vista itself.
Each relay is transparent and carries no test logic of its own.
The start relay passes the blank flag through with the same default the engine gives it.
