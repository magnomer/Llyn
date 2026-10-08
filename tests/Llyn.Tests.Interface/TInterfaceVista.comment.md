# TInterfaceVista.cs
Hash: `864aee31df21410e`

## `internal static partial class TInterface`

The relays for a vista, the engine's view state for one catalog tab.
The start and the row finds are relayed over the engine, and every change over the vista itself.
Each relay is transparent and carries no test logic of its own.
The engine no longer stores a filter or a mode.
So the engine start relay hands it an empty filter and the reading mode.
The posture start relay resolves both from what the posture stored, as the window does.
Both pass the blank flag through with the same default the engine gives it.
The xiaoyun relay takes an optional vista, so a test can pin the order the panel sets.
