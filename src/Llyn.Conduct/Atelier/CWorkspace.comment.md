# CWorkspace.cs

## `public sealed class CWorkspace`

The workspace folder's life cycle within the session: opening it and leaving it.
Opening raises the events every view restores on.
Leaving asks the areas that hold drafts before the quit question.
The atelier builds it once and keeps the gates, so a driver reaches it only to subscribe.

## `private readonly List<(Func<bool> LWorkspacePending, Func<bool, bool> LWorkspaceClosure)> _cWorkspaceDrafts = [];`

Every panel area's unsaved-work verdict and its finish, in the order the areas registered.
Each area registers itself in its constructor, so no driver hands a seam up.

## `private CEditor? _cWorkspaceInput;`

The input tab's editor, learned when the atelier restores its vista.
It has no area of its own, so the quit asks it directly.

## `private bool _cWorkspaceHeard;`

Whether the ledger and status bulletins are heard yet.
They attach on the first open, since an atelier is also built over fake ports that answer no attach.

## `internal CWorkspace(CAtelier atelier)`

Keeps the atelier, whose ports, ledger and bulletins the life cycle reaches.

## `public event Action? CWorkspaceOpened;`

Raised on every open once the workspace is swept, for every view to restore itself.
A driver subscribes each view in its introduce, so one gate call fans out without a second request.

## `public event Action<CWorkspaceState>? CWorkspaceStateOpened;`

Raised after `CWorkspaceOpened` with the entries the duplex wings last stood on.
The views restore first, so a wing lists before it opens its entry.

## `public event Action<CEstablishment>? CWorkspaceEstablishmentChanged;`

The workspace's size and unsaved work, raised on every open and after every bulletin.
A read that fails raises nothing, so a busy or closing workspace leaves the strip as it stood.

## `internal void LWorkspaceOpen(CWorkspaceState state)`

The open's order after the sweep: the views, the ledger, the status, then the wings' `state`.
The ledger and status bulletins are attached on the first open only.
Only `CAtelierOpen` calls it.

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

## `internal void LWorkspaceInputSet(CEditor editor)`

Keeps the input tab's editor, which `CAtelierInputRestore` hands over on every restore.

## `private void LWorkspaceEstablishmentRaise()`

Reads the status and raises `CWorkspaceEstablishmentChanged` with it.
A read that fails is skipped, so a busy or closing workspace leaves the strip as it stood.
