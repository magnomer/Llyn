# CSettings.cs
Hash: `d9ce61c2c6d3e122`

## `public sealed record CSettings(string CSettingsLocalization, bool CSettingsRespelled, bool CSettingsFrequency, bool CSettingsMorphology, bool CSettingsEpithet, int CSettingsOnline, string CSettingsOutpost)`

The workspace settings the settings panel shows.
The count of online lookups is copied from the engine, so the rule stays there.

**Parameters**

- `CSettingsLocalization`: the interface language.
- `CSettingsRespelled`: whether transcriptions show respelled.
- `CSettingsFrequency`: whether the frequency lookup is on.
- `CSettingsMorphology`: whether the morphology lookup is on.
- `CSettingsEpithet`: whether lists show epithets.
- `CSettingsOnline`: how many online lookups are on.
- `CSettingsOutpost`: the stored Joplin port as ready text, so the driver paints it with no number rule.
