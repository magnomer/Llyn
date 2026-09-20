# LPostureVault.cs

## `public interface LPostureVault`

The port for the posture kept under the workspace root, how the main window stands.
`LPostureFile` in Infrastructure is its adapter over the keep file and the JSON the posture is written in.
The posture reads and writes the `LPostureState` record and never sees the text.

## `LPostureState? LPostureRead(string name)`

The posture kept under the name, or nothing while none was ever written.
Text that is not a posture reads as the default posture, so the window always opens.
A file that exists but cannot be read raises `LVaultFault`.

## `void LPostureSave(string name, LPostureState state)`

Replaces the posture kept under the name whole.
A file that cannot be written raises `LVaultFault`.
