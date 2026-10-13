# Draft

This folder holds the ports for work in progress that lives outside the database.
They cover drafts, the claims running programs leave on them, the court of tentative links and the window posture.
It stands apart because these files live in the workspace draft folder, not in the lexicon store.
The engine asks by id and never learns how any of them is kept.

## `LDraftVault`

The persistence port for drafts, the editable copies the engine holds between load and save.
`LDraftArchive` in Infrastructure is its adapter over the workspace draft folder.
Saving replaces any earlier copy under the id.
Delete stays quiet when the draft is already gone.

## `LDraftRead`

Reads the draft with the id, or null when none is stored or the stored one is unreadable.

## `LDraftSweep`

Drops stale half-written drafts and moves unreadable ones aside.
It returns the ids of the drafts moved aside.

## `LClaimVault`

The persistence port for claims, the marks running programs leave on the drafts they hold.
A claim outside memory is what lets a second launch tell live work from what a crash left behind.
`LClaimArchive` in Infrastructure is its adapter over the workspace draft folder.
The engine asks by draft id and never learns how a claim is kept or how a process is recognised.

## `LClaimCreate`

A claim on the draft naming the running program and the instant it started.
Nothing is written, so a caller can compose a claim without touching the workspace.

## `LClaimRead`

The claim held on the draft, or null when none is readable.
No claim and an unknown claim are the same answer.
Nobody can be shown to hold the draft.

## `LClaimScan`

Every readable claim the workspace holds.
An unreadable one is skipped rather than thrown, since a crash is exactly when one is likely.

## `LClaimDelete`

Drops the claim, which is how a hold ends once the draft is committed, cancelled, or deleted.
A claim already gone is not an error.

## `LClaimCheck`

Whether a running program still holds the draft.
A claim whose program is gone is what a crash leaves behind.
Such a claim is dropped here rather than re-examined at every launch.

## `LCourtVault`

The persistence port for the court, the tentative links a draft holds to records that are not yet real.
A translation may name an entry the user has not written yet, and that link cannot be a database row.
A link waits here until the record it points at becomes real or is dropped.
`LCourtArchive` in Infrastructure is its adapter over the workspace draft folder.
Saving replaces whatever was under the id, and delete stays quiet when the link is gone.

## `LCourtScan`

Every readable link the court holds.
An unreadable one is skipped rather than thrown, so one bad file never hides the rest.

## `LCourtSettle`

Settles every link pointing at a draft, whether that draft became a real entry or was abandoned.
The links leave the court and are returned, each naming the draft that owns it.
Rewriting that draft is left to the caller, because the engine owns the draft.

## `LCourtSweep`

Drops the half-written and unreadable leftovers of the court that no read would ever pick up.
A link another copy of the program is still writing is left alone.

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

Commit: `c2f335c331fe`
- `LClaimVault.cs` `1575023eda2b4c52`
- `LCourtVault.cs` `72b17901d8e8b2c9`
- `LDraftVault.cs` `7f358df47b4aeaa4`
- `LPostureVault.cs` `66951a9c0840b428`
