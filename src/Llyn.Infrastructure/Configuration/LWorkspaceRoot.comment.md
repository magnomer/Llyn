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

Records `path` as the workspace folder and creates it.
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
