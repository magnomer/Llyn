# TFold.cs
Hash: `fe40ab6c01eaecf0`

## `public sealed class TFold`

Covers the editor's box folds end to end, driven with no window.
Editors share one engine over a throwaway workspace.
Stored changes refresh open editors holding the same entry.
A fault bundle lets a test refuse each box read or write at the reflex port.
It also covers the reading view's box reads and gates through the display's own fold area.
Both modes share one stored state per entry, so each mode reads back what the other wrote.

## `public void FoldOpened_AnotherEntryHeld_ReadsEachHeldEntrysOwnStoredState()`

Each box reads the state the engine stores for the held entry.
Opening another entry reads that entry's own states, not the former entry's.

## `public void FoldSpread_HeldEntry_StoresBothBoxesAnswersTrueAndRefreshesTheEditor()`

Each gate stores its box for the held entry and answers true.
Each stored write refreshes the editor once through the entry's fold bulletin.
Closing the rime-book box leaves the script box open.

## `public void FoldSpread_NoStoredEntryHeld_AnswersFalseAndStoresNothing()`

A fresh draft makes both gates answer false.
Both boxes read folded, and no stored entry gains a box state.

## `public void FoldSpread_FailingPort_ShowsTheNoticeAnswersFalseAndTellsNoOtherEditor()`

A box write that throws shows `Box.SpreadFailed` through the envoy and answers false, once per gate.
Both boxes keep their stored states, and a second editor on the entry hears nothing.

## `public void FoldOpened_FaultingPort_AnswersFoldedAndShowsTheReadNoticeOnce()`

A box read that throws answers folded for both boxes, though the store holds them open.
Both failed reads share `Box.SpreadReadFailed`, so this fold's repaint memory shows the notice only once.

## `public void FoldSpread_TwoEntriesAndTwoBoxes_KeepsEachStateApart()`

A box stored for one entry leaves the other box and the other entry as they were.

## `public void FoldSpread_FreshEditorOnTheSameEntry_ReadsTheStoredState()`

States stored through one editor read back open in a fresh editor holding the same entry.
This case creates another editor over the same engine, without restarting it.

## `public void FoldSpread_SecondEditorOnTheSameEntry_RefreshesThatEditorFromTheStore()`

A second editor on the same entry hears each stored write once and reads the new states.

## `public void FoldSpread_SecondEditorClosed_RefreshesNothingThere()`

A second editor that has closed hears no box stored in the first.
It holds no entry, so it reads folded.

## `public void DisplayBoxOpened_AnotherEntryShown_ReadsEachShownEntrysOwnStoredState()`

Each view box reads the state the engine stores for the shown entry.
Opening another entry reads that entry's own states, not the former entry's.

## `public void DisplayBoxSpread_ShownEntry_StoresBothBoxesAnswersTrueAndRaisesTheDisplayFold()`

Each view gate stores its box for the shown entry and answers true.
Each stored write raises the display's fold notice once for that entry.

## `public void DisplayBoxSpread_NoEntryShown_AnswersFalseAndStoresNothing()`

With nothing shown both view gates answer false and store nothing.
Both view boxes read folded.

## `public void DisplayBoxSpread_FailingPort_ShowsTheNoticeAndAnswersFalse()`

A box write that throws shows `Box.SpreadFailed` and answers false, once per view gate.
The store keeps both boxes folded.

## `public void DisplayBoxSpread_EditorOnTheSameEntry_RefreshesTheEditorAndReadsBackThere()`

A fold made in the reading view refreshes an editor holding the same entry, once per write.
The editor then reads both boxes open.

## `public void FoldSpread_DisplayOnTheSameEntry_RaisesTheDisplayFoldAndReadsBackThere()`

A fold made in the editor raises the display's fold notice for the shown entry, once per write.
The reading view then reads the rime-book box closed and the script box open.

## `public void DisplayBoxSpread_TwoEntriesAndTwoBoxes_KeepsEachStateApart()`

A box stored from the reading view leaves the other box and the other entry as they were.

## `private static long TFoldEntrySave(LEngine engine, string headword)`

Stores one English entry under `headword` and answers its id.
