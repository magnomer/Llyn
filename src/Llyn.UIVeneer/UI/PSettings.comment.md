# PSettings.xaml.cs

## `public partial class PSettings : UserControl`

The settings panel as a control: what it is made of, and the stored choices it opens on.
What each choice costs — a catalog swap, a whole different workspace — lives in the files beside this one.

## `internal void PSettingsAttach(PWindow host, LEngine engine)`

Puts the panel to work on `engine` and shows the choices already stored in it.
The panel also listens for settings bulletins, which drive every redraw a saved choice needs.

## `internal void PSettingsSync()`

Aligns every control of the panel to the settings and workspace the engine now holds, and to the posture.
It runs on attach and again after a workspace change, since the new workspace may carry other choices.
Setting a control raises its change event, and the handler writes the value back to the engine.
That write leaves the record equal, so the engine neither saves nor raises a bulletin.
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
