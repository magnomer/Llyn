# Entry

This folder holds the persistence ports for entries and everything an entry owns.
That covers entry records and queries, the etymology map, lacunae, folds and history.
Each port lists exactly what the engine asks of its storage, and nothing about how rows are kept.
Every adapter is a `{Base}Archive` in Infrastructure over the workspace database.
A test may answer from a dictionary, which is how the engine is proved to need no database.

## `LAncestorVault`

The persistence port for an entry's etymology map and for the two lists its boxes draw from.
`LAncestorRead` gives the boxes of one entry, and `LAncestorSave` replaces them all.
A save answers the boxes as stored, with the ids the store gave them.
The relation list and the tongue list are read with a usage count of the boxes that hold each value.
Deleting a relation or tongue answers whether a row was removed.
A builtin relation is never removed.

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
A Meaning and a Collocation may carry one id.
So a card fold is keyed by the card kind and the id.
Reflex openings belong to entries, while editor-box openings belong to entry-and-box pairs.
Series member openings belong to entry-and-series-key pairs, apart from the entry page's opening.
Absent opening marks mean closed.
Repeated opening is idempotent, and missing stored entries acquire no mark.

## `LFoldSave`

Repeated folding is idempotent, and missing stored cards of that kind acquire no mark.

## `LFoldDelete`

Unfolding removes view state without changing the card's lexical values.

## `LFoldRead`

One entry-scoped read covers folded Meanings and Collocations together, each as its kind and id.

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

Commit: `8c403d1df768`
- `LAncestorVault.cs` `51212f51c2784511`
- `LEntryQueryVault.cs` `ee319777555535de`
- `LEntryVault.cs` `1458c8742f54fbad`
- `LFoldVault.cs` `af788cd97ab069e3`
- `LLacunaVault.cs` `bc31ece851bb9230`
- `LRevisionVault.cs` `39a90a8856170c2c`
- `LTombstoneVault.cs` `c3f460d349ff9054`
