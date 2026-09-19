# LKeep.cs

## `public interface LKeep`

A named text document kept under the workspace root.
It is the port a ring writes its own state through without knowing the disk.
The infrastructure gives each name a file, and a test gives it a dictionary.
The engine hands the port out, so an outer ring persists beside the engine rather than through it.

## `string? LKeepRead(string name)`

The text kept under the name, or nothing while none was ever written.

## `void LKeepSave(string name, string text)`

Replaces the text kept under the name whole.
