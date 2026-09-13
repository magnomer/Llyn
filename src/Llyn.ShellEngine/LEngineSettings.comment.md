# LEngineSettings.cs

## `public sealed partial class LEngine`

The settings side of the engine boundary.
The user's persisted settings are read here and changed one field at a time.
Every change is written to the open workspace at once, so the file never lags what the engine holds.
The record itself is loaded when the workspace opens and follows it when the workspace moves.

## `public LSettings LEngineSettingsRead()`

Returns the user's persisted settings.

## `public void LEngineLocalizationSave(string language)`

Persists the chosen interface language and keeps it current.
Only that field is written, so a window geometry saved by another part of the shell survives the change.

## `public void LEngineWindowSave(LWindowState window)`

Persists the window geometry of the run that is ending and keeps it current.
Only that field is written, so an interface language chosen during the same session survives the change.

## `public void LEngineVolumeSave(double volume)`

Persists how loud a pronunciation is played and keeps it current.
The level is clamped here as well as on the way in from the file.
No caller can write a volume the player cannot take.
Every view plays through the same level.
A change made in one is the level the next one opens at.

## `public void LEngineRespellingSave(bool respelled)`

Persists whether looked-up transcriptions are recast through the pack's respelling groups and keeps it current.
The next lookup reads the switch, cached or fresh, so no search is run again to honour a flip.

## `public void LEngineFrequencySave(bool frequency)`

Persists whether an entry's frequency is fetched from the pack's web source and keeps it current.
The next fill reads the switch, so a flip neither refetches what is stored nor drops it.

## `public void LEngineMorphologySave(bool morphology)`

Persists whether an entry's inflected forms are looked up on the web and keeps it current.
Turning the switch off cancels every fetch still in flight, so no form lands after the user said no.
Forms already stored stay, because the switch governs asking, not keeping.

## `public void LEngineLayoutSave(IEnumerable<LLayout> layout)`

Persists the panel widths of the tabs given and keeps them current.
Each tab given replaces its own record, and every tab not given keeps the record it had.
The merge runs under the gate, so one drag never drops what another tab wrote.
A linked drag hands over every tab at once, so the file is written once, not once per tab.

## `public void LEngineLinkedSave(bool linked)`

Persists whether dragging a panel in one tab sets the same width in every tab and keeps it current.
The widths themselves are stored per tab either way, so a flip neither moves nor loses any panel.

## `private void LEngineSettingsChange(Func<LSettings, LSettings> change)`

Applies `change` to the settings held here and writes the result out, under the engine gate.
The read and the write are one step, so one writer never overwrites what another wrote between them.
The shell therefore never reads settings, changes a field and hands the whole record back.
