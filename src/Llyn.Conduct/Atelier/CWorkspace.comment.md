# CWorkspace.cs

## `public sealed class CWorkspace`

The workspace folder's life cycle within the session: opening it and leaving it.
Opening raises the events every view restores on.
Leaving asks the areas that hold drafts before the quit question.
The atelier builds it once and keeps the open and quit gates.
A driver reaches it to subscribe and to change the workspace.

## `private readonly List<(Func<bool> LWorkspacePending, Func<bool, bool> LWorkspaceClosure)> _cWorkspaceDrafts = [];`

Every panel area's unsaved-work verdict and its finish, in the order the areas registered.
Each area registers itself in its constructor, so no driver hands a seam up.

## `private readonly List<Action> _cWorkspaceVistas = [];`

Every panel area's vista restore, in the order the areas registered.
A workspace change runs them all, so every area stands on the new workspace's vistas.

## `private CEditor? _cWorkspaceInput;`

The input tab's editor, learned when the atelier restores its vista.
It has no area of its own, so the quit asks it directly and a workspace change restores it directly.

## `private bool _cWorkspaceHeard;`

Whether the ledger and status bulletins are heard yet.
They attach on the first open, since an atelier is also built over fake ports that answer no attach.

## `internal CWorkspace(CAtelier atelier)`

Keeps the atelier, whose ports, ledger and bulletins the life cycle reaches.

## `public event Action? CWorkspaceOpened;`

Raised on every open once the workspace is swept, for every view to restore itself.
A driver subscribes each view in its introduce, so one gate call fans out without a second request.

## `internal event Action<CWorkspaceState>? LWorkspaceStateOpened;`

Raised after `CWorkspaceOpened` with the entries the duplex wings last stood on.
The views restore first, so a wing lists before it opens its entry.
Each `CWing` subscribes itself, so no driver hands the state on.

## `public event Action<CEstablishment>? CWorkspaceEstablishmentChanged;`

The workspace's size and unsaved work, raised on every open and after every bulletin.
A read that fails raises nothing, so a busy or closing workspace leaves the strip as it stood.

## `public string CWorkspaceChange(string chosen, CEnvoy envoy)`

Moves the session onto the workspace the raw `chosen` path names, and answers the folder then in use.
The engine judges the path, so a blank path or the folder already in use asks nothing and moves nothing.
Otherwise the quit asks first, since the move drops every open form.
It is the one leave question the window's closing asks, put through `envoy` only for unsaved work.
One engine call moves, records, sweeps and answers the new workspace's state.
A move that fails is shown through `envoy` as `Workspace.OpenFailed`, and the old workspace stays.
The restore plan follows a move: every area's vistas, then the open events, then the stored tab.
The driver paints the answered folder, so a declined or failed move puts the field back.

## `public string CWorkspaceChange(CEnvoy envoy)`

Asks `envoy` for the folder to move onto, starting from the folder in use.
It then moves as the path gate does.
The question is asked inside the failure policy, so a dialog that fails shows `Workspace.OpenFailed`.
A declined question moves nothing and answers the folder in use.

## `internal void LWorkspaceOpen(CWorkspaceState state)`

The open's order after the sweep: the views, the ledger, the status, the wings' `state`, then the stored tab.
The ledger and status bulletins are attached on the first open only.
`CAtelierOpen` calls it at startup, and `CWorkspaceChange` after a move.

## `internal bool LWorkspaceQuitConfirm(CEnvoy envoy)`

Decides whether the session may end over every area that holds a draft.
The input editor and every registered area are asked whether they hold unsaved work.
Each is asked even after one answered yes, because the asking writes a pause-held keystroke down.
Unsaved work is put to the user once, through `envoy`'s `CEnvoyLeaveConfirm`.
Nothing unsaved closes every area without a question, as a discard.
Storing commits every draft and discarding cancels every one.
Staying closes nothing and answers no.
Every area is told before the answers are read, so one refusal leaves no other draft held.
The session may end only when all of them are finished.
A save the engine refuses answers no, so the window stays over the entry it failed to store.

## `internal void LWorkspaceDraftAdd(Func<bool> pending, Func<bool, bool> closure)`

Registers one area's unsaved-work verdict and its finish for the quit.
Only the areas call it, each once from its constructor.

## `internal void LWorkspaceVistaAdd(Action restore)`

Registers one area's vista restore for a workspace change.
Only the areas call it, each once from its constructor.

## `internal void LWorkspaceClosureAdd(Action closure)`

Registers one area's own close for the window's exit.
Only the areas call it, each once from its constructor.

## `internal void LWorkspaceClose()`

Runs every registered area close, in the order the areas were built.
Only the exit gate `CAtelier.CAtelierClose` calls it, before the leftover sweep.

## `private void LWorkspaceVistaRestore()`

Restarts every registered area's vistas, then the input tab's, on the workspace just moved onto.
Only `CWorkspaceChange` calls it, before the open events, so each view restores onto a started vista.

## `internal void LWorkspaceInputSet(CEditor editor)`

Keeps the input tab's editor, which `LAtelierInputRestore` hands over on every restore.

## `private void LWorkspaceEstablishmentRaise()`

Reads the status and raises `CWorkspaceEstablishmentChanged` with it.
A read that fails is skipped, so a busy or closing workspace leaves the strip as it stood.
