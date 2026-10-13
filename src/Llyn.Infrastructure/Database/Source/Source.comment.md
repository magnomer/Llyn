# Source

This folder persists the independent records that cards and Examples cite or show.
They are Authors, bibliographic References (shown as Sources), Images and Videos.
None of them is owned by an Entry, Meaning or Collocation.
Each is created once with an opaque id and then referenced through association rows.
So attaching and detaching only write association rows and never touch the record or its other references.
The usage readers sit beside the archives because tracing who cites a record is a different responsibility from storing it.

## `LAuthorArchive`

Persists Authors, which no Reference owns.
An Author is referenced by any number of References through reference_author.
That row carries the position the Author takes on that Reference alone.
Renaming rewrites the visible name and never the id, so every Reference keeps pointing at the same Author.
`LAuthorDelete` refuses to run while any Reference still credits it.
Attaching an Author to a Reference belongs to `LReferenceArchive`, which owns the association rows.

## `LAuthorCreate`

Inserts an Author with a fresh opaque id, credited on no Reference until one attaches it.
The name may be empty, because section 6 of the format spec lets an author carry none.
An import writes such a row and the engine refuses one typed by hand.

## `LAuthorReferenceRead`

Reads the Authors a Reference credits, in the order that Reference gives them.
Another Reference crediting the same Authors may order them differently, because the order lives on the association row.
The parameterless overload reads the credits of every Reference at once.
So a catalog showing credits does not run a query per row.

## `LAuthorAllRead`

Reads every Author by name, with ties going by id, which only keeps the list stable.
The Sources panel offers them for crediting, so it asks for the whole shelf at once.

## `LAuthorUpdate`

Rewrites the name of one Author.
The id and every Reference crediting it are untouched, so a rename never changes where the Author appears.
It throws when no Author carries that id.

## `LAuthorAbsorb`

Folds one Author into another and deletes the dropped row.
Every Reference crediting the dropped Author credits the kept one afterwards, at the same position.
A Reference already crediting both keeps the kept credit where it stood and loses the duplicate.
The remaining credits of each touched Reference are renumbered, because the order is a unique index.
The kept Author is inserted past every live position first, so the renumbering never collides.
It throws when either id names no Author or both name the same one.
The whole fold is one transaction, so no Reference is left crediting a deleted Author.

## `LAuthorDelete`

Deletes an Author, with detach dropping every credit first.
Dropping a credit renumbers that Reference's remaining credits, because the order is a unique index.
Deleting an Author never deletes a Reference.

## `LAuthorUsage`

Reads the citing side of an Author, meaning which cards and Examples cite a Source the Author is credited on.
Nothing here creates, changes or deletes anything.
The trace is computed on read and stored nowhere.
No table links an Author to an Entry, so no stored row can contradict the chain.
Every hop is already indexed.
The hops are reference_author_member, example_reference, then the two card member indexes.
The return type is `LUsage`, which no write path accepts, so a trace cannot become an input.
An Author credited on no Source, or on a Source no Example cites, traces to nothing.
That is correct under the chain rather than a fault, and a tab renders it as not cited yet.

## `LAuthorUsageRead`

Reads the Meanings, Collocations and Examples citing any Source the Author is credited on.
A card names its own id, the Entry it belongs to, that Entry's headword and its title.
An Example names its sentence and its first Gloss, because an Example belongs to no Entry of its own.
The first Gloss is joined by position zero, and an Example without one reads as unspecified.
A card is listed once however many credited Examples it holds, because the row leads to the card.
Card rows sharing a headword go by language, then entry id, then place on the Entry.
Those are the keys `LCatalogEntry` breaks entry ties by, so storage order never decides a place.
Example rows go by sentence, then language, then example id, as `LReferenceUsage` lists them.

The count of an Author is the Example rows, which are the citations themselves.
The card rows are the way up to those citations rather than additions to them.
Counting cards and Examples together would count one citation twice, as `LReferenceUsage.comment.md` records.

## `LReferenceArchive`

