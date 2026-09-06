# PWorkspaceChoice.cs

## `public partial class PSettings`

The workspace folder the user picks in the settings panel.
That is the path field and its browse button.
A change to that path costs a different database.
So the form, the list, and the display all move onto the new workspace.
The engine announces the move, and each panel puts itself back on its own.
This file names no panel, so a panel added later moves with the rest and nothing here is edited.
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
The fault is reported as well, because a field that reverts on its own tells the user nothing about why.

### `_pSettingsHost.PWindowViewRestore(_lEngine.LEngineStateRead());`

The new workspace carries its own view state, so the shell is put onto it as well as onto its database.
Restoring an ordering re-lists the catalog it orders, so each panel ends on the new workspace's order rather than the old one's.
That is all this handler does after the change: emptying and re-reading is the engine's announcement, not a list kept here.

### `_lEngine.LEngineWorkspaceChange(path);`

The new workspace has its own database.
So everything on screen came from a database that is no longer open.
It carries ids that mean nothing here.
The engine says so once, and every panel empties its form, its list, and its display on hearing it.
