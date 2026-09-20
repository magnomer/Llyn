# LWorkspaceRoot.cs

## `public static class LWorkspaceRoot`

Resolves and persists the user's workspace folder — the single location that owns the user's settings and database.
The chosen folder path is the one piece of state that cannot itself live in the workspace.
It is what tells the program where the workspace is.
So it is kept in a small pointer file under the user's application-data folder.
Everything else the program persists goes inside the resolved workspace, never beside this pointer.

## `public static string LWorkspaceRootRead()`

Returns the current workspace folder, creating it if needed.
When no folder has been chosen yet, a default under the user profile is used.
It is recorded so later runs are stable.

## `public static void LWorkspaceRootChange(string path)`

Records `path` as the workspace folder, resolved to its full form, and creates it.
The pointer then reads the same as the rig's root, whatever the user typed.
Subsequent settings and database access resolve against this folder.

## `public static string LWorkspaceDatabaseRead(string root)`

The database file path within `root`.
The database lives only here.

## `public static string LWorkspaceDraftRead(string root)`

The folder inside `root` that holds one file per tentative record, created if needed.
Drafts live beside the database and never inside it.
That is what lets unsaved work survive a forced shutdown.
The pronunciation cache under `temp` is a separate folder and is never reused for this.

## `public static string LWorkspaceCourtRead(string root)`

The folder inside the drafts folder that holds one file per tentative link, created if needed.
The court sits under `drafts` because a link is only meaningful while the draft it points at is still tentative.
Its files are named after link ids, so a listing of the drafts folder itself never picks them up.

## `public static string LWorkspaceBrokenRead(string root)`

The folder inside the drafts folder where the sweep sets aside a draft file it cannot read, created if needed.
It sits under `drafts` so the file stays beside the drafts it came from, out of the listing's way.

## `public static string LWorkspaceClaimRead(string root)`

The folder inside the drafts folder that holds one file per claim on a tentative record, created if needed.
A claim sits under `drafts` because it means nothing once the draft it names is gone.
Its files are named after draft ids, so a listing of the drafts folder itself never picks them up.

## `public static void LWorkspacePendingCommit(string pending, string path)`

Moves a fully written pending file over its final path, replacing what was there.
The move is retried a few times with a growing pause because Windows scanners hold a fresh file briefly.
That hold surfaces as an access-denied or sharing error on the very first move and clears within milliseconds.
The last attempt lets the error through, since a path that stays locked is a real fault.
Every draft, link and claim archive completes its save through this one place.

## `public static async Task LWorkspaceFileSave(string path, byte[] content, CancellationToken cancellation)`

Writes the bytes beside the target and moves them over it in one step.
A kill mid-write leaves a `.tmp` file, never a truncated recording or flag that would be served forever.
The recording archive and the language loader's flag cache both save through it.
