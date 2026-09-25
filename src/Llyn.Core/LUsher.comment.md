# LUsher.cs

## `public interface LUsher`

The port through which the logic rings ask a fact about a path.
`LEnsign` decides which flags to keep and which cached file to drop, and never touches a disk itself.
`LUsherFile` in the infrastructure answers from the file system, and a test answers from a list.
Playing a recording is not a path fact, so it goes through its sibling port `LPhonograph`.

## `bool LUsherPathExist(string? path);`

True when a file is present at the path, false for a null path.

## `void LUsherPathDelete(string path);`

Removes the file at the path, and stays quiet when the file is already gone or locked.

## `bool LUsherLockCheck(Exception exception);`

True when `exception` says the system would not open a file, rather than that its content was bad.

## `void LUsherOpen(string target);`

Opens a folder or a web address for the user.
A launch that fails throws, and the caller shows the failure.
