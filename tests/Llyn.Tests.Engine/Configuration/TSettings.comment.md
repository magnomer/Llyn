# TSettings.cs
Hash: `5e4d094d21d1bcb4`

## `public sealed class TSettings`

Covers the engine's settings file, including interface language and its switches.
The epithet switch round-trips under the `epithet` key and is written even when off.
The tally switch loads as off unless the key is an explicit true, and the engine's save writes it.
A language the catalog does not list is kept as saved but reads back as the default.
A file written before the window posture moved out still loads, its posture keys passed over.
A file written before the box states moved to the database still loads, its `fanqie` and `script` keys passed over.
The next save drops those keys.
The respelling switch round-trips through the loader as a JSON boolean.
A missing key or a non-boolean value loads as off, so an older or hand-edited file never turns it on.
The engine's save reaches both the held settings and the file under the `respelling` key.
The frequency switch round-trips under the `frequency` key and is written even when off.
A missing key or a non-false value loads as on, so only an explicit false turns the fill off.
The morphology switch round-trips under the `morphology` key with the same rule.
The custom analysis switch round-trips under the `analysis` key with the same rule, on by default.
Changing analysis raises one Settings bulletin and one all-entry Inflection bulletin.
Saving the same value again raises nothing more.
A file that is not JSON loads as defaults and is copied aside as `settings.broken.json` first.
A save leaves no pending file behind and the saved file reports as existing.
A folder without a settings file reports none.
A switch saved with the value already held raises no settings bulletin, so an echoing control is a no-op.
A switch that changes raises the bulletin once, and saving the same value again raises nothing more.

## `public void SettingsSave_Respelled_LoadsTrue()`

A saved true respelling value reloads as true.

## `public void SettingsLoad_KeyAbsentOrNotBoolean_LoadsFalse(string json)`

Absent or non-boolean respelling values load as false.

## `public void SettingsSave_FrequencyOff_RoundTripsFalse()`

Frequency can be persisted explicitly as false.

## `public void SettingsLoad_FrequencyAbsentOrNotFalse_LoadsTrue(string json)`

Only explicit false disables frequency on load.

## `public void SettingsSave_MorphologyOff_RoundTripsFalse()`

Morphology can be persisted explicitly as false.

## `public void SettingsSave_AnalysisOff_RoundTripsFalse()`

Custom analysis can be persisted explicitly as false.

## `public void SettingsLoad_AnalysisAbsentOrNotFalse_LoadsTrue(string json)`

Only explicit false disables custom analysis on load.

## `public void AnalysisSave_Off_RaisesInflectionAndKeepsValue()`

Changing analysis updates storage and publishes one Settings bulletin plus one all-entry Inflection bulletin.

## `public void SettingsLoad_MorphologyAbsentOrNotFalse_LoadsTrue(string json)`

Only explicit false disables morphology on load.

## `public void SettingsSave_EpithetOff_RoundTripsFalse()`

Epithet false remains an explicit persisted value.

## `public void RespellingSave_True_ReadsBackAndWritesKey()`

Engine respelling changes reach both the held settings and JSON key.

## `public void SettingsSave_TallyOn_RoundTripsTrue()`

A saved true tally value reloads as true.

## `public void SettingsLoad_TallyAbsentOrNotTrue_LoadsFalse(string json)`

Only explicit true enables tally on load.

## `public void TallySave_True_ReadsBackAndWritesKey()`

Engine tally changes reach both the held settings and JSON key.

## `public void SettingsLoad_BrokenJson_LoadsDefaultsAndKeepsCopy()`

Malformed JSON falls back to defaults and retains a broken-file copy.

## `public void SettingsSave_Written_LeavesNoPendingFile()`

Successful saving leaves the settings file without a pending file.

## `public void SettingsExist_NoFile_ReportsFalse()`

A missing settings file is reported as absent.

## `public void SettingsLoad_LegacyPostureKeys_LoadsEngineFields()`

Legacy posture keys do not prevent current engine fields from loading.

## `public void SettingsLoad_LegacyBoxKeys_LoadsTheOtherFieldsAndDropsThemOnSave()`

Legacy Fanqie and Script keys are ignored and disappear on the next save.

## `public void LocalizationRead_UnlistedLanguageSaved_FallsToDefault()`

An unlisted saved language reads as the catalog default.

## `public void SettingsChange_Echo_RaisesNoBulletin()`

Saving an unchanged setting publishes no Settings bulletin.

## `public void SettingsChange_Changed_RaisesBulletinOnce()`

A changed setting publishes once, and a repeated equal value publishes nothing more.
