# PSettings.xaml.cs

## `public partial class PSettings : UserControl`

The settings panel as a control: what it is made of, and the stored choices it opens on.
What each choice costs — a catalog swap, a whole different workspace — lives in the files beside this one.

## `internal void PSettingsAttach(PWindow host, LEngine engine)`

Puts the panel to work on `engine` and shows the choices already stored in it.
That is the last thing done before user changes start being saved.

## `internal void PSettingsSync()`

Aligns every control of the panel to the settings and workspace the engine now holds.
It runs on attach and again after a workspace change, since the new workspace may carry other choices.
The ready flag is lowered while the controls are set, so none of them writes its value back.
A stored language the interface does not carry is shown as English, the language the program opened in.

## Inline notes

### `private PWindow _pSettingsHost = null!;`

The window this panel sits in.
It is who asks before a workspace change throws typed work away.
It also owns the other panels that change moves onto the new workspace.

### `private bool _pSettingsReady;`

Whether the stored choices have finished being applied.
Applying them raises the same change events a user action does, and those must not be written back as choices.
It is lowered again whenever the controls are aligned to another workspace.
