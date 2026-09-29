# QMarkup.cs

## `internal static class QMarkup`

The file dialog that answers the library import's file question for the envoy.
It owns no state and opens no file, because the workspace is the engine's to read.

## `internal static string? QMarkupConsult(Window owner)`

The file the reader picked, or null for a cancelled pick.
A cancelled pick is not a failure and leaves the workspace exactly as it was.
The format's own extension is offered first, with everything else still reachable.
Markup is plain text, so a file that carries it under another name is still importable.
