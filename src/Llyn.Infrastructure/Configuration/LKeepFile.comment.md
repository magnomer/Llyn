# LKeepFile.cs
Hash: `c6c78aa9341281fa`

## `public sealed class LKeepFile : LKeep`

The disk behind the keep port: one `{name}.json` file per name under the workspace root.
The root is fixed at construction, so the engine builds a fresh one for whatever workspace stands open.
It is written under a pending name and moved into place, so a crash mid-write leaves the old file whole.
That is the same move the settings loader makes, so both files survive a crash the same way.

## `public string? LKeepRead(string name)`

A missing file reads as nothing, so the owner falls back to its defaults and starts.
A file that exists but will not read throws.
The owner then keeps what it holds and does not write over the file.

## `public void LKeepSave(string name, string text)`

Creates the workspace folder first, so the first save into a fresh workspace succeeds.
A failed write throws the system's own exception, unlike the settings save, which wraps it in `LVaultFault`.
