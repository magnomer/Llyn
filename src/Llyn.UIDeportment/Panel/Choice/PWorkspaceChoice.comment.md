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

Hands `chosen` to `CAtelierWorkspaceChange`, which trims it, checks it and asks to lose the form.
The gate answers the view state of the workspace moved onto, or nothing when no move was made.
The ledger repaints this panel on the engine's workspace bulletin, so nothing here reads the settings again.
The widths, every panel's vistas and the view are then put on the workspace moved onto.
The view goes through `CAtelierOpen`, the same gate the window opens with.
The widths are reset before restoring, so a workspace without stored widths opens at the markup widths.

## Inline notes

### `state = PSettingsAtelier.CAtelierWorkspaceChange(chosen, _pSettingsHost.PWindowEnvoy);`

The new workspace has its own database.
So everything on screen came from a database that is no longer open.
It carries ids that mean nothing here.
The engine says so once, and every panel empties its form, its list, and its display on hearing it.
The envoy asks the user first, since changing the workspace throws the form away with it.

### `PWorkspacePath.Text = _pSettingsState.CLedgerStatePath;`

An unusable path leaves the previous workspace in place.
Permission and invalid characters are such faults.
Restore the field to the folder last painted, so it keeps showing the folder actually in use.
The fault is reported as well, because a field that reverts on its own tells the user nothing about why.
A blank path, the same path or a declined question also restores the field, and reports nothing.

### `PSettingsAtelier.CAtelierOpen();`

The new workspace carries its own view state.
`CAtelierOpen` sweeps its leftovers and puts the shell onto it as well as onto its database.
Restoring an ordering re-lists the catalog it orders.
Each panel ends on the new workspace's order rather than the old one's.
