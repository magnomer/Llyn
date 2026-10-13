# Kind

Kind holds one browsed-row record per kind of stored thing a catalog lists.
Each row carries the stored record plus the counts the ordering reads and the row shows.
Each kind also owns its orderings and its match, so no panel decides them for itself.
Every ordering ends in a tie rule, so storage order never decides a place.
The id comes last in each tie rule and only keeps the result stable.
Every row has a chosen flag.
It is true on the row the vista stands on, and false until the vista find fills it.

## `LCatalogAuthor`

One Author as a browsed row with the Sources crediting it and the places citing those.
An Author credited nowhere is still a row, because the workspace still holds it.
`LCatalogAuthorName` is the stored name until the vista find numbers twins apart, while sort and match read the stored name.
`LCatalogAuthorCount` is read off the count on every access, so a copied row never shows a stale count.
`LCatalogAuthorCreate` sums the citations of every Source crediting the Author.
A Source cited nowhere adds nothing, and an Author with no Sources counts zero on both.
`LCatalogAuthorSort` falls back to the name for orderings an Author does not answer to, including the two count orderings.
`LCatalogAuthorMatch` reads the name alone, because a name is all an Author carries.

## `LCatalogEntry`

The orderings the entry catalog is listed in.
The store already answers which entries match, so only the ordering is decided here.
`LCatalogEntrySort` falls back to the headword where the ordering is not one an entry answers to.
A headword is read as a word, so the comparison is the reader culture and not an ordinal one.
A timestamp an entry does not carry sorts as empty.
That puts an entry of unknown age last in a newest-first list.
The generic overload is the one tie rule for every list of rows that stand for entries.
Rows equal under the chosen ordering go by language, then headword, then entry id.
The store queries listing entries by headword add the same keys, so a list reads alike from either side.
Language and headword compare in the reader culture, ignoring case.

## `LCatalogExample`

One Example as a browsed row with the name of the Source it cites and its quotation count.
An Example citing nothing carries an empty name rather than an id nobody would recognise.
`LCatalogExampleName` is the sentence as a name, numbered apart from its twins by the vista find.
`LCatalogExampleText` is the same wording without the twin number.
`LCatalogExampleCount` is worded on every read, so a copy with another usage shows its own count.
Ordering by Source orders by the resolved name rather than the id, because the id is never shown.
Rows equal under any ordering go by language, then sentence, then id.
`LCatalogExampleMatch` covers the sentence, the text of every Gloss it carries, and its Source name.

## `LCatalogFavorite`

An entry the user has marked, carried with the moment it was marked.
Favorite status is no lexical object, so the entry keeps its identity whether it is marked or not.
The stamp exists so the catalog can order by when the mark was made, never when the entry was written.
The store already answers which marked entries match, so only the ordering is decided here.
Ordering by grasp puts the best known entries first and breaks ties by headword.
Rows still equal go by the entry tie rule in `LCatalogEntry`.

## `LCatalogPronunciation`

One entry as a phonology row with the primary pronunciation stored for it.
An entry with nothing stored carries an empty sound.
Only the pending ordering lists such rows, and it lists nothing else.
The sound is the respelling where the clerk was told to respell and one is stored, the IPA otherwise.
`LCatalogPronunciationText` is the sound in square brackets, so a pending row still shows an empty slot.
The pending ordering goes by headword, since its rows all hold an empty sound.
A sound is compared ordinally, because an IPA string is a sequence of symbols and not a word.
Rows still equal go by the entry tie rule in `LCatalogEntry`.

## `LCatalogReference`

One Source as a browsed row with its shown name, byline, credits and citation count.
The name and the byline are derived once here rather than in whatever is drawing the row.
`LCatalogReferenceCreate` gives a Source with no credits an empty list rather than nothing, so no reader tests for null.
`LCatalogReferenceWriter` joins the credited authors into one line for Conduct, or null when none is credited.
Ordering by year puts the states in order first, so an unstated year is never read as an early one.
Ordering by author reads the first credit, because the credit order belongs to the Source.
A Source with no credits is ordered by the authorship state it states.
That is not the same as being credited.
Rows equal under year, author or name go by `LCatalog.LCatalogNameSort`, the name then the id.
Rows equal under usage go by `LCatalog.LCatalogUsageSort`, which a citation field's offer reads too.
`LCatalogReferenceMatch` breaks the query into terms at blanks, commas and parentheses, and every term must hit some field.
So a citation typed as it is shown, `Darwin (1859)`, finds the work of that author in that year.
Only a field standing specified carries text, so an unknown field matches nothing.

## `LCatalogRegister`

One Register as a browsed row with how many Meanings and Collocations are marked with it.
`LCatalogRegisterIcons` names the Registers that wear an icon of their own, keyed without regard to case.
The icon follows the name alone, because a Register belongs to no language.
`LCatalogRegisterIcon` reads the name without outer spaces, and a name with no icon wears the plain register icon.
A Register nothing is marked with counts as zero, which puts it last under the usage ordering.
Equal counts fall back to the name through `LCatalog.LCatalogUsageSort`.
This is the one owner of the Register order, for the tenor panel and for a card's offer alike.
`LCatalogRegisterMatch` reads the name, and an empty query is answered by every row.

## `LCatalogSituation`

One Situation as a browsed row with how many Meanings and Collocations reference it.
`LCatalogSituationName` is the title as shown, numbered apart from twins, while sort and match read the stored title.
`LCatalogSituationKind` is the kind as shown, empty when none is stated, with an unknown kind worded by the panel.
`LCatalogSituationCount` is worded through the one shared rule, blank for an unused Situation.
A Situation with no stated kind is ordered as empty, which puts it before every stated kind.
Rows equal under the kind go by title, then id.
Rows equal under usage go by `LCatalog.LCatalogUsageSort`, which a typed field's offer reads too.
`LCatalogSituationMatch` covers the title, the description and the kind.

## `LCatalogTag`

One Tag as a browsed row, and the home of the tag catalog's orderings and match.
A tag is a word and nothing else, so both answers are about that one text.
`LCatalogTagSort` is ascending under every ordering but the reverse one.
A tag is read as a word, so the comparison is the reader culture and not an ordinal one.

## Stamp

Commit: `c2f335c331fe`
- `LCatalogAuthor.cs` `4ce4794a49e22664`
- `LCatalogEntry.cs` `40cd70240c65e6bc`
- `LCatalogExample.cs` `c086e3b8f39c8ef8`
- `LCatalogFavorite.cs` `e4b9348359539796`
- `LCatalogPronunciation.cs` `95f3f55ce3adfe3d`
- `LCatalogReference.cs` `cb83fbfb6db8a399`
- `LCatalogRegister.cs` `6738e8767039f0da`
- `LCatalogSituation.cs` `110dfe7ad39010ee`
- `LCatalogTag.cs` `17fde97cf17d2843`
