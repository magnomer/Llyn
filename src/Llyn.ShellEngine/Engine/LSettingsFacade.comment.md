# LSettingsFacade.cs
Hash: `90385d9efafa7de3`

## `internal sealed class LSettingsFacade`

The engine's facade for settings.
The user's persisted settings are read here and changed one field at a time.
Every change is written to the open workspace at once, and a failed write restores the previous settings.
The record itself is loaded when the workspace opens.
A workspace moved onto keeps its own settings, and only one without any inherits the current record.

## `public LSettingsFacade(LEngine engine)`

Creates the facade for its owning engine and shares the engine gate for settings operations.

## `internal event Action? LEngineFoldChanged;`

A saved fold state changed, so every open editor over this engine repaints its rime-book and script switches.
It is the narrow notice of the folds, raised outside the gate.
An unchanged save raises nothing, so a repainted switch that echoes its state cannot loop.

## `internal LSettings LEngineSettingsRead()`

Returns the user's persisted settings.

## `internal bool LEnginePostureLoad(string name, out LPostureState? state)`

The posture stored under `name`, read through the workspace clerk.
False means a vault fault, recorded, and the caller keeps what it has.

## `internal bool LEnginePostureSave(string name, LPostureState state, out Exception? fault)`

The posture written under `name` through the workspace clerk, answering whether the write succeeded.
A vault fault is recorded by the clerk and handed back as `fault`, never thrown.
`fault` is a plain `Exception`, so the shell names no Core type.

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

## `internal IReadOnlyList<string> LEngineGroupFind(IReadOnlyList<(string, IReadOnlyList<string>)> groups, string? text)`

The groups whose keys' texts read `text`, as the Application localization matches them.

## `internal void LEngineLocalizationSave(string language)`

Persists the chosen interface language and keeps it current.
Only that field is written, so a switch flipped by another part of the shell survives the change.
A settings bulletin is raised when the language changed, so the shell applies the catalog from the record.

## `internal void LEngineRespellingSave(bool respelled)`

Persists whether readings are shown and edited in their respelled form and keeps it current.
Every reading is stored in both forms, so a flip only changes which one each surface shows.
A settings bulletin is raised when the switch changed, so every open reading re-reads itself at once.

## `internal bool LEngineRespellingCheck(string language)`

Whether respellings show for `language`, when the setting is on and the pack declares respelling groups.

## `internal string LEnginePronunciationRead(LEntryDraft draft)`

The primary reading of `draft` under the held respelling setting, through the language clerk.

## `internal bool LEnginePhonemicCheck(string language)`

Whether the pack of `language` is phonemic, through the language clerk.

## `internal (bool, string, string) LEngineMarkRead(string language)`

Whether readings of `language` show their respelling, and the brackets around them.
A respelled phonemic pack writes its readings between slashes.
Every other reading stands between square brackets.
This is the one owner of the bracket rule, so the editor and the reading view print alike.

## `internal void LEngineEpithetSave(bool epithet)`

Turns the epithet after every listed headword on or off.
A settings bulletin is raised when the switch changed, so the settings panel rewrites its summaries.

## `internal void LEngineTallySave(bool respelled)`

Persists whether the tally lines of a category page print the respelling set and keeps it current.
The page reads the switch on every fill, so the choice survives a restart and a change of category alike.

## `internal void LEngineFanqieSave(bool opened)`

Persists whether the editor's rime-book box stands open and keeps it current.
A real change raises `LEngineFoldChanged`, so every open editor repaints its switch.
No settings bulletin is raised, since that would refill every panel for one switch.

## `internal void LEngineScriptSave(bool opened)`

Persists whether the editor's script box stands open and keeps it current.
It raises `LEngineFoldChanged` on a real change, as the rime-book save does.

## `internal void LEngineFrequencySave(bool frequency)`

Persists whether an entry's frequency is fetched from the pack's web source and keeps it current.
The next fill reads the switch, so a flip neither refetches what is stored nor drops it.
A settings bulletin is raised when the switch changed, so the settings panel rewrites its summaries.

## `internal void LEngineOutpostSave(string text)`

Persists the port the Joplin clipper is looked for on first, from the text the user typed.
The staff's workspace clerk parses the text through `LWorkspaceOutpostParse`, which throws the port refusal.
So the engine never builds a Core refusal itself.
A settings bulletin is raised when the port changed, so the settings panel repaints the stored port.

## `internal bool LEngineMorphologyCheck()`

Whether the morphology fetch is on, so a reading view offers its paradigm.

## `internal void LEngineMorphologySave(bool morphology)`

Turns the morphology fetch on or off.
Turning it off cancels every pending inflection fetch through the lacuna clerk.

## `internal bool LEngineSettingsChange(Func<LSettings, LSettings> change)`

Applies one change to the settings snapshot under the gate and writes it through the workspace clerk.
An unchanged snapshot writes nothing and answers false.
A write that throws puts the previous snapshot back before the exception leaves, still under the gate.
The caller then raises no notice, so a rolled-back change tells no one.
The courier facade also calls it, so the hidden token is stored the same way.



