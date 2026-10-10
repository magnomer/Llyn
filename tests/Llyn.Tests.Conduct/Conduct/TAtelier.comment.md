# TAtelier.cs
Hash: `82cc009fe7bb9f3a`

## `public sealed class TAtelier`

Covers Conduct's atelier over a real posture, on the fake rig or a throwaway workspace database.
No test opens a window.
A fresh engine reads full volume and no split.
The about notice reads its version line's wording key.
A workspace that fails to open reads the busy key only when another program holds the database.
The rescue notice reads its key only when the database was set aside.
A vista start hands every Conduct subject and order to the engine by name, and no subject as none.
A volume step plays at once, and only a settled level reaches the stored posture.
A second atelier over the same engine reads the stored posture.
A posture store whose save throws `LVaultFault` shows `Layout.SaveFailed` once over two settled levels.
The audit receives each fault once, from the clerk, since the shown notice records nothing.
A layout failure heard before the first open is shown by that open, and a second open shows nothing.
A kept level below silence, above full or infinite reads at the nearest end.
A kept level that is not a number reads as full, so every driver gets a finite level.
A set level below silence, above full or infinite is pulled to the nearest end before the player hears it.
A set level that is not a number plays and keeps full, so set and read keep one promise.
A level held only in memory therefore shows as unwritten there.
The status strip is shown the status at once, and a detached strip is shown nothing more.
The status carries the wording keys the atelier chose from the engine's verdicts, and the amount in their unit.
Opening raises the views before their state, and sweeps a blank leftover draft first.
Opening twice still lets each ledger change be heard once.
Workspace opening leaves the input editor holding a blank draft.
The quit case asks once and discards every area.
The workspace change gate lives in `TAtelierWorkspace`.

## `private static LMediaPort TAtelierMediaCreate(List<double> played)`

A media port that records every level handed to the player.

## `public void AtelierAboutRead_AnySession_ReadsVersionKey()`

The about read exposes the version wording key.

## `public void AtelierRefusalRead_BusyOrNot_ReadsItsOwnKey()`

Only a busy workspace refusal selects the busy wording key.

## `public void AtelierRescueRead_SetAsideOrNot_ReadsTheKeyOnlyWhenDone()`

Only a completed set-aside selects the rescue wording key.

## `public void AtelierVistaStart_EverySubjectAndOrder_ReachesTheEngineByName()`

Subject and order values reach the engine as their expected names.

## `public void AtelierRead_FreshEngine_ReadsDefaultVolumeAndSplit()`

A fresh engine starts at full volume with no split.

## `public void AtelierVolumeSet_Unsettled_PlaysWithoutWriting()`

An unsettled volume change reaches playback without persisting posture.

## `public void AtelierVolumeSet_Settled_WritesTheLevel()`

A settled level is available to a second atelier over the same engine.

## `public void AtelierVolumeSet_FaultingPostureStore_ShowsTheLayoutFailureOnceAndRecordsEachFault()`

Repeated posture-save faults produce one shown notice but retain both recorded faults.

## `public void AtelierOpen_LayoutFailureBeforeOpen_ShowsTheNoticeOnTheOpen()`

A pending layout failure is shown on opening and not repeated on reopening.

## `public void AtelierVolumeRead_KeptLevelOutOfRange_ReadsAFiniteLevelFromSilenceToFull(double level, double read)`

Stored hostile levels read as finite values between silence and full volume.

## `public void AtelierVolumeSet_LevelOutOfRange_PlaysAndKeepsAFiniteLevelFromSilenceToFull(double level, double kept)`

Hostile input levels are normalized consistently for playback and storage.

## `public void WorkspaceEstablishmentChanged_Opened_ShowsTheStatusAtOnceAndStopsOnDetach()`

Status is delivered immediately and stops reaching a detached observer.

## `public void AtelierEstablishmentRead_EngineVerdicts_ChoosesWordingKeysAndAmount(int unsaved, long entry, long size, string entryKey, string sizeKey, double amount)`

Engine counts and sizes select the expected status wording and unit amount.

## `public void AtelierOpen_Opened_RaisesTheViewsBeforeTheirState()`

Opening publishes views before their state.

## `public void AtelierOpen_BlankLeftoverDraft_SweepsItBeforeTheViewsRestore()`

A blank leftover draft is swept before view restoration.

## `public void AtelierOpen_Reopened_HearsEachLedgerChangeOnce()`

Reopening does not duplicate ledger-change delivery.

## `public void AtelierWorkspaceChange_InputHeld_OpenLeavesTheInputOnABlankEntry()`

Workspace opening leaves the input editor holding a blank draft.

## `public void AtelierQuitConfirm_InputUnsaved_AsksOnceAndDiscardsEveryArea()`

Quit confirmation asks once, closes input and discards held editing areas.
