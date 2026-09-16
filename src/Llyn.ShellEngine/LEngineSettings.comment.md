# LEngineSettings.cs

## `public sealed partial class LEngine`

The settings side of the engine boundary.
The user's persisted settings are read here and changed one field at a time.
Every change is written to the open workspace at once, so the file never lags what the engine holds.
The record itself is loaded when the workspace opens.
A workspace moved onto keeps its own settings, and only one without any inherits the current record.

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

Persists whether readings are shown and edited in their respelled form and keeps it current.
Every reading is stored in both forms, so a flip only changes which one each surface shows.
A settings bulletin is raised so every open reading re-reads itself at once.

## `public bool LEngineRespellingCheck(string language)`

Whether readings of `language` are shown and edited as respellings right now.
True only while the switch is on and the pack declares respelling groups, since otherwise no respelling is ever stored.
A blank language has no pack and answers false.

## `public bool LEnginePhonemicCheck(string language)`

Whether the pack of `language` declares its respelling phonemic, so a respelled reading stands between slashes.
A blank language has no pack and answers false.

## `public void LEngineEpithetSave(bool epithet)`

Turns the epithet after every listed headword on or off.

## `public void LEngineTallySave(bool respelled)`

Persists whether the tally lines of a category page print the respelling set and keeps it current.
The page reads the switch on every fill, so the choice survives a restart and a change of category alike.

## `public void LEngineFrequencySave(bool frequency)`

Persists whether an entry's frequency is fetched from the pack's web source and keeps it current.
The next fill reads the switch, so a flip neither refetches what is stored nor drops it.

## `public void LEngineMorphologySave(bool morphology)`

Persists whether an entry's inflected forms are looked up on the web and keeps it current.
Turning the switch off cancels every fetch still in flight, so no form lands after the user said no.
Forms already stored stay, because the switch governs asking, not keeping.

## `public void LEngineLayoutSave(IEnumerable<LLayout> layout)`

Persists the panel widths, ordering and hidden languages of the tabs given and keeps them current.
Each field a tab gives replaces that field of its own record.
A field left empty keeps what the record had.
Every tab not given keeps the record it had.
A drag therefore never drops the ordering a panel chose, and an ordering never drops a dragged width.
The merge runs under the gate, so one drag never drops what another tab wrote.
A linked drag hands over every tab at once, so the file is written once, not once per tab.

## `public void LEngineLayoutReset()`

Drops the stored width of every tab and keeps the result current.
Each tab's ordering and hidden languages stay, because the button promises widths and nothing more.
A tab stripped of its width takes its markup width on the next start.

## `public void LEngineLinkedSave(bool linked)`

Persists whether dragging a panel in one tab sets the same width in every tab and keeps it current.
The widths themselves are stored per tab either way, so a flip neither moves nor loses any panel.

## `private void LEngineSettingsChange(Func<LSettings, LSettings> change)`

Applies `change` to the settings held here and writes the result out, under the engine gate.
The read and the write are one step, so one writer never overwrites what another wrote between them.
The shell therefore never reads settings, changes a field and hands the whole record back.
A change that leaves the record equal writes nothing, so a window resting where it was costs no file write.

## `private void LEngineSettingsSave()`

Writes the settings held here into the open workspace.
A refused write is recorded in the audit log rather than thrown, so a locked file never ends the program.
The record in memory stays current either way, and the next change tries the file again.
