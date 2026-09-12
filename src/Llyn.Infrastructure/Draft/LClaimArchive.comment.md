# LClaimArchive.cs

## `public static class LClaimArchive`

Keeps the claims running programs hold on tentative records, one file per draft in `drafts/claim`.
A claim on disk is what lets a second launch tell live work from what a crash left behind.
An in-memory set answers that only for the copy of the program holding it.
The folder sits under `drafts` because a claim means nothing once the draft it names is gone.
Files are named after draft ids, so a listing of the drafts folder never picks them up.
The folder is never locked and nothing is held open, since two copies of the program write here at once.

## `public static LClaim LClaimArchiveCreate(long draftId)`

A claim on `draftId` naming the process running this code and the instant it started.
The start time travels with the id because the operating system hands a freed id to the next program.
It is stored as the universal instant, not the local one.
A workspace read across a daylight-saving change still recognises its own claims.
Nothing is written here, so a caller can compose a claim without touching the workspace.

## `public static void LClaimArchiveSave(string root, LClaim claim)`

Writes `claim` to `drafts/claim/<LClaimDraft>.json`, replacing whatever was there.
The text goes to a `.json.tmp` file first and is then moved over the target.
A move is atomic, so the other copy of the program never reads half a claim.

## `public static LClaim? LClaimArchiveRead(string root, long draftId)`

The claim held on `draftId`, or `null` when no readable file holds one.
No claim and an unknown claim are the same answer: nobody can be shown to hold the draft.

## `public static IReadOnlyList<LClaim> LClaimArchiveScan(string root)`

Every claim the folder holds.
A file that fails to parse or cannot be opened is skipped rather than thrown.
A crash is exactly when a truncated file is likely, and one bad file must not hide the rest.

## `public static void LClaimArchiveDelete(string root, long draftId)`

Drops the claim on `draftId`, which is how a hold ends once the draft is committed, cancelled, or deleted.
A file already gone, or held open by the other copy of the program, is not an error.

## `public static bool LClaimArchiveCheck(string root, long draftId)`

Whether a running program still holds `draftId`.
True demands all three: a claim exists, its process is running, and that process started when the claim says.
A claim whose process is gone is what a crash leaves behind.
So is one whose id now belongs to a later program.
Such a claim is deleted here rather than left to be re-examined at every launch.
So the answer doubles as the sweep, and a stale hold never outlives the first question asked about it.

## `private static bool LClaimArchiveMatch(LClaim claim)`

Whether the process a claim names is the one that wrote it.
The start times are compared with a second of slack.
The operating system reports them at coarser resolution than they are stored.
Both sides are taken as universal instants, so the comparison does not move when the clock the machine shows does.
A process that cannot be found, has exited, or refuses to say when it started answers no.
Refusing to answer counts as gone: a claim nothing can confirm must not hold work back forever.

## `private static LClaim? LClaimArchiveLoad(string path)`

One claim file read back, or `null` when it is missing, truncated, or unknown.

## Inline notes

### the extension check inside the listing

A Windows search pattern can match a file whose extension merely starts with the one asked for.
The half-written `.json.tmp` file must never be read as a claim.
