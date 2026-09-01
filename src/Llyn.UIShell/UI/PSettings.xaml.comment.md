# PSettings.xaml.cs

## `public partial class PSettings : UserControl`

The settings panel as a control: what it is made of, and the stored choices it opens on. What each choice costs — a catalog swap, a whole different workspace — lives in the files beside this one.

## `internal void PSettingsAttach(PWindow host, LEngine engine)`

Puts the panel to work on `engine` and shows the choices already stored in it, which is the last thing done before user changes start being saved.

## Inline notes

### `private PWindow _pSettingsHost = null!;`

The window this panel sits in, which is who asks before a workspace change throws typed work away, and who owns the other panels that change moves onto the new workspace.

### `private bool _pSettingsReady;`

Whether the stored choices have finished being applied. Applying them raises the same change events a user action does, and those must not be written back as choices.
