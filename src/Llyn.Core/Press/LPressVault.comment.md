# LPressVault.cs
Hash: `ebd676ab2466657b`

## `public interface LPressVault`

The port for the print setting kept under the workspace root, so the next print window opens with it.
`LPressFile` in Infrastructure is its adapter over the keep file and the JSON the setting is written in.
Core sees only the `LPressSetting` record and never the text.

## `LPressSetting? LPressVaultRead()`

The kept setting, or nothing while none was ever written.
Text that is not a setting also reads as nothing, so the window always opens.
A file that exists but cannot be read raises `LVaultFault`.

## `void LPressVaultSave(LPressSetting setting)`

Replaces the kept setting whole, exactly as given.
The caller decides what is worth keeping, such as an emptied page range.
A file that cannot be written raises `LVaultFault`.
