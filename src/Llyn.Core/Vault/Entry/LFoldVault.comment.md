# LFoldVault.cs
Hash: `d3f0ea93d9525715`

## `public interface LFoldVault`

Card folds and reflex-list, editor-box and series-member openings share one persistence boundary.
`LFoldArchive` supplies the workspace database adapter.
Marks remain separate from lexical card storage and the disposable reflex cache.
Meanings and Collocations share card identity, so card folds need no kind parameter.
Reflex openings belong to entries, while editor-box openings belong to entry-and-box pairs.
Series member openings belong to entry-and-series-key pairs, apart from the entry page's opening.

## `void LFoldSave(long cardId);`

Repeated folding is idempotent, and missing stored cards acquire no mark.

## `void LFoldDelete(long cardId);`

Unfolding removes view state without changing the card's lexical values.

## `IReadOnlySet<long> LFoldRead(long entryId);`

One entry-scoped read covers folded Meanings and Collocations together.

## `void LFoldReflexSpread(long entryId, bool opened);`

"More readings" belongs to one entry, independently of its cached readings.
Repeated opening is idempotent, and missing stored entries acquire no mark.

## `bool LFoldReflexCheck(long entryId);`

Absent reflex-opening marks mean closed.

## `void LFoldBoxSpread(long entryId, LFoldBox box, bool opened);`

Each entry-and-box pair has independent state, separate from reflex opening.
Repeated opening is idempotent, and missing stored entries acquire no mark.

## `bool LFoldBoxCheck(long entryId, LFoldBox box);`

Absent box-opening marks mean closed.

## `void LFoldStemSpread(long entryId, string key, bool opened);`

The "More readings" fold of one member on one series page, keyed by its entry and the series key.
It never touches the entry page's reflex opening, so each view keeps its own fold.
Repeated opening is idempotent, and missing stored entries acquire no mark.

## `bool LFoldStemCheck(long entryId, string key);`

Absent member-opening marks mean closed.
