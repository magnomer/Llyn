# LUsherFile.cs
Hash: `4c0ba06ff2d16b37`

## `public sealed class LUsherFile : LUsher`

Answers the `LUsher` port from the file system.
The host wraps one in `LUsherShell` and hands that to the rig factory.
Opening a folder or a link lives in the media ring's `LUsherShell`, since a launch is platform work.

## `public bool LUsherPathExist(string? path)`

True when a file is present at the path, false for a null path.

## `public void LUsherPathDelete(string path)`

Removes a cached SVG the renderer could not read, so the next fetch replaces it.
A file already gone or held by another process is left as it is, since the next fetch retries anyway.

## `public bool LUsherLockCheck(Exception exception)`

True for an input or output failure or a refused access, found on the exception or any inner one.
A reader may wrap the failure, so the whole chain is walked.
A missing file or folder answers false, since there is nothing left to keep.

## `public void LUsherOpen(string target)`

Throws, because a file usher answers path facts only.
`LUsherShell` in the media ring wraps this usher and adds the launch.
