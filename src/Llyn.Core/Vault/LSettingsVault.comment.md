# LSettingsVault.cs

## `public interface LSettingsVault`

The persistence port for the user's engine settings.
`LSettingsLoader` in Infrastructure is its adapter over `settings.json` inside the workspace.
The engine reads once per workspace and saves on every change.

## `bool LSettingsExist();`

Whether the workspace has stored settings of its own.
A workspace without them inherits the settings of the one left behind.

## `LSettings LSettingsRead();`

The stored settings, or the defaults when none are stored or the stored ones are unreadable.

## `void LSettingsSave(LSettings settings);`

Stores `settings`, replacing whatever was there.
A store that cannot be written raises `LVaultFault`.
