# TCollocationSense.cs
Hash: `81360a78fa51777a`

## `public sealed class TCollocationSense`

Covers the links from a collocation to the meanings of its entry, in the engine.
Every case starts from one entry with a parent meaning and two children, a second top-level meaning, and one collocation.
The collocation links the second top-level meaning first and the first child second.
So its home is the second top-level meaning, and its further link sits one level down.

## `public void CollocationSave_LinkedMeanings_StoresLinkOrder()`

The stored links name the stored meanings, in the order saved.
The first link stays first, because it is the home.

## `public void RequestContentApply_LinkedMeanings_SendsLinksInOrder()`

The shared entry is applied as content to a fresh draft.
The answered draft keeps both links in order, against its own meaning ids.
After the commit, the stored links read as the paths 2 and 1.1.

## `public void MeaningSave_LinkedMeaningDropped_DropsLinkRows()`

The update drops the home meaning but keeps the stale link in the collocation.
The save keeps only links to meanings still stored, so one link row remains.
The child link is then the home, lifted to its top-level parent when nesting is off.

## `public void CollocationSave_LinkEdit_RecordsCollocationUpdate()`

Reordering the links changes nothing else, yet records a collocation update in the revision.

## `public void CardSenseAttach_NestedMeaning_KeepsLink()`

A meaning one level down is a valid link target.
The new link goes last, so the home stays first, and the commit stores it.

## `public void CardSenseDetach_TwoLinks_KeepsOtherLink()`

Dropping the home link leaves the further link alone.

## `public void CardRemove_LinkedParentMeaning_SweepsDescendantLinks()`

Removing a parent meaning in the draft drops the links to its children too.
Links to meanings outside the removed branch stay.

## `public void CardMove_LinkedMeaning_KeepsLinks()`

Moving a linked child to the top level keeps every link in order.
A move is no removal, so nothing is swept.

## `public void CardApply_ExpressionOnMeaning_RefusesRequest()`

A typed expression sent to a meaning one level down is refused with the card reason.

## `public void CardApply_EmptyExpressionOnMeaning_LeavesTheMeaningUnchanged()`

An unspecified or blank expression sent to a meaning is no refusal.
The meaning comes back with its expression and definition as they were.

## `public void EntryExport_LinkedCollocation_RoundTripsSensePaths()`

The entry is exported, deleted, then imported as new with no omission.
Its links then read as the paths 2 and 1.1 against the imported meanings.
So the links point at the same places as before the export.

## `public void LiveryCardAppend_NestedHome_RendersUnderNestedMeaning()`

The page is altered by hand so the first link is the first child.
The livery note keeps nested homes, so the collocation renders inside that child, before the second child.
Its row is open and sits in a collocations block under the collocation heading.
The further link adds a stub line to the second top-level meaning.
With keys as their own text, the stub prints its bare key.
No collocation is unhomed, so the Other section is absent.

## `public void LiveryCardAppend_FoldedCollocation_WritesClosedRow()`

A stored fold on the collocation writes its row as a closed disclosure.
The summary still names the phrase, and no numbered card title stands in for the row.

## `public void LiveryCardAppend_UnlinkedCollocation_WritesOtherRow()`

A collocation linked to no meaning writes an open row in a collocations block under the Other heading.
No numbered card title stands in for the row.
The block carries no collocation heading of its own.

## `private static LEntry TCollocationSenseSave(LEngine engine) =>`

Saves the shared entry from `TCollocationSenseCreate`.

## `private static LEntryDraft TCollocationSenseCreate()`

Builds the shared entry, with meaning ids minted for the draft so the links can name them.
