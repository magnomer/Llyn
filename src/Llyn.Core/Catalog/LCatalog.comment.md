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

## `public static string LCatalogTextNormalize(string? text)`

One text in the form two wordings are compared in: edge spaces gone and case lowered the invariant way.
It is the same fold the database's `lfold` helper applies.
A match made in memory therefore agrees with one made in a query.
Every place that asks whether two typed wordings name one row folds both sides with this.

## `public static bool LCatalogTextMatch(string? text, string query)`

Whether one text answers the query, read as a contains under the reader culture.
An empty query answers everything, because a browsing panel with an empty box lists the whole catalog.
Text that is empty answers nothing, so a field that was never written matches no query at all.
