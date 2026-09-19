# PMarkup.cs

## `internal static class PMarkup`

The library panel's import action: the pick of a markup file whose path is handed to the engine.
It owns no state and opens no file, because the workspace is the engine's to read.

## `internal static string? PMarkupOpen(Window owner)`

The file the reader picked, or null for a cancelled pick.
A cancelled pick is not a failure and leaves the workspace exactly as it was.
The format's own extension is offered first, with everything else still reachable.
Markup is plain text, so a file that carries it under another name is still importable.
