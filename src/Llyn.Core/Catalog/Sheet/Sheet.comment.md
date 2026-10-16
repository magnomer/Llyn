# Sheet

Sheet holds the vocabulary and the shared rules every browsed catalog uses.
It stands apart from `Kind`, which holds the per-kind rows, by owning what no single kind may decide.
That covers the ordering names, the language filter, the text match and the twin numbering.
It also covers the read sheets handed to a panel.

## `LCatalog`

The vocabulary every browsed catalog shares.
It names the orderings in the form they are written and read back in.
It also holds the one text test every catalog match is built from.
Both live here so no caller decides for itself what a choice is called or what counts as a match.

## `LCatalogOfferLimit`

The most stored rows a typed field offers in its dropdown, so a short word never floods it.
Every chip field's offer keeps to it, so the dropdowns stay one length.

## `LCatalogOrderFormat`

The text form of one ordering is what a stored choice is written as.
It is the word the panel tags its dropdown row with, so one form serves both.

## `LCatalogOrderParse`

Reads an ordering back from its text, falling back where the text names none.
A panel opens on its own fallback, because catalogs do not share a default ordering.
Unknown text is never an error, because a stored choice can outlive the ordering it named.

## `LCatalogFilterSeparator`

The character a stored filter joins its hidden languages with.
A language folder name never carries one, so the join reads back whole.

## `LCatalogFilterFormat`

An empty filter writes an empty text.
`LCatalogFilterParse` reads empty or missing text as the filter that hides nothing.
Stray spaces and empty names are dropped, so a hand-edited row still reads.

## `LCatalogTextNormalize`

The form two wordings are compared in has edge spaces gone and case lowered the invariant way.
The database's `lfold` helper lowers case the same way.
It trims nothing.
So the case fold agrees between a match made in memory and one made in a query.
Every place that asks whether two typed wordings name one row folds both sides with this.

## `LCatalogMarkFind`

Splits a found text around the first place the typed word stands in it, ignoring case in the current culture.
A picker draws the middle piece in weight, so a reader sees why the row is offered.
The comparison reports how long the match ran.
A match under a culture's rules can differ in length from the word.
A text the word does not stand in reads whole as its lead.

## `LCatalogUsageFormat`

The count an offered row shows for how often its stored row is already used.
A row nobody uses shows nothing, because an unused row needs no number to say so.
Every offer that carries a usage count reads it here, so the rows agree.

## `LCatalogUsageSort`

The one usage ordering for every catalog whose rows carry a usage count and a name.
The most used row comes first.
Equal counts go by `LCatalogNameSort`, so the two orderings never drift apart.
The Register, Situation and Source catalogs read it, and so do the offers built on them.

## `LCatalogNameSort`

The one tie for catalog rows that carry a name and an id, after whatever keys the caller ordered by.
Rows still equal go by name in the reader culture, ignoring case, then by id.
The id comes last and only keeps the result stable, so storage order never decides a place.
`LCatalogUsageSort` ends with it, and so does every ordering of the Source catalog.

## `LCatalogTallyFormat`

The sentence for how many places cite a record, one of three forms by count.
The realm chooses the keys, such as `Source`, `Example` or `Situation`.
Every tally and the vita word it here, so the wording lives once.
It forms the stem `{realm}.Usage` and hands it to `LCatalogCountFormat`.

## `LCatalogCountFormat`

The sentence for a count under any key stem, one of three forms by count.
The stem takes `None`, `One` and `Many` as its keys, and the many form leads with the count.
The library list uses it with `Library.Shown`, so its count line shares the one rule.

## `LCatalogTextMatch`

Both text and query are lowercased the invariant way, as `LCatalogTextNormalize` does.
Neither is trimmed.
A query without a wildcard is read as a contains, so a partial wording still finds its rows.
A query with a wildcard is read against the whole text.
So `cat*` finds what starts with cat and `*cat` finds what ends with it.
The star stands for any run of characters, including none, and the question mark for exactly one.
A user who wants a contains with wildcards writes a star at both ends.
An empty query answers everything, because a browsing panel with an empty box lists the whole catalog.
Empty text answers nothing, so a field that was never written matches no query at all.
The database's `lmatch` helper calls this, so a store query and a panel query agree.

