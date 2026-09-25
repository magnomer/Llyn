# LUsherShell.cs

## `public sealed class LUsherShell : LUsher`

The usher the composition root hands the engine.
Path facts go to the inner usher, and the launch runs here.
This lives beside the press because starting a process is a platform capability, not a file.
The veneer never starts a process itself, so every launch passes through the engine to here.

## `public LUsherShell(LUsher inner)`

Takes the usher that answers path facts.

## `public void LUsherOpen(string target)`

Hands the target to the operating-system shell.
The shell picks the file manager for a folder and the default browser for an address.
A launch that fails throws, and the caller shows the failure.
