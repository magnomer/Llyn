# TFold.cs
Hash: `abbb3ef129196abd`

## `public sealed class TFold`

Covers the editor's remembered folds end to end, driven with no window.
Real editors over a throwaway workspace share one engine, so a fold saved in one reaches the others.
A fake settings port lets a test refuse each save.
A faulting settings store under a real engine checks the whole path from the vault up.

## `public void FoldToggle_OpenedBoxes_ReadsTheNewStateAndRaisesTheChange()`

Each toggle gate answers its new state on the next read and raises the fold change once.
Closing the rime-book box leaves the script box open.

## `public void FoldToggle_FreshConduct_KeepsTheSavedState()`

Open states saved through earlier editors read back true in a fresh editor holding a stored entry.
The workspace's settings file holds both states too, so a restart keeps them.

## `public void FoldToggle_SecondEditor_RaisesEachSavedChangeThere()`

A second attached editor hears each real fold change once, and an unchanged save raises nothing there.

## `public void FoldToggle_SecondEditorClosed_RaisesNothingThere()`

A second attached editor that has closed no longer hears a fold toggled in the first.

## `public void FoldToggle_RefusedSave_ShowsTheSaveFailureAndKeepsTheState()`

A settings port whose saves throw makes each toggle gate show `Settings.SaveFailed` through the envoy.
Each gate still raises the fold change, and both boxes read open as saved before.
The folds need no desk, so the test builds them alone over the refusing port.

## `public void FoldToggle_FaultingStore_ShowsTheSaveFailureKeepsTheStateAndRecordsTheFault()`

A settings store whose save throws `LVaultFault` makes the toggle show `Settings.SaveFailed`.
The box and the engine's settings read as before the toggle.
The audit receives the fault once, from the shown notice, since the clerk only throws it on.

## `private static long TFoldEntrySave(LEngine engine)`

Stores one English entry and answers its id.

## `private static CEditor TFoldEditorPrepare(LEngine engine, long? entry)`

An editor on the library tab, holding `entry` or a fresh draft.