## `LCatalogFilter`

The languages a browsing panel keeps out of its list.
It records what is hidden rather than what is shown.
A language pack added later is therefore shown until the user hides it.
An empty record hides nothing, which is how every panel opens.
`LCatalogFilterEmpty` is shared so no panel builds its own.
`LCatalogFilterCreate` returns it when nothing is hidden, so the vista skips an equal filter and raises no notice.
`LCatalogFilterActive` marks the panel's button.
`LCatalogFilterMatch` keeps a row with no language, because nothing was hidden by that name.
Names are compared without case, as the language folders are.
`LCatalogFilterApply` hands the same list back untouched for an empty filter.

## `LCatalogOrder`

Every ordering a browsed catalog can be listed in.
A browsing panel picks one of these rather than inventing a word of its own.
One member means one ordering wherever it is asked for, so two panels ordering by usage agree.
A panel offers only the members its own kind of record can be ordered by.
The stored form of a member is its text, so a remembered choice survives a restart.
`LCatalogOrderYear` lists unstated years first.
`LCatalogOrderAuthor` lists uncredited sources first.
`LCatalogOrderRecent` and `LCatalogOrderMarked` list newest first.
`LCatalogOrderUsage` and `LCatalogOrderWork` list the most cited or most credited first.
`LCatalogOrderSound` lists only entries carrying a pronunciation, by sound.
`LCatalogOrderPending` lists only entries carrying none.
`LCatalogOrderGrasp` lists the best known entries first.

## `LFellow`

One co-author of a read Author as the engine hands it to the vita, counted and ordered.
The vita copies it into its list and decides nothing.
`LFellowCount` is read off the count on every access, because the author clerk raises the count on a copy.

## `LTwin`

The numbering that tells apart catalog rows sharing one displayed name.
Two entries may carry the same headword, and two cards the same title.
The numbering is display text only, so stored names stay untouched.
It lives in Core so the engine can number the rows it returns for a view.

## `LTwinNameApply`

Appends `(1)`, `(2)` and so on to names more than one row carries.
Only rows of one group count as twins, so entries of two languages never number each other.
Rows with no group to tell apart pass an empty one, which makes every row one group.
A name held by a single row is written unchanged.
Numbers follow the list order, so the first row shown takes `(1)`.
The ranked overload numbers by ascending rank instead, so a row keeps its number under any sort or filter.
Entry lists pass the entry id, which makes the older entry `(1)` in every view.
Rows tied on rank fall back to list order.
Names and groups are compared byte for byte, so case and spacing separate two names.

## `LVistaRow`

One row of a catalog list as the engine hands it to a panel, ready to show.
The engine has already filtered, sorted, numbered and marked it.
The panel copies it into its list and decides nothing.
The headword is stored untouched, and the name is the displayed one, numbered when another listed row shares it.
The epithet is empty when the workspace shows none.

## `LVita`

The read sheet of one Author with every text already decided, so the vita page writes and never branches.
The name falls back to the unnamed label when the stored name is blank, and the flag says so.
The work and citation sentences are composed from the counts the catalog row carries.
`LVitaCreate` given a missing row composes the sheet of nobody, so a fresh author draft still shows zero counts.
`LVitaWorkFormat` gives one of three forms by count.

## Stamp

Commit: `bd299c638b2c`
- `LCatalog.cs` `4e576dcda285294e`
- `LCatalogFilter.cs` `2a78cb399c5b8aea`
- `LCatalogOrder.cs` `d928c60059e57574`
- `LFellow.cs` `3a26e85092f16979`
- `LTwin.cs` `5f5a23587a7557a9`
- `LVistaRow.cs` `a8c21bb590b88b0f`
- `LVita.cs` `287c01caa33e411b`
