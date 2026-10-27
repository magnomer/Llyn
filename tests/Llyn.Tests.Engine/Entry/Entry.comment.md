# Entry

These tests cover the entry from draft to store, each on a real temporary workspace.
They span the draft round trip, save, load, update, the delete cascade, entry queries and collocation links.
They stand apart from the Engine folder, which covers tenures, vistas, liveries and workspace moves.
Most run through the engine relays, and the store and query cases call the archives directly.

## `TCollocationSense.cs`

It covers the links from a collocation to the meanings of its entry, in the engine.
Every case starts from one entry with a parent meaning and two children, a second top-level meaning, and one collocation.
The collocation links the second top-level meaning first and the first child second.
So its home is the second top-level meaning, and its further link sits one level down.
The first link stays first, because it is the home.
Stored links name the stored meanings in the order saved.
Applying the shared entry as content keeps both links in order against the draft's own meaning ids.
After the commit, the stored links read as the paths 2 and 1.1.
Dropping the home meaning keeps the stale link in the collocation.
But the save keeps only links to meanings still stored.
The child link is then the home, lifted to its top-level parent when nesting is off.
Reordering the links changes nothing else, yet records a collocation update in the revision.
A meaning one level down is a valid link target, and a new link goes last.
Dropping the home link leaves the further link alone.
Removing a parent meaning in the draft drops the links to its children too.
Links to meanings outside the removed branch stay.
Moving a linked child to the top level keeps every link in order, since a move is no removal.
A typed expression sent to a meaning one level down is refused with the card reason.
An unspecified or blank expression sent to a meaning is no refusal, and the meaning stays as it was.
An exported, deleted and re-imported entry reads its links as the paths 2 and 1.1 against the new meanings.
The livery note keeps nested homes, so the collocation renders inside the child that is its home.
Its row is open and sits in a collocations block under the collocation heading.
The further link adds a stub line to the second top-level meaning.
With keys as their own text, the stub prints its bare key.
No collocation is unhomed, so the Other section is absent.
A stored fold on the collocation is saved with the collocation kind.
It writes its row as a closed disclosure that still names the phrase.
A collocation linked to no meaning writes an open row in a collocations block under the Other heading.
That block carries no collocation heading of its own, and no numbered card title stands in for any such row.
`TCollocationSenseCreate` builds the shared entry with meaning ids minted for the draft, so the links can name them.

## `TEntryDraft.cs`

It covers what the draft model can carry between the store and the app.
A save and a load are inverses, so anything a card holds must make the trip in both directions.
The tests are about that trip and not about any one archive.
Every later stage of the markup work leans on it.
A card row stating a frame and no sentence is real data and is stored as one.
It comes back naming no Example, and saving it again neither invents one nor drops the row.
Two cards quoting one sentence are two rows naming one Example, and saving writes one Example row.
The id the rows carry is what names the Example.
A sense and a collocation quote on identical terms, so the test uses one of each.
A Meaning nested three deep loads as the tree it is stored as.
Position is per sibling group, which is what the store's unique index counts.
Saving the loaded draft back leaves every parent and position as it was.
A root the draft drops while one of its children is kept under another root removes only that root.
The child keeps its row and its text, and the save must not depend on the store reading roots last.
A Collocation carrying a card inside it is refused rather than saved with the card dropped.
Only a Meaning nests, and the format cannot write a nested Collocation either.
The fields the card draft used to collapse or drop make the trip whole.
Those are a Situation's three fields, a Register's name, a card's Translations, and a video's span.
No control on the card shows most of them, so the trip is the only thing keeping them.
A load followed by a save with no edit leaves every stored row exactly as it was.
The entry used carries one of everything a card can hold, including a nested Meaning.
Row identity counts and not merely row content, because an id is what other rows point at.
Bookkeeping the format deliberately does not carry is left out of the comparison.

## `TEntryDraftQuery`

It lists every table a card's content reaches, read whole rather than counted.
A count would pass while a column quietly changed under it.
The entry row is read by its named columns, which leaves out its stamps and other bookkeeping.
Its update stamp moves on every save by design, so a whole-row read would never match.

## `TEntryTableRead`

It reads the workspace as one comparable value, one table at a time.
Rows are sorted within their own table, so a renumber that changed nothing reads as nothing.
The query stands in the list beside its rows, so a difference names the table it is in.

## `TEntryLoading.cs`

