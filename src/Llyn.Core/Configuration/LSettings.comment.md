# LSettings.cs

## `public sealed record LSettings(`

The user's persisted application settings.
These live as `settings.json` inside the user's workspace folder — never elsewhere — so a workspace carries its own preferences.
The workspace folder path itself is not stored here.
It is the bootstrap locator that tells the program where to find this file.
It is kept in a small fixed pointer outside the workspace.

**Parameters**

- `LSettingsLocalization` — The chosen interface-language code, for example `"en"`.
