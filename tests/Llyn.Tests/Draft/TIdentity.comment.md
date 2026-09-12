# TIdentity.cs

## `public sealed class TIdentity`

Covers the identity rule for drafts: every item is positive or negative, and only the engine mints.

## `public void DraftSave_TwoNewSentencesSharingText_MintsTwoIdsAndCommitStoresTwoRows()`

Two new sentences with one wording are two items, not one.
Each gets its own negative id on save and its own row on commit.
Matching by text would have collapsed them.

## `public void DraftSave_SentencePickingStoredExample_KeepsIdAndCommitReusesRow()`

An item the user picked from the database keeps its positive id through the save.
The commit reuses that row and mints nothing, so the map has no pair for it.

## `public void DraftCommit_EveryNegativeIdHeld_AppearsInTheOutcomeMap()`

Every negative id the saved draft held is a key of the map, and every value is a stored row.
The stored entry, loaded back, carries those same rows under those same ids.

## `public void DraftArchiveSave_ItemWithoutId_Refuses()`

The drafts folder will not write a draft whose item still carries zero.
The rule cannot be bypassed by writing the file directly.
