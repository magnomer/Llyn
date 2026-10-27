# Catalog

Tests of the browsing catalogs and the display names their rows carry.
They stand apart from other engine tests because they pin ordering, querying and display naming.
Every catalog orders ties fully, so rows stored out of order never surface in storage order.
Every workspace starts with the Source titled Unknown, so listings carry it among the test's own rows.

## `TCatalogAuthor.cs`

Covers the Author catalog and the oeuvre read under one Author.
Each row carries the Sources crediting the Author and the places citing those.
The citation count is worded, and blank while nothing cites the Author.
The query reads the name alone, because a name is all an Author carries.
The oeuvre is covered for a chosen Author, for nobody, and for no choice at all.
The oeuvre narrows by a hidden kind and by typed text.
Count orders list the busiest first, and name order also runs reversed.

## `TCatalogEntry.cs`

Headword order and its reverse are shared by the library and phonology panels.
Newest first and oldest first read the moment the entry was added.
The typed query is answered by the store, not by the panel.
The star and question wildcards anchor the query to the whole headword.
Equal headwords go by language and then entry id in either headword order.
Equal moments go by language, headword and then entry id.

## `TCatalogExample.cs`

Orders cover sentence, language and the name of the Source cited.
Usage order counts the places quoting the Example, and the worded count follows a copy carrying another usage.
The query covers the sentence, the translation and the cited name.
The cited name is matched rather than the id, because the id is never shown.
The names of the whole shelf of Sources are read once per catalog fill.
A shown entry's citation lines include child cards, and a gone Source is written as its id.
Equal usage or equal sources go by language, sentence and then id.
Equal sentences go by language and then id.

## `TCatalogFavorite.cs`

Headword order, its reverse, and grouping by the entry language are covered.
Mark order reads when the mark was made and never when the entry was written.
Grasp order puts the best known entry first and unrated entries by headword.
Equal headwords go by language and then entry id.
Ties under language and mark order go by language, headword and then entry id.

## `TCatalogNames.cs`

Duplicate headwords receive numbered display names in markup and incoming-use rows.
Each result preserves the stored headword, so display labels never alter domain text.
One headword in two languages keeps a bare display name in both rows.

## `TCatalogOrder.cs`

The named set of orderings is written out and read back.
A remembered choice has to survive that round trip.
Text the set does not know falls back rather than failing.
A stored choice can outlive the ordering it named, so the fallback is behaviour and not defence.

## `TCatalogPronunciation.cs`

Every phonology row carries the pronunciation stored for its entry.
The sound and headword orderings leave out an entry with nothing stored.
The pending ordering lists only such entries.
Under the respelling setting a row carries its respelling instead of its IPA.
The row carries its sound, because the panel shows one and orders by it.
Bracketed text is an empty pair of brackets where nothing is stored.
Equal sounds go by language, then headword, then entry id.
Equal headwords go by language and then entry id.

## `TCatalogReference.cs`

A Source is shown under the first field it actually states.
A Source naming itself nowhere is shown under its id rather than blank.
Year order puts an unstated year before every stated one.
Author order reads the first credit and keeps an uncredited Source apart.
Usage order counts the places citing the Source, busiest first.
The query covers the four texts of the Source, its year and its credited names.
The byline reads `Author (Year)`, and a query typed as a byline finds that author's work in that year.
A typed citation line mints a Source titled with that line alone.
Resolving a citation line mints a Source only for an unknown line.
A blank line resolves to nothing and changes no Source.
The Source already cited keeps a byline other works share, and a credited title finds its own Source.
Equal usage, year and author go by name and then id.
Equal names ignore case and go by id.
So a citation offer never follows storage order.

## `TCatalogSituation.cs`

Title order and kind order are covered.
Usage order counts the Meanings and Collocations referencing the Situation.
The query covers the title, the description and the kind.
Equal usage and equal kind go by title without case and then id.

## `TCatalogTag.cs`

The Tag catalog is ordered and matched over one text and nothing else.
Name order and its reverse are covered.
The query ignores case, because a Tag is read as a word.
A found text is split around the typed word, which a picker draws in weight.
The usage count an offered row shows is empty for a row nobody uses.
The tally sentence is worded in the realm it is given for none, one and many.

## Inline notes

### TCatalogSituation.cs: TCatalogDraftRead

The chips a form holds for stored Situations let the save pick them rather than mint new ones.

### TCatalogSituation.cs: TCatalogSituationBuild

Three rows of one kind and one usage are stored out of title order.
Two titles differ only in case.

## Stamp

Commit: `9377e657b395`
- `TCatalogAuthor.cs` `49e18bef9e0452bd`
- `TCatalogEntry.cs` `15d9370512b222d2`
- `TCatalogExample.cs` `a8035891d0e105aa`
- `TCatalogFavorite.cs` `6b5f02fb775f637c`
- `TCatalogNames.cs` `10dbf1361a7503a6`
- `TCatalogOrder.cs` `1f202c2a833f5fa0`
- `TCatalogPronunciation.cs` `51c3953ca4359d55`
- `TCatalogReference.cs` `c518649a38a3c6bf`
- `TCatalogSituation.cs` `2b1d3667214be533`
- `TCatalogTag.cs` `26e755b2ce5a4a72`
