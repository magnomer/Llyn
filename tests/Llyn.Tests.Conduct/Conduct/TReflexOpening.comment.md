# TReflexOpening.cs
Hash: `118cf235b0198d0e`

## `public sealed class TReflexOpening`

Covers the "More readings" opening of the reading view and the editor on a real workspace.
The engine stores it per entry, so each case reads it back through the engine relay.
The reading view writes through `CDisplaySound.CDisplayReflexToggle`, the editor through `CKindred.CKindredSpread`.

## `public void DisplaySpreadCheck_StoredStateWrittenTwice_ReadsThatEntryOpened()`

Repeating the same-state opening write leaves the entry opened.
An absent entry reads closed.

## `public void DisplayFoldOpened_AnotherEntryOpened_ShowsEachEntrysOwnStoredState()`

Opening another entry shows that entry's own stored state, and coming back shows the first one's again.

## `public void DisplayReflexToggle_ShownEntry_StoresTheStateAnswersTrueAndRaisesTheDisplayFold()`

The gate stores the opening for the shown entry and answers true.
The engine's fold bulletin reaches the display once, for that entry.

## `public void DisplayReflexToggle_NoEntryShown_AnswersFalseAndStoresNothing()`

With nothing shown the gate answers false, and the store stays closed.

## `public void DisplayReflexToggle_FailingPort_ShowsTheNoticeAndAnswersFalse()`

A port that faults the write shows `Reflex.SpreadFailed` once, and the gate answers false.

## `public void DisplayReflexToggle_OneEntry_LeavesAnotherEntryClosed()`

Two entries keep independent openings, so closing one leaves the other opened.

## `public void DisplayReflexToggle_EditorOnTheSameEntry_RefreshesTheEditorReflexFromTheStore()`

A write in the reading view repaints an editor holding the same entry through the fold bulletin.
The editor's block then reads the stored opening.

## `public void KindredRead_AnotherEntryHeld_ReadsEachHeldEntrysOwnStoredState()`

The editor's block reads the opening of the entry it holds, and another held entry reads its own.

## `public void KindredSpread_HeldEntry_StoresTheStateAnswersTrueAndRaisesTheDisplayFold()`

The editor's gate stores the opening for the held entry and answers true.
A reading view on that entry hears the fold bulletin and reads the stored opening.

## `public void KindredSpread_NoHeldEntry_AnswersFalseAndStoresNothing()`

An empty desk answers false, and the store stays closed.

## `public void KindredSpread_FailingPort_ShowsTheNoticeAndAnswersFalse()`

A port that faults the write shows `Reflex.SpreadFailed` once, and the gate answers false.

## `private static long TReflexOpeningPrepare(LEngine engine, string headword)`

Stores a bare English entry and answers its id.
