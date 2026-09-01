# PWorkspaceChoice.cs

## `public partial class PSettings`

The workspace folder the user picks in the settings panel: the path field and its browse button, and what a change to that path costs — a different database, so the form, the list, and the display all move onto the new workspace or the change is called off.

## Inline notes

### `if (!_pSettingsHost.PWindowDiscardConfirm())`

Changing the workspace throws the form away with it, and this runs from a mere LostKeyboardFocus on the path box — tabbing past it must not cost the user what they typed.

### `PWorkspacePath.Text = _lEngine.LEngineWorkspaceRead();`

An unusable path (permission, invalid characters) leaves the previous workspace in place; restore the field so it keeps showing the folder actually in use.

### `_pSettingsHost.PInput.PInputReset();`

The new workspace has its own database, so everything on screen came from a database that is no longer open and carries ids that mean nothing here. The input form is emptied, and the list and display are re-read from the new workspace.
