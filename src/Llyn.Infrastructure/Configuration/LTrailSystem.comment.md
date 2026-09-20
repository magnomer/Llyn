# LTrailSystem.cs

## `public sealed class LTrailSystem : LTrail`

The trail port answered by `System.IO.Path`, the rules of the operating system the program runs on.
It never touches the disk, so every answer is a pure function of its text.

## `public string? LTrailResolve(string root, string path)`

A fully qualified path is returned in full form as it is.
Any other path is anchored under `root` with its separator, and one that climbs back out resolves to nothing.
Text the system cannot read as a path resolves to nothing rather than throwing.

## `public string? LTrailRelativeResolve(string root, string path)`

The relative form, or nothing when the system answers with a rooted path or a climb out of `root`.

## `public string LTrailNameRead(string path)`

The file name, a trailing separator trimmed first so a folder path names the folder.

## `public bool LTrailRootCheck(string path)`

`Path.IsPathRooted`, as the system sees it.

## `public string LTrailNameNormalize(string name)`

Every character the system bars from a file name becomes an underscore.
