# TInterfaceEnsign.cs
Hash: `9cba746c999a7ec7`

## `internal static class TInterfaceEnsign`

The relay for the shared flag-fill ordering rule, so a test drives it over a fake settings port.
The relay is transparent and carries no test logic of its own.

## `internal static Task<CEnsignSheet<TEnsignKind>> TCatalogEnsignLoad<TEnsignKind>(CEnvoy envoy, LSettingsPort settings, string key, Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store, Func<TEnsignKind> read)`

Relays `CCatalog.LCatalogEnsignLoad` with the envoy, the port, the key, the store and the read a test hands in.
