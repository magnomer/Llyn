# LEngineSettings.cs

## `public sealed partial class LEngine`

The settings side of the engine boundary.
The user's persisted settings are read here and changed one field at a time.
Every change is written to the open workspace at once, so the file never lags what the engine holds.
The record itself is loaded when the workspace opens.
A workspace moved onto keeps its own settings, and only one without any inherits the current record.

## `public LSettings LEngineSettingsRead()`

Returns the user's persisted settings.

## `public LKeep LEngineKeepRead()`

Hands out the keep port bound to the open workspace, set with the other ports when the workspace opened.
The shell persists its posture through it, so a switched workspace is followed without the shell knowing the disk.

## `public IReadOnlyDictionary<string, string> LEngineLocalizationLoad(string language)`

Opens the catalog of a language through the localization port and makes it the current one.
The veneer copies the answer into its resources, so no ring above the engine opens a resource.
The default catalog is applied before any engine is built, so the bootstrap opens that one through the port itself.

## `public void LEngineLocalizationSave(string language)`

Persists the chosen interface language and keeps it current.
Only that field is written, so a switch flipped by another part of the shell survives the change.
A settings bulletin is raised when the language changed, so the shell applies the catalog from the record.

## `public void LEngineRespellingSave(bool respelled)`

Persists whether readings are shown and edited in their respelled form and keeps it current.
Every reading is stored in both forms, so a flip only changes which one each surface shows.
A settings bulletin is raised when the switch changed, so every open reading re-reads itself at once.

## `public bool LEngineRespellingCheck(string language)`

Whether readings of `language` are shown and edited as respellings right now.
True only while the switch is on and the pack declares respelling groups, since otherwise no respelling is ever stored.
A blank language has no pack and answers false.

## `public bool LEnginePhonemicCheck(string language)`

Whether the pack of `language` declares its respelling phonemic, so a respelled reading stands between slashes.
A blank language has no pack and answers false.

## `public void LEngineEpithetSave(bool epithet)`

Turns the epithet after every listed headword on or off.
A settings bulletin is raised when the switch changed, so the settings panel rewrites its summaries.

## `public void LEngineTallySave(bool respelled)`

Persists whether the tally lines of a category page print the respelling set and keeps it current.
The page reads the switch on every fill, so the choice survives a restart and a change of category alike.

## `public void LEngineFrequencySave(bool frequency)`

Persists whether an entry's frequency is fetched from the pack's web source and keeps it current.
The next fill reads the switch, so a flip neither refetches what is stored nor drops it.
A settings bulletin is raised when the switch changed, so the settings panel rewrites its summaries.

## `public void LEngineMorphologySave(bool morphology)`

Persists whether an entry's inflected forms are looked up on the web and keeps it current.
Turning the switch off cancels every fetch still in flight, so no form lands after the user said no.
Forms already stored stay, because the switch governs asking, not keeping.
A save that leaves the switch where it was cancels nothing and raises nothing.
A settings bulletin is raised when the switch changed, so the settings panel rewrites its summaries.

## `private bool LEngineSettingsChange(Func<LSettings, LSettings> change)`

Applies `change` to the settings held here and writes the result out, under the engine gate.
The read and the write are one step, so one writer never overwrites what another wrote between them.
The shell therefore never reads settings, changes a field and hands the whole record back.
A change that leaves the record equal writes nothing, so a window resting where it was costs no file write.
Returns whether the record changed, so a caller raises its bulletin only for a real change.
A settings control echoing the stored value back is therefore a no-op below the boundary.

## `private void LEngineSettingsSave()`

Writes the settings held here into the open workspace through the settings port.
A refused write is recorded in the audit log rather than thrown, so a locked file never ends the program.
The record in memory stays current either way, and the next change tries the file again.
