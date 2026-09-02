# PWorkspaceChoice.cs

## `public partial class PSettings`

The workspace folder the user picks in the settings panel.
That is the path field and its browse button.
A change to that path costs a different database.
So the form, the list, and the display all move onto the new workspace.
Otherwise the change is called off.

## Inline notes

### `if (!_pSettingsHost.PWindowDiscardConfirm())`

Changing the workspace throws the form away with it.
This runs from a mere LostKeyboardFocus on the path box.
Tabbing past it must not cost the user what they typed.

### `PWorkspacePath.Text = _lEngine.LEngineWorkspaceRead();`

An unusable path leaves the previous workspace in place.
Permission and invalid characters are such faults.
Restore the field so it keeps showing the folder actually in use.

### `_pSettingsHost.PInput.PInputReset();`

The new workspace has its own database.
So everything on screen came from a database that is no longer open.
It carries ids that mean nothing here.
The input form is emptied, and the list and display are re-read from the new workspace.
