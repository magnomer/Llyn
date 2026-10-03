# TIdentity.cs
Hash: `91d8f92b072ca101`

## `public sealed class TIdentity`

Covers the identity rule for drafts.
Every item is positive or negative, and only the engine mints.

## `public void RequestApply_TwoNewSentencesSharingText_MintsTwoIdsAndCommitStoresTwoRows()`

Two new sentences with one wording are two items, not one.
Each gets its own negative id when the content is applied, and its own row on commit.
Matching by text would have collapsed them.

## `public void RequestApply_SentencePickingStoredExample_KeepsIdAndCommitReusesRow()`

An item the user picked from the database keeps its positive id through the save.
The commit reuses that row and mints nothing, so the map has no pair for it.

## `public void DraftCommit_EveryNegativeIdHeld_AppearsInTheOutcomeMap()`

Every negative id the saved draft held is a key of the map, and every value is a stored row.
The stored entry, loaded back, carries the card, Situation, Register and sentence rows under those ids.

## `public void DraftCommit_LinkedRowGone_Refuses()`

A draft naming a stored row by id, when that row has since been deleted, is refused rather than rebound.
The old path made a fresh row from the text the chip still showed.
The card silently pointed somewhere new.
The refusal names the link, nothing is written, and the draft file stays for the user to mend.

## `public void RequestApply_WrittenNameStoredAlready_PicksTheStoredRow()`

A wording typed into a chip is looked up by the engine before it is added.
A Situation or Register the workspace already holds under that wording is picked by id, case and edge spaces folded.
So the form and any other client of the engine get one row for one wording.
The shelf grows no twins.
A wording nothing matches is minted as new.
A later draft typing it finds the row the first one made.

## `public void DraftArchiveSave_ItemWithoutId_Refuses()`

The drafts folder will not write a draft whose item still carries zero.
The rule cannot be bypassed by writing the file directly.

## `private static LEntryDraft TIdentityContentCreate(IReadOnlyList<LSentenceDraft> sentences)`

One entry with a pronunciation and a single Meaning card holding the given sentences.
The card carries a negative id from the test counter, not one the engine minted.

## `private static void TIdentityNegativeRead(LEntryDraft content, List<long> minted)`

Gathers every negative id the entry holds, from its pronunciation, Meaning and Collocation cards.

## `private static void TIdentityNegativeRead(IReadOnlyList<LCardDraft> cards, List<long> minted)`

Gathers every negative id of each card and its items, then walks its child cards.

## `private static void TIdentityNegativeAdd(long id, List<long> minted)`

Keeps an id only when it is negative, so stored and empty ids are skipped.
