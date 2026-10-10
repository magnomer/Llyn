# LFoldArchive.cs
Hash: `f066ca03fb72b2a4`

## `public sealed class LFoldArchive : LFoldVault`

Fold tables keep view-state marks separate from lexical rows.
Each card or entry has at most one mark per table, except one per series key in `stem_fold`.
Foreign-key cascades remove marks with their owning card or entry.
Meaning folds use `sense_fold`, while Collocation folds use `collocation_fold`.
Reflex openings use `reflex_fold`, and box openings use `fanqie_fold` or `script_fold`.
Series member openings use `stem_fold`, keyed by entry and series key.
The clerk owns nonpositive-id filtering, so this adapter takes ids as supplied.

## `public LFoldArchive(LDatabase database)`

All fold operations use the supplied workspace database.

## `public void LFoldSave(long cardId)`

Selecting each parent card before insertion avoids missing-parent foreign-key failures.
Conflict handling makes repeated folding idempotent.
Shared card identity lets only the owning card table contribute a mark.

## `public void LFoldDelete(long cardId)`

Both card-fold tables accept the same id, so callers need not supply the card kind.
Lexical card rows remain untouched.

## `public IReadOnlySet<long> LFoldRead(long entryId)`

One query combines both card kinds into an entry-scoped set.
The Meaning join also includes sub-meanings carrying that entry parent.

## `public void LFoldReflexSpread(long entryId, bool opened)`

A row means opened, while no row means closed.
Parent selection prevents missing entries from acquiring marks.
Conflict handling makes repeated opening idempotent.

## `public bool LFoldReflexCheck(long entryId)`

Only a stored reflex-opening mark answers true.

## `public void LFoldBoxSpread(long entryId, LFoldBox box, bool opened)`

Each box owns its table, keeping its entry-scoped state independent of other boxes.
A row means opened, while no row means closed.
Parent selection and conflict handling preserve missing-entry safety and idempotence.

## `public bool LFoldBoxCheck(long entryId, LFoldBox box)`

Only a mark in the selected box table answers true.

## `public void LFoldStemSpread(long entryId, string key, bool opened)`

A row in `stem_fold` for the entry and series key means opened, while no row means closed.
Parent selection and conflict handling preserve missing-entry safety and idempotence.

## `public bool LFoldStemCheck(long entryId, string key)`

Only a mark for that entry and series key answers true.

## `private static string LFoldTableSelect(LFoldBox box)`

A fixed enum-to-table mapping keeps caller-supplied text out of SQL identifiers.
Unknown boxes are rejected instead of naming an arbitrary table.
