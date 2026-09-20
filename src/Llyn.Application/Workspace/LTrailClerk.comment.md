# LTrailClerk.cs

## `public sealed class LTrailClerk`

Paths and locations resolved against one workspace root.
It holds the trail, the usher and the root of one rig, so the engine names no path port itself.
A location is a web address or a file path, absolute or relative to the workspace.
The same resolution serves recordings, images, videos and the markup import.

## `public LTrailClerk(LRig rig)`

Reads the trail, the usher and the workspace root out of `rig`.

## `public string LTrailClerkWorkspace`

The workspace root the clerk resolves against.

## `public string LWorkspaceFormat()`

The workspace shown by its folder name, or by the full root when the name is empty.

## `public string LTrailNameNormalize(string name)`

A file name made safe for the file system, through the trail.

## `public bool LTrailRootCheck(string path)`

Whether `path` is rooted, so a flag code that is a path is told apart from a remote code.

## `public bool LTrailPathCheck(string path)`

Whether the file or folder at `path` exists, through the usher.

## `public string? LTrailPathResolve(string path)`

The full path of `path` under the workspace, or null when the trail refuses it.

## `public Uri? LTrailClerkResolve(string? location)`

A web address stays a web address.
A path that starts with a separator is refused, since it names no drive and no workspace.
A drive-qualified path is taken as a file and any other scheme is refused.
The rest is resolved as a workspace-relative file.

## `public Uri? LTrailFileResolve(string path)`

A file path resolved against the workspace root into an absolute file address.
A path the trail cannot place, or one the Uri parser refuses, answers null.

## `public Uri? LTrailClerkRead(string? location)`

The resolved location, or null when it is a file that does not exist.

## `public void LTrailClerkOpen(string target)`

Opens `target` through the shell usher.

## `public string LRecordingResolve(string file)`

The recording path as an absolute local path, or the text as given when it is not a file.

## `public bool LRecordingExist(string? file)`

Whether the recording resolves to a file that exists.

## `public string LRecordingFormat(string path)`

The recording path made relative to the workspace, so a moved workspace keeps its audio.
A path outside the workspace is stored as given.
