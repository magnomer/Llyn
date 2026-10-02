# QWorkspace.cs

## `internal sealed class QWorkspace`

The workspace folder the user picks in the settings panel.
That is the path field and its browse button.
A change to that path costs a different database.
So the form, the list, and the display all move onto the new workspace.
Conduct's workspace change gate owns the move, its question, its failure and its restore plan.
This driver hears the field and the button, and paints the folder the gate answers.
It finds its controls through `QContract.QContractFind` on the settings panel, keeping their markup names.

## `internal QWorkspace(FrameworkElement settings)`

Runs as the settings panel is built.
It sets the browse icon and wires the field's keys, its focus loss and the button once.
Escape is heard before Enter, as the panel always wired them.

## `internal void QWorkspaceIntroduce(CAtelier atelier, CEnvoy envoy)`

Puts the field to work on the host's atelier, and on its envoy for the move's questions.

## `internal void QWorkspacePathRefine(string path)`

Writes `path`, a folder Conduct answered, into the field.
The panel calls it with the folder in every ledger state.

## `private void QWorkspaceDialogObserve(object sender, RoutedEventArgs e)`

The browse button hands the move to `CWorkspaceChange`, which asks for the folder through the envoy.
The folder dialog is a question the gate asks, so its failure is the gate's to report.
The field then shows the folder in use, moved or not.

## `private void QWorkspacePathObserve(object sender, KeyEventArgs e)`

Enter hands the raw typed path to `CWorkspaceChange`, which judges and moves.
Nothing else applies it, so a half-typed path never becomes a folder on disk.
The field then shows the folder the gate answers, so a declined or failed move puts it back.

## `private void QWorkspaceEscapeRefine(object sender, KeyEventArgs e)`

Escape puts the folder in use back into the field and asks nothing.

## `private void QWorkspaceFocusRefine(object sender, KeyboardFocusChangedEventArgs e)`

Leaving the field without Enter puts the folder in use back into it.
The field therefore never shows a path that is not the workspace.
