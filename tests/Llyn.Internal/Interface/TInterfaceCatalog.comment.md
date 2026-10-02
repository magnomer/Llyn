# TInterfaceCatalog.cs
Hash: `5e6bdac7f9362b14`

## `internal static partial class TInterface`

The relay for the browsing seam: the find call of each browsed kind, and the catalog vocabulary.
Each relay delegates to one engine or Core operation and returns what that operation returned.
The orderings and the matches under test are reached only here, so no test body calls them itself.

## `internal static LCatalogRegister TCatalogRegisterCreate(LRegister register, int usage)`

Builds one browsed Register row from a stored Register, so a test reads the row's ready answers.
