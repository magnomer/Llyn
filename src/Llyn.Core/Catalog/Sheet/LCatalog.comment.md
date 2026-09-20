# LCatalog.cs

## `public static class LCatalog`

The vocabulary every browsed catalog shares.
It names the orderings in the form they are written and read back in.
It also holds the one text test every catalog match is built from.
Both live here so no caller decides for itself what a choice is called or what counts as a match.

## `public static string LCatalogOrderFormat(LCatalogOrder order)`

The text form of one ordering, which is what a stored choice is written as.
The text is the word the panel tags its dropdown row with, so one form serves both.

## `public static LCatalogOrder LCatalogOrderParse(string? text, LCatalogOrder fallback)`

Reads an ordering back from its text, falling back where the text names none.
A panel opens on its own fallback, because catalogs do not share a default ordering.
Text the set does not know is never an error, because a stored choice can outlive the ordering it named.

## `private const char LCatalogFilterSeparator`

The character a stored filter joins its hidden languages with.
A language folder name never carries one, so the join reads back whole.

## `public static string LCatalogFilterFormat(LCatalogFilter filter)`

The text form of one language filter, which is what a stored choice is written as.
An empty filter writes an empty text.

## `public static LCatalogFilter LCatalogFilterParse(string? text)`

Reads a language filter back from its text.
Empty or missing text is the filter that hides nothing.
Stray spaces and empty names are dropped, so a hand-edited row still reads.

## `public static string LCatalogTextNormalize(string? text)`

One text in the form two wordings are compared in: edge spaces gone and case lowered the invariant way.
It is the same fold the database's `lfold` helper applies.
A match made in memory therefore agrees with one made in a query.
Every place that asks whether two typed wordings name one row folds both sides with this.

## `public static bool LCatalogTextMatch(string? text, string query)`

Whether one text answers the query, both folded the same way `LCatalogTextNormalize` folds.
A query without a wildcard is read as a contains, so a partial wording still finds its rows.
A query with a wildcard is read against the whole text.
So `cat*` finds what starts with cat and `*cat` finds what ends with it.
The star stands for any run of characters, including none.
The question mark stands for exactly one character.
A user who wants a contains with wildcards writes a star at both ends.
An empty query answers everything, because a browsing panel with an empty box lists the whole catalog.
Text that is empty answers nothing, so a field that was never written matches no query at all.
The database's `lmatch` helper calls this, so a store query and a panel query agree.

## `private static readonly char[] LCatalogTextWildcard`

The two characters a query may stand a wildcard on.
A query that holds neither is a plain contains.
