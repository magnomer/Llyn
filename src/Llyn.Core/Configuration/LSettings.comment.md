# LSettings.cs

## `public sealed record LSettings(`

The user's persisted engine settings.
These live as `settings.json` inside the user's workspace folder — never elsewhere — so a workspace carries its own preferences.
Every field here is an engine fact: what the engine fetches, shows or transcribes.
How the window stands is not an engine fact, so it lives in the shell's posture beside this file.
The workspace folder path itself is not stored here.
It is the bootstrap locator that tells the program where to find this file.
It is kept in a small fixed pointer outside the workspace.

**Parameters**

- `LSettingsLocalization` — The chosen interface-language code, for example `"en"`.
- `LSettingsRespelled` — Whether looked-up transcriptions are recast through the pack's respelling groups, off by default.
- `LSettingsFrequency` — Whether an entry's frequency is fetched and shown, on by default.
- `LSettingsMorphology` — Whether an entry's inflected forms are fetched from the pack's morphology sources, on by default.
- `LSettingsEpithet` — Whether every list prints an entry's epithet after its headword, on by default.
  The epithet is the reading a pack's reflex rule names, such as the Korean 훈 and 음.
- `LSettingsTally` — Whether the tally lines of a category page print the respelling set rather than IPA, off by default.
