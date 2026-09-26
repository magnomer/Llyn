# TInterfaceCatalog.cs

## `internal static partial class TInterface`

The relay for the browsing seam: the find call of each browsed kind, and the catalog vocabulary.
Each relay delegates to one engine or Core operation and returns what that operation returned.
The orderings and the matches under test are reached only here, so no test body calls them itself.