Persists bibliographic References, which no Entry, Example or Author owns.
An Example cites at most one through its example.reference_ref column.
An Entry reaches a Reference only through the Examples its cards quote, and holds no citation of its own.
The citation is a pointer, so clearing it leaves the Reference standing.
`LReferenceDelete` refuses to run while any citation remains.

Every text field is stored as a state column beside its value column.
So "never filled in", "recorded as unknown" and "this value" stay three different facts.
The value column is written only when the state is specified.
The kind is written as a single word, because its own members already carry those three states.

The Authors a Reference credits are attached in order through reference_author.
Attaching and detaching write association rows only, and deleting the Reference never drops an Author.
The author order is a unique index.
So crediting and dropping a credit renumber the whole set they touch through `LDatabaseOrder`.
A caller names the index it wants and never has to find a free position or leave a gap behind.

## `LReferenceCreate`

Inserts a Reference with a fresh opaque id.
Each field is written as its state plus, for a specified field alone, its value.
The new Reference is cited by nothing and credits no Author until one is attached.

## `LReferenceAllRead`

Reads every Source by stored title.
Sources sharing a title, or stating none, go by id, which only keeps the list stable.

## `LReferenceUsageRead`

Answers through `LReferenceUsage` so the port has one adapter.
The parameterless overload gives the citation count of every cited Reference, and the id overload gives the citing cards.

## `LReferenceUpdate`

Rewrites every field of one Reference, states and values alike.
The id, the Authors credited and every citation pointing at it are untouched.
So an update never changes where the Reference appears or in what order.
It throws when no Reference carries that id.

## `LReferenceDelete`

Deletes a Reference, with detach clearing every citation first.
The detaching, the count and the delete stand in one session, because between any two steps the answer can change.
The guard and the delete share one transaction, so nothing can cite the Reference between them.
An Example is not deleted with the Reference, so its citation is cleared back to Unspecified.
Only the links between them and this Reference go.

## `LReferenceAuthorAttach`

Credits an existing Author on this Reference at a position in the Reference's own author order.
The Author row is untouched and stays available to every other Reference.
`LReferenceAuthorDetach` removes the credit and leaves the Author and its other credits.

## `LReferenceLinkAttach`

The attach and detach share one implementation parameterised by table and columns, so another association table can reuse it.
Every identifier is a store-owned literal chosen by the public methods, never caller input.
Every value still travels as a parameter.
The row goes in beyond the end of the set and the set is then renumbered around it.
So the requested index is honoured, and an occupied position is no longer a unique-index failure.

## `LReferenceUsage`

Reads the citing side of a Source, meaning which cards and Examples cite it and how many.
Nothing here creates, changes or deletes a Source.
A Situation is not counted, because a Situation is written rather than quoted and carries no Source column.
Only an Example cites a Source, through its own reference_ref column.
A Meaning or a Collocation is reported as citing because it holds such an Example.
It is never reported so because a row says so.
So a card arm is a join back up the chain, and detaching is clearing two columns on the Example.

## `LReferenceUsageRead` in `LReferenceUsage`

The map form counts the citations of every Source at once, for the catalog, the ordering and the delete.
A Source nothing cites is absent from the map rather than present at zero.
The unit is the citing Example, which is the only thing that cites at all.
It is not the number of rows the id overload returns.
That list also names each citing card as the way up to the citation.
Counting cards and Examples together would count one citation twice, from two directions.
So the badge showing this number says what it counts, through Source.Tally.

The id form reads the citing Meanings, Collocations and Examples of one Source.
Each comes with the id the row leads to.
A card names its own id, its Entry, that Entry's headword, its title and the definition or expression behind it.
An Example names its sentence and its first Gloss, because an Example belongs to no Entry of its own.
The first Gloss is joined by position zero, and an Example without one reads as unspecified.
A card holding two Examples of one Source is listed once.
The row leads to the card rather than the citation.
Card rows sharing a headword go by language, then entry id, then place on the Entry.
Those are the keys `LCatalogEntry` breaks entry ties by, so storage order never decides a place.
Example rows go by sentence, then language, then example id.
The sentence stands where a card's headword stands, and the id comes last only to keep the list stable.

