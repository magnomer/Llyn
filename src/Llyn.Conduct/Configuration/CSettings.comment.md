# CSettings.cs
Hash: `82deb205c34554dc`

## `public sealed record CSettings(string CSettingsLocalization, bool CSettingsRespelled, bool CSettingsFrequency, bool CSettingsMorphology, bool CSettingsEpithet, int CSettingsOnline, string CSettingsNotebook)`

The workspace settings the settings panel shows.
The count of online lookups is copied from the engine, so the rule stays there.

**Parameters**

- `CSettingsLocalization`: the interface language.
- `CSettingsRespelled`: whether transcriptions show respelled.
- `CSettingsFrequency`: whether the frequency lookup is on.
- `CSettingsMorphology`: whether the morphology lookup is on.
- `CSettingsEpithet`: whether lists show epithets.
- `CSettingsOnline`: how many online lookups are on.
- `CSettingsNotebook`: the title of the top Joplin notebook a sync writes.
