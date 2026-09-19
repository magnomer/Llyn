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

## `private void PWorkspacePathHandle(object sender, KeyEventArgs e)`

Enter applies the typed path and Escape puts the folder in use back into the field.
Nothing else applies it, so a half-typed path never becomes a folder on disk.

## `private void PWorkspaceFocusHandle(object sender, KeyboardFocusChangedEventArgs e)`

Leaving the field without Enter puts the folder in use back into it.
The field therefore never shows a path that is not the workspace.

## `private void PWorkspaceApply(string chosen)`

Moves onto `chosen` once the user has agreed to lose the form.
The panel controls, the language, the panel widths and the view are then aligned to the workspace moved onto.
The widths are reset before restoring, so a workspace without stored widths opens at the markup widths.

## Inline notes

### `if (!_pSettingsHost.PWindowDiscardConfirm())`

Changing the workspace throws the form away with it.
The user must say so before it happens.

### `PWorkspacePath.Text = _lEngine.LEngineWorkspaceRead();`

An unusable path leaves the previous workspace in place.
Permission and invalid characters are such faults.
Restore the field so it keeps showing the folder actually in use.
The fault is reported as well, because a field that reverts on its own tells the user nothing about why.

### `_pSettingsHost.PWindowViewRestore(_lEngine.LEngineStateRead());`

The new workspace carries its own view state.
The shell is put onto it as well as onto its database.
Restoring an ordering re-lists the catalog it orders.
Each panel ends on the new workspace's order rather than the old one's.
That is all this handler does after the change.
Emptying and re-reading is the engine's announcement, not a list kept here.

### `_lEngine.LEngineWorkspaceChange(path);`

The new workspace has its own database.
So everything on screen came from a database that is no longer open.
It carries ids that mean nothing here.
The engine says so once, and every panel empties its form, its list, and its display on hearing it.
