# TCatalogFavorite.cs
Hash: `6b5f02fb775f637c`

## `public sealed class TCatalogFavorite`

Covers the orderings the favorite catalog is listed in.
It covers headword order and its reverse, and grouping by the language of the entry.
It covers ordering by the mark, which reads when the mark was made and never when the entry was written.
It covers ordering by grasp, which puts the best known entry first and unrated entries by headword.
It covers equal headwords under headword order, which go by language and then entry id.
It covers ties under language and mark order, which go by language, headword and then entry id.

