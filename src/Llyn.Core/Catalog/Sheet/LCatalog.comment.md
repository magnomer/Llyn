# LCatalog.cs
Hash: `d1fe2f8dab2a49ea`

## `public static class LCatalog`

The vocabulary every browsed catalog shares.
It names the orderings in the form they are written and read back in.
It also holds the one text test every catalog match is built from.
Both live here so no caller decides for itself what a choice is called or what counts as a match.

## `public const int LCatalogOfferLimit`

The most stored rows a typed field offers in its dropdown, so a short word never floods it.
Every chip field's offer keeps to it, so the dropdowns stay one length.

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

One text in the form two wordings are compared in.
That form has edge spaces gone and case lowered the invariant way.
It is the same fold the database's `lfold` helper applies.
A match made in memory therefore agrees with one made in a query.
Every place that asks whether two typed wordings name one row folds both sides with this.

## `public static (string LCatalogMarkLead, string LCatalogMarkText, string LCatalogMarkTail) LCatalogMarkFind(`

Splits a found text around the first place the typed word stands in it, ignoring case in the current culture.
A picker draws the middle piece in weight, so a reader sees why the row is offered.
The comparison reports how long the match ran.
A match under a culture's rules is not always as long as the word that found it.
A text the word does not stand in reads whole as its lead.

## `public static string LCatalogUsageFormat(int usage)`

The count an offered row shows for how often its stored row is already used.
A row nobody uses shows nothing, because an unused row needs no number to say so.
Every offer that carries a usage count reads it here, so the rows agree.

## `public static string LCatalogTallyFormat(int count, string realm, Func<string, string> localize)`

The sentence for how many places cite a record, one of three forms by count.
The realm chooses the keys, such as `Source`, `Example` or `Situation`.
Every tally and the vita word it here, so the wording lives once.

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
