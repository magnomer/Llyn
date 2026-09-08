# TClaim.cs

## `public sealed class TClaim`

Covers the claim files, the record on disk of which running program holds which tentative draft.
An in-memory set answers that for one copy of the program only, and two copies may run on one workspace.
The tests spawn a real child process.
A claim is only worth anything if the operating system agrees the holder is alive.

## `public void ClaimArchiveCheck_ProcessRunning_HoldsDraft()`

A claim naming a live process with its true start time reports the draft as held.
This is the case that must never be swept: it is a window somebody is typing into.

## `public void ClaimArchiveCheck_ProcessGone_SweepsClaim()`

A claim naming a process that has exited reports nothing held and its file is deleted.
This is what a forced shutdown leaves, and the draft under it is what recovery exists to offer back.

## `public void ClaimArchiveCheck_ProcessIdTakenOver_SweepsClaim()`

A claim naming a live process that started at another time is stale too.
The operating system hands a freed id to the next program.
The id alone would let an unrelated program hold work hostage forever.

## `public void ClaimArchiveCheck_DraftNothingClaims_AnswersFalse()`

A draft nobody claims reports nothing held.

## `public void LeftoverRead_ClaimHeldByAnotherProcess_PassesOverIt()`

A draft another running copy of the program claims is not offered as a leftover.
It becomes one the moment that copy is gone.
This is the whole point of writing the claim down.
The first copy's live work must not be shown to the second as wreckage.

## `public void DraftCommit_StoredDraft_DropsItsClaim()`

Committing a draft removes the claim with the draft file.
A claim outliving its draft would be swept only by luck, since nothing asks about a draft that is gone.

## `public void DraftCancel_DiscardedDraft_DropsItsClaim()`

Cancelling a draft removes its claim too.

## `private static string TClaimDraftCreate(TWorkspace workspace)`

Starts a draft through an engine that is then closed, leaving held work behind with content worth recovering.
An untouched draft is not a leftover whatever claims it.
The content has to differ from the entry it opened from.

## `private static Process TClaimProcessStart()`

A child process that stays alive until it is killed, standing in for the other copy of the program.
Its input is redirected so it waits rather than reading the test runner's console.

## `private static void TClaimProcessStop(Process held)`

Kills that child and waits for the operating system to release its id.
Reading the start time before the kill gives the tests a process that certainly existed.
That process certainly does not exist now.
