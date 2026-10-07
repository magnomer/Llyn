# TCatalogReference.cs
Hash: `7792c9eb54f4676b`

## `public sealed class TCatalogReference`

Covers the Source catalog, meaning what a Source is named, what it is ordered by, and what it answers.
It covers the name a Source is shown under, taken from the first field it actually states.
It covers a Source naming itself nowhere, which is shown under its id rather than blank.
It covers year order, which puts an unstated year before every stated one.
It covers author order, which reads the first credit and keeps an uncredited Source apart.
It covers usage order, which counts the places citing the Source, busiest first.
It covers the query, over the four texts of the Source, its year and its credited names.
It covers the byline, `Author (Year)`.
It covers a query typed as a byline, which finds the work of that author in that year.
It covers minting a Source from a typed citation line, titled with that line alone.
It covers resolving a typed citation line to the Source it names, minting one only for an unknown line.
A blank line resolves to nothing and changes no Source.
The Source already cited keeps a byline other works share, and a credited title finds its own Source.
Every workspace starts with the Source titled Unknown, so each listing carries it among the rows the test made.
It covers equal usage, year and author, which go by name and then id.
It covers equal names, which ignore case and go by id.
So a citation offer never follows storage order.

## `private static LCatalogReference TCatalogReferenceBuild(long id, string title, string? year = null)`

One browsed Source row cited once, named by its title, uncredited, with an optional year.
Only name and id can tell such rows apart.
