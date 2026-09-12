# TIdentity.cs

## `public sealed class TIdentity`

Covers the identity rule for drafts: every item is positive or negative, and only the engine mints.

## `public void RequestApply_TwoNewSentencesSharingText_MintsTwoIdsAndCommitStoresTwoRows()`

Two new sentences with one wording are two items, not one.
Each gets its own negative id when its text is requested and its own row on commit.
Matching by text would have collapsed them.

## `public void RequestApply_SentencePickingStoredExample_KeepsIdAndCommitReusesRow()`

An item the user picked from the database keeps its positive id through the save.
The commit reuses that row and mints nothing, so the map has no pair for it.

## `public void DraftCommit_EveryNegativeIdHeld_AppearsInTheOutcomeMap()`

Every negative id the saved draft held is a key of the map, and every value is a stored row.
The stored entry, loaded back, carries those same rows under those same ids.

## `public void DraftCommit_LinkedRowGone_Refuses()`

A draft naming a stored row by id, when that row has since been deleted, is refused rather than rebound.
The old path made a fresh row from the text the chip still showed, and the card silently pointed somewhere new.
The refusal names the link, nothing is written, and the draft file stays for the user to mend.

## `public void RequestApply_WrittenNameStoredAlready_PicksTheStoredRow()`

A wording typed into a chip is looked up by the engine before it is added.
A Situation or Register the workspace already holds under that wording is picked by id, case and edge spaces folded.
So the form and any other client of the engine get one row for one wording, and the shelf grows no twins.
A wording nothing matches is minted as new, and a later draft typing it finds the row the first one made.

## `public void DraftArchiveSave_ItemWithoutId_Refuses()`

The drafts folder will not write a draft whose item still carries zero.
The rule cannot be bypassed by writing the file directly.
