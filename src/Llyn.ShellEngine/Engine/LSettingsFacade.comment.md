# LSettingsFacade.cs

## `internal sealed class LSettingsFacade`

The engine's facade for settings.
The user's persisted settings are read here and changed one field at a time.
Every change is written to the open workspace at once, so the file never lags what the engine holds.
The record itself is loaded when the workspace opens.
A workspace moved onto keeps its own settings, and only one without any inherits the current record.

## `public LSettingsFacade(LEngine engine)`

Creates the facade for its owning engine and shares the engine gate for settings operations.

## `internal LSettings LEngineSettingsRead()`

Returns the user's persisted settings.

## `internal bool LEnginePostureLoad(string name, out LPostureState? state)`

The posture stored under `name`, read through the workspace clerk.
False means a vault fault, recorded, and the caller keeps what it has.

## `internal void LEnginePostureSave(string name, LPostureState state)`

The posture written under `name` through the workspace clerk, a vault fault recorded and swallowed.

## `internal DateTimeOffset LEngineStampRead()`

The moment now, through the workspace clerk's clock, for a blank draft's stamp.

## `internal string LEngineTrailNormalize(string name)`

A file name made safe for the file system, through the trail clerk, for a vista's export name.

## `internal IReadOnlyDictionary<string, string> LEngineLocalizationLoad(string language)`

The localization table of `language`, through the workspace clerk.

## `internal string LEngineLocalizationRead()`

The stored interface language normalised to a listed one, so a deportment never names the localization.

## `internal IReadOnlyList<string> LEngineLocalizationScan()`

The interface languages the build embeds, as the localization catalog lists them.

## `internal string LEngineTextRead(string key)`

The interface text under a key, or the key itself when none is loaded.

## `internal string? LEngineTextFind(string key)`

The interface text under a key, or null when none is loaded.

## `internal void LEngineLocalizationSave(string language)`

Persists the chosen interface language and keeps it current.
Only that field is written, so a switch flipped by another part of the shell survives the change.
A settings bulletin is raised when the language changed, so the shell applies the catalog from the record.

## `internal void LEngineRespellingSave(bool respelled)`

Persists whether readings are shown and edited in their respelled form and keeps it current.
Every reading is stored in both forms, so a flip only changes which one each surface shows.
A settings bulletin is raised when the switch changed, so every open reading re-reads itself at once.

## `internal bool LEngineRespellingCheck(string language)`

Whether respellings show for `language`: the setting is on and the pack declares respelling groups.

## `internal bool LEnginePhonemicCheck(string language)`

Whether the pack of `language` is phonemic, through the language clerk.

## `internal void LEngineEpithetSave(bool epithet)`

Turns the epithet after every listed headword on or off.
A settings bulletin is raised when the switch changed, so the settings panel rewrites its summaries.

## `internal void LEngineTallySave(bool respelled)`

Persists whether the tally lines of a category page print the respelling set and keeps it current.
The page reads the switch on every fill, so the choice survives a restart and a change of category alike.

## `internal void LEngineFrequencySave(bool frequency)`

Persists whether an entry's frequency is fetched from the pack's web source and keeps it current.
The next fill reads the switch, so a flip neither refetches what is stored nor drops it.
A settings bulletin is raised when the switch changed, so the settings panel rewrites its summaries.

## `internal void LEngineMorphologySave(bool morphology)`

Turns the morphology fetch on or off.
Turning it off cancels every pending inflection fetch through the lacuna clerk.

## `private bool LEngineSettingsChange(Func<LSettings, LSettings> change)`

Applies one change to the settings snapshot under the gate and writes it through the workspace clerk.
An unchanged snapshot writes nothing and answers false.



