# LCapsule.cs

## `public sealed class LCapsule`

Deportment's own storage for the GUI-only state that outlives a run.
It keeps one JSON file under the workspace root, so the state moves with the workspace.
It holds no rule about what the state means, and Deportment clamps, propagates and decides.

## `private const string LCapsuleName = "capsule.json";`

The file's name under the workspace root.

## `private static readonly JsonSerializerOptions LCapsuleOptions = new()`

Writes the file indented, so a file read by hand stays legible.

## `public LCapsuleContent LCapsuleRead(string? root)`

The state kept under `root`, or the default state when no file is there.
A null or blank root means no file, so it answers the default too.
A file that cannot be read or parsed also answers the default, so the window always opens.

## `public void LCapsuleSave(string? root, LCapsuleContent state)`

Replaces the state kept under `root` whole.
A null or blank root has no file, so nothing is written.
It writes a pending file first and moves it over the old one.
A crash therefore never leaves half a file.
A folder that cannot be written throws, and the caller decides what a lost save means.
