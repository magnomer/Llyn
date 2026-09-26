# PSettings.cs

## `public partial class PSettings : UserControl`

The settings panel as a control: what it is made of, and the stored choices it opens on.
What each choice costs — a catalog swap, a whole different workspace — lives in the files beside this one.

## `public PSettings()`

Loads the panel's markup from the Veneer and wears it as its content.
The markup's name scope is copied onto the panel, so its named parts answer `FindName`.
It names the language box's value path, sets the three icons and subscribes every control's event.
The language box starts empty, since its items wait for the window deportment.

## `private TextBox PWinnow`

Each named part of the markup is read through `FindName`, so call sites keep the old generated names.

## `internal void PSettingsAttach(PWindow host)`

Puts the panel to work on the window deportment and shows the choices already stored.
The panel also listens for settings bulletins, which drive every redraw a saved choice needs.
The language items are built first, so the ledger summary and the sync find the stored language.
The ledger rows get their fill through `PLookItemAttach` before the catalog is built.

## `private void PLocalizationBuild()`

Fills the language box with one item per catalog the build embeds.
Each item shows the culture's native name and carries its language code as `Tag`.
The markup lists no language, so a new catalog file needs no edit to either side.

## `internal void PSettingsSync()`

Aligns every control of the panel to the settings and workspace the engine now holds, and to the posture.
It runs on attach and again after a workspace change, since the new workspace may carry other choices.
Setting the language box raises its selection change, and the handler writes the value back.
That write leaves the record equal, so the engine neither saves nor raises a bulletin.
The switches listen on `Click`, which setting `IsChecked` in code does not raise.
So a synced switch writes nothing back at all.
A stored language the interface does not carry is shown as English, the language the program opened in.

## `private void PSettingsBulletinHandle(LBulletin bulletin)`

Redraws the panel after the engine reports a settings change.
The language catalog is applied from the record, so the interface follows whatever the engine holds.
The ledger summaries are rewritten, since every one of them prints a stored choice.
The linked switch is no settings field, so its handler links the tabs itself.

## Inline notes

### `private PWindow _pSettingsHost = null!;`

The window this panel sits in.
It is who asks before a workspace change throws typed work away.
It also owns the other panels that change moves onto the new workspace.
