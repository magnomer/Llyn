# TCatalogEntry.cs
Hash: `74f343bbbae11fce`

## `public sealed class TCatalogEntry`

Covers the orderings the entry catalog is listed in.
It covers headword order and its reverse, which the library and phonology panels share.
It covers newest first and oldest first, which read the moment the entry was added.
It covers the typed query, which the store answers rather than the panel.
It covers the star and question wildcards, which anchor the query to the whole headword.
It covers equal headwords, which go by language and then entry id in either headword order.
It covers equal moments, which go by language, headword and then entry id.