It covers reading entries back, by listing and searching them by headword.
It also covers loading one whole entry into the draft it was saved from.
The load is the inverse of the save.
So what a test writes through `TEngineEntrySave` is exactly what comes back.
That is headword, language, pronunciation, note, every card with its definition, and the ordered Examples, Situations and Tags.
A sentence row carrying a frame and no Example survives a save and a load.
The frame is the card's own, so nothing about it may wait on a sentence being written first.
The row comes back naming no Example, because none was ever stored.
The cards are compared field by field.
Lists compare by reference, which is never true across a round trip through the store.
A pronunciation audio path is stored relative, so a moved or copied workspace keeps its audio.
The row is read through the archive rather than as SQL text, so the path separator stays the platform's.
Card titles are columns of the Meaning and Collocation rows, so they are on the rows before any load.
Rows are stored at the positions the lists held, so nothing shares a position.
The order is a fact of the store rather than of the read.
Three tags and two examples on one card all come back in the order written.
The collocation card references its own sets on the same terms.
A blank value in a list is not a row, and an empty field detaches.
The independent tables stay empty rather than collecting rows with no text.

## `TEntryQueryArchive.cs`

It covers the read-only entry queries over a real workspace database.
A find folds case beyond ASCII and trims the query before it matches.
SQLite's own lower() folds ASCII only, so a non-ASCII headword is the case the old matching could not make.
A query of whitespace alone lists everything, as an empty box does.
A headword lookup lists entries tied on headword by language spelling, then by id.
So the import window never lists its candidates in storage order alone.

## `TEntrySaving.cs`

It covers the one seam that writes an entry.
The whole input form goes in as a single engine call, and every row it produces reads back.
A blank headword is refused before anything is opened.
A failure part-way through leaves no entry row behind.
The save is one unit of work or none.
Meanings land in card order, renumbered from zero.
The collocation carries both its expression and the meaning that explains it.
The save is recorded as history, and the workspace row is moved onto the revision.
Each field became a row of its own that the card references, not a column on the card.
Saving the same text again writes a new Example row for each sentence.
A Situation is matched by its text, so the second entry shares the first entry's row.
A Tag reads back the same text on both entries.
The form always hands over one seeded Meaning card and one seeded Collocation card, both untouched.
It refuses to remove a list's last card.
An entry with no cards loads as cleanly as it saved.
A card of whitespace counts as blank, on the same terms a card field does.
The refusal names its reason with a localization key, so the shell can present it in the interface language.
The engine carries no sentence of its own.
The revision is written after the entry and everything under it.
Dropping the revision tables therefore fails the save at its last step.
The entry, its meaning and its note are already written inside the session, and none survives.
The engine is built first, or creating it would restore the table.

## `TEntryStore.cs`

It covers what an Entry owns and what it must not take with it.
That is the cascade that removes everything beneath a deleted Entry.
It is also the refusal to report success for an update that reached no row.
An Example was referenced, never owned, so it outlives the Entry that pointed at it.

## `TEntryUpdating.cs`

It covers the seam that changes a stored entry.
The entry keeps its identity while its cards are reconciled to the draft.
The revision says what actually moved.
An entry that is no longer there is refused.
An update that fails part-way leaves the stored entry exactly as it was.
The update stamp moves only when the draft differs from what is stored.
A change to any card alone moves it, because the stamp belongs to the whole entry.
The shell hands back the loaded draft with its middle card dropped and its first card reworded.
It also carries a new card with no id.
The entry stays the same record, with the same id and creation stamp and a fresh modification stamp.
Cards the draft still named kept their rows, so anything pointing at them still points at the same Meaning.
The added card got a row of its own.
The workspace row moved onto the revision the update recorded.
Reordered Examples re-attach the rows already there rather than writing new ones.
A cleared Tag is detached from the card and stays in the tag table, since it is independent data.
A tag write naming a Tag row nobody stores is refused.
That failure is reached only after the entry row and the card text have been written.
A refused update records no history at all.

## Inline notes

### TEntryLoading.cs: Assert.Equal(["sword", "word"], engine.TEngineEntryFind("WORD"))

A partial headword narrows the list, case-insensitively, and matches anywhere in the headword.
The word "word" is inside "sword" too.
So a query that picks exactly one has to be one of its own.
An empty query lists everything, ordered by headword.

## Stamp

Commit: `9377e657b395`
- `TCollocationSense.cs` `3b07875a5a3cc53a`
- `TEntryDraft.cs` `e7076b12c7cffecf`
- `TEntryLoading.cs` `ec3766d4e8655267`
- `TEntryQueryArchive.cs` `424c68b2f7177a79`
- `TEntrySaving.cs` `67390bade7cf6ab7`
- `TEntryStore.cs` `de0802e0a187965c`
- `TEntryUpdating.cs` `c9b8ebd520115104`
