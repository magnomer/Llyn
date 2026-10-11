# TPressVault.cs
Hash: `dd0d4c8e3d031dae`

## `internal sealed class TPressVault : LPressVault`

The print setting store kept in memory, so a print window is checked without a file.
It sits beside `TPress` because the store is apart from the press.
Two print windows handed one instance share the kept setting, as two runs share the file.

## `internal LPressSetting? TPressVaultSetting`

The kept setting, or null before a save.
A test sets it to stand for a setting an earlier run kept.

## `internal int TPressVaultCount`

How many saves the store took, so a fact proves a close saved once or not at all.
A refused save does not count.

## `internal Exception? TPressVaultFailure`

The failure every save throws, or null for a store that keeps.
A test sets it to stand for a store that cannot be written.

## `public LPressSetting? LPressVaultRead()`

Answers `TPressVaultSetting`, so nothing kept reads as nothing.

## `public void LPressVaultSave(LPressSetting setting)`

Keeps the setting exactly as given, replacing the last one, and counts the save.
It throws `TPressVaultFailure` instead when one is set, keeping nothing.
