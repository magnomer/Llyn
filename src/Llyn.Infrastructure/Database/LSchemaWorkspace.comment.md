# LSchemaWorkspace.cs

## `public static class LSchemaWorkspace`

The schema step that gives the workspace row the shell's ordering choices.
A workspace written before the shell stored its own view state carries the panes and the revision alone.
The orderings a panel was listing by died with the process.

## `public static void LSchemaWorkspaceNormalize(SqliteConnection connection)`

Adds one ordering column per browse panel to `workspace`, and clears a split that names neither side.
A column is added only when it is missing, so the step may run over a workspace that already carries it.
The rows carry over untouched.
An ordering that was never stored stands empty, and the panel falls back to the ordering it opens on.

## `private static readonly string[] LSchemaWorkspaceColumn`

The ordering columns the step adds, named after the panel each one belongs to.

## `private static bool LSchemaWorkspaceFind(SqliteConnection connection, string column)`

Reports whether `column` already stands on the workspace table.
