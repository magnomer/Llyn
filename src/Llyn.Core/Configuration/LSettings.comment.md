# LSettings.cs
Hash: `7e9f1b34d7e479e4`

## `public sealed record LSettings(string LSettingsLocalization, bool LSettingsRespelled = false, bool LSettingsFrequency = true, bool LSettingsMorphology = true, bool LSettingsEpithet = true, bool LSettingsTally = false, string LSettingsGloss = "English", int LSettingsOutpost = 41184, string LSettingsWarrant = "", bool LSettingsAnalysis = true)`

The user's persisted engine settings.
These live as `settings.json` inside the user's workspace folder and nowhere else.
So a workspace carries its own preferences.
Every field here is a preference, except `LSettingsWarrant`, the protected Joplin credential.
The choices cover what the engine fetches, shows or transcribes.
Window posture is not part of this engine settings record.
The workspace folder path itself is not stored here.
It is the bootstrap locator that tells the program where to find this file.
It is kept in a small fixed pointer outside the workspace.

**Parameters**

- `LSettingsLocalization`: The chosen interface-language code, for example `"en"`.
- `LSettingsRespelled`: Whether looked-up transcriptions are recast through the pack's respelling groups, off by default.
- `LSettingsFrequency`: Whether an entry's frequency is fetched and shown, on by default.
- `LSettingsMorphology`: Whether an entry's inflected forms are fetched from the pack's morphology sources, on by default.
- `LSettingsEpithet`: Whether every list prints an entry's epithet after its headword, on by default.
  The epithet is the reading a pack's reflex rule names, such as the Korean 훈 and 음.
- `LSettingsTally`: Whether category tallies print the respelling set rather than IPA, off by default.
- `LSettingsGloss`: The language a new translation of an Example starts in, English by default.
  The pack list holds no such default, so the user's own language is kept here.
- `LSettingsOutpost`: The port Joplin's Data API is tried on first, 41184 by default.
- `LSettingsWarrant`: The Joplin token in its hidden form, empty until the user grants one.
  Only the platform twin of the warrant port can turn it back into the token.
- `LSettingsAnalysis`: Whether the inflection box shows the pack's custom sheets and marked letters, on by default.
  Off shows the default sheets with plain forms.
  The analysis is stored on every write either way, so a flip needs no new analysis.

## `public int LSettingsOnline`

How many of the web lookups are on, counting frequency and morphology.
The settings Web page shows it against the number of lookups there are.
