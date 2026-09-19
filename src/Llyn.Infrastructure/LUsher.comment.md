# LUsher.cs

## `public static class LUsher`

Opens a path or a web address in the operating-system shell.
The veneer never starts a process itself, so every launch passes through here.
A launch that fails throws, and the caller shows the failure.

## `public static void LUsherFolderOpen(string path)`

Opens a folder in the file manager the system associates with folders.

## `public static void LUsherLinkOpen(string address)`

Opens a web address in the default browser.

## `public static bool LUsherPathExist(string? path)`

True when a file is present at the path, false for a null path.
