# LSettings.cs
Hash: `42501c42353a7ccc`

## `public sealed record LSettings(string LSettingsLocalization, bool LSettingsRespelled = false, bool LSettingsFrequency = true, bool LSettingsMorphology = true, bool LSettingsEpithet = true, bool LSettingsTally = false, string LSettingsGloss = "English", bool LSettingsFanqieOpened = false, bool LSettingsScriptOpened = false, int LSettingsOutpost = 41184, string LSettingsWarrant = "", bool LSettingsAnalysis = true)`

The user's persisted engine settings.
These live as `settings.json` inside the user's workspace folder and nowhere else.
So a workspace carries its own preferences.
Every field here is a user choice, except `LSettingsWarrant`, a machine-bound credential the user grants once.
The choices cover what the engine fetches, shows or transcribes, or which box stays open.
How the window stands is not such a choice, so it lives in the shell's posture beside this file.
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
- `LSettingsGloss` — The language a new translation of an Example starts in, English by default.
  The pack list holds no such default, so the user's own language is kept here.
- `LSettingsFanqieOpened` — Whether the editor's folded rime-book box stands open, off by default.
  It survives a move to another entry and a restart, so the user opens it once.
- `LSettingsScriptOpened` — Whether the editor's folded script box stands open, off by default.
  It is kept the same way as the rime-book box.
- `LSettingsOutpost` — The port Joplin's Data API is tried on first, 41184 by default.
- `LSettingsWarrant` — The Joplin token in its hidden form, empty until the user grants one.
  Only the platform twin of the warrant port can turn it back into the token.
- `LSettingsAnalysis` — Whether the inflection box shows the pack's custom sheets and marked letters, on by default.
  Off shows the default sheets with plain forms.
  The analysis is stored on every write either way, so a flip needs no new analysis.

## `public int LSettingsOnline`

How many of the web lookups are on, counting frequency and morphology.
The settings Web page shows it against the number of lookups there are.
