# LUsherShell.cs

## `public static class LUsherShell`

Opens a folder or a web address in the operating-system shell.
This lives beside the press because starting a process is a platform capability, not a panel.
The veneer never starts a process itself, so every launch passes through here.
A launch that fails throws, and the caller shows the failure.

## `public static void LUsherShellOpen(string target)`

Hands the target to the shell.
The shell picks the file manager for a folder and the default browser for an address.
