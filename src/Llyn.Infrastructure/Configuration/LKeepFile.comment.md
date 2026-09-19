# LKeepFile.cs

## `public sealed class LKeepFile : LKeep`

The disk behind the keep port: one `{name}.json` file per name under the workspace root.
The root is fixed at construction, so the engine builds a fresh one for whatever workspace stands open.
It is written under a pending name and moved into place, so a crash mid-write leaves the old file whole.
That is the same move the settings loader makes, so both files survive a crash the same way.

## `public string? LKeepRead(string name)`

A missing or unreadable file reads as nothing, so the owner falls back to its defaults and starts.
