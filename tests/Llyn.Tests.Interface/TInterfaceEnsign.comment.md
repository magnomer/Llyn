# TInterfaceEnsign.cs
Hash: `e7d5890625efa881`

## `internal static class TInterfaceEnsign`

The relay for the shared flag-fill ordering rule, so a test drives it over a fake settings port.
The relay is transparent and carries no test logic of its own.

## `internal static Task<CEnsignSheet<TEnsignKind>> TCatalogEnsignLoad<TEnsignKind>(LSettingsPort settings, Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store, Func<TEnsignKind> read)`

Relays `CCatalog.LCatalogEnsignLoad` with the port, the store and the read a test hands in.
