# LClaimVault.cs

## `public interface LClaimVault`

The persistence port for claims, the marks running programs leave on the drafts they hold.
A claim outside memory is what lets a second launch tell live work from what a crash left behind.
`LClaimArchive` in Infrastructure is its adapter over the workspace draft folder.
The engine asks by draft id and never learns how a claim is kept or how a process is recognised.

## `LClaim LClaimCreate(long draftId);`

A claim on `draftId` naming the running program and the instant it started.
Nothing is written, so a caller can compose a claim without touching the workspace.

## `void LClaimSave(LClaim claim);`

Stores `claim` under its draft id, replacing whatever was there.

## `LClaim? LClaimRead(long draftId);`

The claim held on `draftId`, or `null` when none is readable.
No claim and an unknown claim are the same answer: nobody can be shown to hold the draft.

## `IReadOnlyList<LClaim> LClaimScan();`

Every readable claim the workspace holds.
An unreadable one is skipped rather than thrown, since a crash is exactly when one is likely.

## `void LClaimDelete(long draftId);`

Drops the claim on `draftId`, which is how a hold ends once the draft is committed, cancelled, or deleted.
A claim already gone is not an error.

## `bool LClaimCheck(long draftId);`

Whether a running program still holds `draftId`.
A claim whose program is gone is what a crash leaves behind.
Such a claim is dropped here rather than re-examined at every launch.