## `LReferenceUsageClear`

Drops every citation of one Source.
An Example is not deleted with the Source, so its citation is cleared back to Unspecified.
The cards citing through those Examples lose the Source with them, because they never held it themselves.
The static count overload reads inside a session the caller holds, so `LReferenceDelete` counts after detaching in one session.

## `LImageArchive`

Persists Images, which no Entry, Meaning or Collocation owns.
An Image is referenced by Meanings, Collocations and Situations through the association tables.
Each association carries the position the Image takes for that referrer alone.
Updating rewrites the location and never the id.
Every referrer sees a location change at once, because a reference points at the row and never at its text.

Each association table is held as one `LDatabaseLink`, which renumbers a referrer's whole set on every write.
A caller names the index it wants and never has to find a free position or leave a gap behind.

## `LImageCreate`

Inserts an Image with a fresh opaque id, referenced by nothing until it is attached.

## `LImageMeaningRead`

Reads the Images a Meaning references, in the order that Meaning gives them.
The Collocation and Situation readers do the same for their referrers.

## `LImageMeaningAttach`

References the Image from a Meaning at the index named, renumbering that Meaning's other references around it.
The Collocation and Situation attaches follow the same rule.

## `LImageMeaningDetach`

Drops a Meaning's reference and closes the gap it leaves.
The Image and every other reference stay.
The Collocation and Situation detaches follow the same rule.

## `LImageSituationScan`

Returns every situation-to-image link joined to its Image, ordered by parent and position, bucketed by parent.
The Situation store fills a whole list from it in one query rather than one per Situation.

## `LImageSituationClear`

Drops every Image link of one Situation and leaves the Image rows standing.
The Situation store calls it inside its delete, so the session nests into that one.

## `LVideoArchive`

Stores the independent Video and the references that reach it from a Meaning, a Collocation or a Situation.
It mirrors `LImageArchive` in behaviour, because a Video is kept exactly as an Image is.
It keeps one thing an Image has no use for, the span of the film worth watching.
The span is held as written and read as written.
A Video is owned by nothing, so a reference is a link and never containment.
The order a Video appears in lives on each reference, and every write renumbers that referrer's set.

## `LVideoCreate`

Returns the video with the id the store gave it, and the new Video is referenced by nothing yet.
An unreadable location or span is refused before any row is written.

## `LVideoUpdate`

Rewrites the location and span of one Video and never its id.
An id no row carries throws, so a lost row is never mistaken for a saved one.

## `LVideoMeaningAttach`

A Video the Meaning already references keeps its single link and moves to the position.
A position outside the set clamps to its nearest end.
The Collocation and Situation attaches follow the same rule.

## `LVideoMeaningDetach`

Detaching a Video the Meaning does not reference is no error.
The Video row stays even when no referrer remains.
The Collocation and Situation detaches follow the same rule.

## `LVideoSituationScan`

Returns every situation-to-video link joined to its Video, ordered by parent and position, bucketed by parent.
It carries the span beside the location.
The Situation store fills a whole list from it in one query rather than one per Situation.

## `LVideoSituationClear`

Drops every Video link of one Situation and leaves the Video rows standing.
The Situation store calls it inside its delete, so the session nests into that one.

## Inline notes

### LReferenceArchive.cs: using (SqliteCommand command = connection.CreateCommand())

reference_author cascades from reference, so the author links go with the row and the Authors stay.

## Stamp

Commit: `c2f335c331fe`
- `LAuthorArchive.cs` `959d148d44860a7a`
- `LAuthorUsage.cs` `8d37052136e2cf56`
- `LImageArchive.cs` `fce87a0f29147ecf`
- `LReferenceArchive.cs` `ad95ee16f7fdc592`
- `LReferenceUsage.cs` `d68eeb8d554402b3`
- `LVideoArchive.cs` `85ebdcb8704b775f`
