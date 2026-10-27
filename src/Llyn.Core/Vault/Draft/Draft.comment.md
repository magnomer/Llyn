# Draft

This folder holds the ports for work in progress that lives outside the database.
They cover drafts, the claims running programs leave on them, the court of tentative links and the window posture.
It stands apart because these files live in the workspace draft folder, not in the lexicon store.
The engine asks by id and never learns how any of them is kept.

## `LDraftVault`

The one port for the dossier.
The dossier is the drafts the engine holds between load and save, with their claims and court links.
`LDossierEntrance` in Infrastructure is its adapter over the workspace draft folder.
One port keeps the three together, because ending a draft must drop its claim and its links in one step.
Saving replaces any earlier copy under the id.
Delete and finish stay quiet when the draft is already gone.

### `LDraftStart`

Saves a new draft and claims it for this program in one call.
So a draft never stands on disk without its claim.
No second launch then mistakes live work for a crash leftover.

### `LDraftRead` and `LDraftScan`

Read the draft with the id, or null when none is stored or the stored one is unreadable.
The scan returns every readable draft, and an unreadable one is skipped rather than thrown.

### `LDraftClaimCheck`

Whether this running program holds the draft.
A claim whose program is gone is what a crash leaves behind, and it reads false.

### `LDraftForeignCheck`

Whether another running program holds the draft.
It lets a launch leave a live draft alone instead of offering it back as a leftover.

### `LDraftFinish`

Drops the claim and the draft, which is how a hold ends once the draft is committed.
The claim goes first, so no claim outlives its draft.

### `LDraftDelete`, `LDraftCancel` and `LDraftSweep`

Each takes the set of drafts the engine holds and answers an `LDraftRelease`.
The release names the drafts that ended and, for a cancel, the owners whose content was rewritten.
The engine owns that set, so the vault never guesses which drafts are open in a panel.
A delete removes the draft and the court links it owns.
A cancel does the same and also settles the links that pointed at the draft, with no real record.
A held draft that only the removed links pointed at ends with it, unless another link still points at it.
A sweep drops stale half-written drafts and court files, then cancels each draft it dropped.
It also drops every claim whose draft is gone.

## Court

The dossier also keeps the court, the tentative links a draft holds to records that are not yet real.
A translation may name an entry the user has not written yet, and that link cannot be a database row.
A link waits here until the record it points at becomes real or is dropped.
Saving replaces whatever was under the id, and delete stays quiet when the link is gone.

## `LDraftCourtScan`

Every readable link the court holds.
An unreadable one is skipped rather than thrown, so one bad file never hides the rest.

## `LDraftCourtSettle`

Settles every link pointing at a draft, whether that draft became a real entry or was abandoned.
Each owner draft is rewritten in place with the real id, or with zero when the draft was abandoned.
The owners rewritten are returned, so the engine can clear what it kept for them.

## `LPostureVault`

The port for the posture kept under the workspace root, how the main window stands.
`LPostureFile` in Infrastructure is its adapter over the keep file and the JSON the posture is written in.
The posture reads and writes the `LPostureState` record and never sees the text.

## `LPostureRead`

The posture kept under the name, or nothing while none was ever written.
Text that is not a posture reads as the default posture, so the window always opens.
A file that exists but cannot be read raises `LVaultFault`.

## `LPostureSave`

Replaces the posture kept under the name whole.
A file that cannot be written raises `LVaultFault`.

## Stamp

Commit: `1f3467bbe5c6`
- `LDraftVault.cs` `3ffb7473732147f8`
- `LPostureVault.cs` `66951a9c0840b428`
