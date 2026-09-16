# LSettings.cs

## `public sealed record LSettings(`

The user's persisted application settings.
These live as `settings.json` inside the user's workspace folder — never elsewhere — so a workspace carries its own preferences.
The workspace folder path itself is not stored here.
It is the bootstrap locator that tells the program where to find this file.
It is kept in a small fixed pointer outside the workspace.

**Parameters**

- `LSettingsLocalization` — The chosen interface-language code, for example `"en"`.
- `LSettingsWindow` — The window geometry from the last run, or nothing before a first close.
- `LSettingsVolume` — How loud a stored pronunciation is played, from silence at zero to full at one.
- `LSettingsRespelled` — Whether looked-up transcriptions are recast through the pack's respelling groups, off by default.
- `LSettingsFrequency` — Whether an entry's frequency is fetched and shown, on by default.
- `LSettingsMorphology` — Whether an entry's inflected forms are fetched from the pack's morphology sources, on by default.
- `LSettingsLayout` — The widths, ordering and hidden languages of each tab, one record per tab, or nothing yet.
- `LSettingsLinked` — Whether dragging a panel in one tab sets the same width in every tab, on by default.
- `LSettingsMode` — Name of the tab standing open, and nothing before any tab is chosen.
- `LSettingsSplit` — Whether the open tab shows its editor rather than its read area.
- `LSettingsEpithet` — Whether every list prints an entry's epithet after its headword, on by default.
  The epithet is the reading a pack's reflex rule names, such as the Korean 훈 and 음.
- `LSettingsTally` — Whether the tally lines of a category page print the respelling set rather than IPA, off by default.
