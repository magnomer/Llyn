# LWorkspaceRoot.cs

## `public static class LWorkspaceRoot`

Resolves and persists the user's workspace folder — the single location that owns the user's settings and database. The chosen folder path is the one piece of state that cannot itself live in the workspace (it is what tells the program where the workspace is), so it is kept in a small pointer file under the user's application-data folder. Everything else the program persists goes inside the resolved workspace, never beside this pointer.

## `public static string LWorkspaceRootRead()`

Returns the current workspace folder, creating it if needed. When no folder has been chosen yet, a default under the user profile is used and recorded so later runs are stable.

## `public static void LWorkspaceRootChange(string path)`

Records `path` as the workspace folder and creates it. Subsequent settings and database access resolve against this folder.

## `public static string LWorkspaceDatabaseRead(string root)`

The database file path within `root`. The database lives only here.
