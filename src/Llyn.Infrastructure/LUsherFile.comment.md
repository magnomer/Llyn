# LUsherFile.cs

## `public sealed class LUsherFile : LUsher`

Answers the `LUsher` port from the file system.
The engine builds one and hands it to `LEnsign`, so the cache half of the flag reaches no disk itself.
Opening a folder or a link lives in the media ring's `LUsherShell`, since a launch is platform work.

## `public bool LUsherPathExist(string? path)`

True when a file is present at the path, false for a null path.

## `public void LUsherPathDelete(string path)`

Removes a cached SVG the renderer could not read, so the next fetch replaces it.
A file already gone or held by another process is left as it is, since the next fetch retries anyway.
