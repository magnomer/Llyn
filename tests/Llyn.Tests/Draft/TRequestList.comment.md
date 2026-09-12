# TRequestList.cs

## `public sealed class TRequestList`

Covers the list requests inside a card and the panel lists, and what commit makes of them.
One apply case per structural noun, the two minting rules, the refusals, and a full sequence against the store.

## `public void RequestApply_SentenceAddition_MintsNegativeIdWithNoExample()`

A new row is named at once and quotes nothing, and a draft holding only such a row is unchanged.

## `public void RequestApply_SentenceText_MintsExampleOnFirstText()`

The first text on a row is the moment its example is named.

## `public void RequestApply_SentenceRemoval_DropsTheRowNamed()`

The row named goes and its neighbour stays.

## `public void RequestApply_SentenceShift_ReordersRows()`

A shift lands the row where asked and leaves the others in their order.

## `public void RequestApply_SentenceExample_KeepsThePositiveId()`

Picking a stored example puts its text under its stored id, so commit will update that row.

## `public void RequestApply_SituationAddition_MintsNegativeId()`

A typed situation is a new row with a minted id, never a match on wording.

## `public void RequestApply_SituationPick_CopiesTheStoredRow()`

A picked situation arrives with every stored field under its positive id, and picking it twice adds nothing.

## `public void RequestApply_SituationTitle_ChangesEveryCardHoldingIt()`

A situation is one row, so retitling it reaches both cards that link it.

## `public void RequestApply_ZeroItemId_Refuses()`

Item id zero names nothing and is refused with the item reason.

## `public void RequestApply_UnknownItemId_Refuses()`

An id the card does not hold is refused with the item reason.

## `public void RequestApply_RegisterTagImageVideoAddition_MintsEachRow()`

Every other list mints on addition the way situations do.

## `public void RequestApply_TranslationPick_HoldsTheEntryOnce()`

A translation picked twice is linked once.

## `public void DraftCommit_BlankSentenceRow_StoresNoExample()`

The blank row the form keeps reaches the store as nothing at all.

## `public void DraftCommit_FullRequestSequence_StoresTheSameRowsAsTheFixture()`

A fixture sent as requests commits to the rows the old whole-draft save produced, every list included.
Every stored row carries a positive id afterwards.

## `public void ReferenceCommit_AuthorsAddedAndPicked_AttachesThemInOrder()`

A source draft credits a new author and a stored one, and the authorship is marked known.
Commit attaches both in order.

## `public void RequestApply_AuthorRemovalAndShift_ReorderTheCredits()`

The credits move and drop by id like any other list.

## Inline notes

### `private static long TRequestCardAdd(LEngine engine, long draftId)`

Adds a meaning card at the end and answers its minted id.

### `private static long TRequestSentenceAdd(LEngine engine, long draftId, long cardId, int position)`

Adds a row at the place asked and answers its minted id.
