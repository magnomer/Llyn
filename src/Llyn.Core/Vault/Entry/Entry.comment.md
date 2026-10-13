# Entry

This folder holds the persistence ports for entries and everything an entry owns.
That covers entry records and queries, etymology, lacunae, folds and history.
Each port lists exactly what the engine asks of its storage, and nothing about how rows are kept.
Every adapter is a `{Base}Archive` in Infrastructure over the workspace database.
A test may answer from a dictionary, which is how the engine is proved to need no database.

## `LEntryVault`

The persistence port for entries and their two owned child structures, written forms and parts of speech.
The list and count queries live on `LEntryQueryVault`, so this port keeps the record lifecycle only.

## `LEntryCreate`

Stores an entry with a fresh id and timestamps and its ordered forms and speeches.
It returns the stored entry with its id and timestamps filled in.

## `LEntryRead`

Reads the entry by id, or null when no entry has that id.

## `LEntryLoad`

Assembles the whole editable content of the entry, or null when no entry has that id.

## `LEntryFormRead`

Reads the entry's written forms, ordered by position.

## `LEntrySpeechRead`

Reads the entry's part-of-speech assignments, ordered by position.

## `LEntryUpdate`

Updates the headword and metadata of the entry identified by the entry's id.

## `LEntryFormSet`

Replaces the entry's written forms in list order.
`LEntrySpeechSet` does the same for the part-of-speech assignments.

## `LEntryEpithetRead`

Reads the entry's epithet, or an empty string when none is stored.

## `LEntryDelete`

Removes the entry and everything it owns.

## `LEntryQueryVault`

The read-only query port over entries.
It answers lists, scans and counts, and never changes a row.
It stands apart from `LEntryVault` so a reader holds no write it does not need.
The find and scan methods match headwords, tags, registers, situations, examples and references.
`LEntryHeadwordScan` finds the entries of a language whose headword occurs in a text.
`LEntryTagFind`, `LEntryRegisterFind` and `LEntrySituationFind` match through a card of the entry.
`LEntryExampleFind` matches through a sentence of the entry.
`LEntryFind` answers every entry for an empty query.

## `LEntryCountRead`

Counts every entry in the workspace.

## `LEtymologyVault`

The persistence port for the etymology of an entry, in both of its shapes.
One shape is a narrative with spans, and the other is a list of direct links.

## `LEtymologyRead`

Reads the entry's narrative, or null when it keeps none.
The spans come back in offset order, as every Mention read does.

## `LEtymologyEtymonRead`

Reads the entry's direct links in position order, empty when it has none.

## `LEtymologySave`

Makes the given etymology the whole narrative of the entry, spans and all.
A null or blank narrative deletes the stored one and returns null.
The stored spans are written fresh each time, so their ids do not survive the save.
It throws when a span falls outside the text, overlaps another, or names no Entry.
Otherwise it returns the stored narrative with its ids filled in.

## `LEtymologyEtymonSet`

Makes the target ids the whole list of the entry's direct links, in the order given.
A repeated target, a nonpositive id and the entry itself are dropped rather than refused.
It returns the stored links in their stored order.

## `LLacunaVault`

The persistence port for the Lacuna rows, the paradigm slots a fetch found missing.

## `LLacunaRead`

Reads the entry's rows in the order they were written.
A cell key identifies a missed slot even without a morphology id.
A row with neither identifier names no slot.

## `LLacunaSave`

Replaces the entry's rows with one per given lacuna in one transaction.
Each row keeps its morphology id and its cell key.
An empty list leaves the entry with no rows at all.
Every row is stamped with the same fetch time.

## `LLacunaDelete`

Drops the entry's rows, so a hand-edited entry is asked afresh.

## `LFoldVault`

Card folds and reflex-list, editor-box and series-member openings share one persistence boundary.
`LFoldArchive` supplies the workspace database adapter.
Marks remain separate from lexical card storage and the disposable reflex cache.
Meanings and Collocations share card identity, so card folds need no kind parameter.
Reflex openings belong to entries, while editor-box openings belong to entry-and-box pairs.
Series member openings belong to entry-and-series-key pairs, apart from the entry page's opening.
Absent opening marks mean closed.
Repeated opening is idempotent, and missing stored entries acquire no mark.

## `LFoldSave`

Repeated folding is idempotent, and missing stored cards acquire no mark.

## `LFoldDelete`

Unfolding removes view state without changing the card's lexical values.

## `LFoldRead`

One entry-scoped read covers folded Meanings and Collocations together.

## `LFoldReflexSpread`

"More readings" belongs to one entry, independently of its cached readings.
`LFoldReflexCheck` reads it back.

## `LFoldBoxSpread`

Each entry-and-box pair has independent state, separate from reflex opening.
`LFoldBoxCheck` reads it back.

## `LFoldStemSpread`

The "More readings" fold of one member on one series page, keyed by its entry and the series key.
It never touches the entry page's reflex opening, so each view keeps its own fold.
`LFoldStemCheck` reads it back.

## `LRevisionVault`

The persistence port for revisions, the recorded history of what changed in the workspace.

## `LRevisionRecord`

Records the changes as one new revision stamped with the current time.
It returns the stored revision with its id filled in.

## `LTombstoneVault`

The persistence port for tombstones, the records that an entry was deleted.

## `LTombstoneRecord`

Records that the entry was deleted under a revision and returns the stored tombstone.

## Stamp

Commit: `c2f335c331fe`
- `LEntryQueryVault.cs` `ee319777555535de`
- `LEntryVault.cs` `1458c8742f54fbad`
- `LEtymologyVault.cs` `5a5ef80155a96f0f`
- `LFoldVault.cs` `d3f0ea93d9525715`
- `LLacunaVault.cs` `bc31ece851bb9230`
- `LRevisionVault.cs` `39a90a8856170c2c`
- `LTombstoneVault.cs` `c3f460d349ff9054`
