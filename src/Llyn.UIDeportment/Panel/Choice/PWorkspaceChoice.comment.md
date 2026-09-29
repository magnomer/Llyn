# PWorkspaceChoice.cs

## `public partial class PSettings`

The workspace folder the user picks in the settings panel.
That is the path field and its browse button.
A change to that path costs a different database.
So the form, the list, and the display all move onto the new workspace.
Conduct's workspace change gate owns the move, its question, its failure and its restore plan.
This file hears the field and the button, and paints the folder the gate answers.

## `private void PWorkspaceDialogObserve(object sender, RoutedEventArgs e)`

The browse button hands the move to `CWorkspaceChange`, which asks for the folder through the envoy.
The folder dialog is a question the gate asks, so its failure is the gate's to report.
The field then shows the folder in use, moved or not.

## `private void PWorkspacePathObserve(object sender, KeyEventArgs e)`

Enter hands the raw typed path to `CWorkspaceChange`, which judges and moves.
Nothing else applies it, so a half-typed path never becomes a folder on disk.
The field then shows the folder the gate answers, so a declined or failed move puts it back.

## `private void PWorkspaceEscapeRefine(object sender, KeyEventArgs e)`

Escape puts the folder in use back into the field and asks nothing.

## `private void PWorkspaceFocusRefine(object sender, KeyboardFocusChangedEventArgs e)`

Leaving the field without Enter puts the folder in use back into it.
The field therefore never shows a path that is not the workspace.

## `private void PWorkspacePathRefine(string path)`

Writes `path`, a folder Conduct answered, into the field.
