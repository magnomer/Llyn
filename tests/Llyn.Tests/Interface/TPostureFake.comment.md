# TPostureFake.cs

## `internal sealed class TPostureFake : LPostureVault`

The posture port kept in a dictionary, so a fake-rig engine starts a posture without a file.

## `public LPostureState? LPostureRead(string name)`

The state saved under the name, or nothing before a save.

## `public void LPostureSave(string name, LPostureState state)`

Keeps the state under the name, replacing the last one.
