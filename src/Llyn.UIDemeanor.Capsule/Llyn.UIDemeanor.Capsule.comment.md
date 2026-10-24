# Llyn.UIDemeanor.Capsule

Demeanor's own storage for the console-only state that outlives a run.
It keeps one JSON file under the workspace root, so the state moves with the workspace.
It holds no rule about what the state means, and Demeanor validates, falls back and decides.

The storage reads and writes files alone, so it builds for every platform.
It names no Llyn project, so it compiles against nothing beyond .NET.

## `LCapsuleConsole`

The file is `console.json` under the workspace root, written indented so a hand-read file stays legible.

`LCapsuleConsoleRead` answers the state kept under the root, or the default state when no file is there.
A null or blank root means no file, so it answers the default too.
A file that cannot be read or parsed also answers the default, so the console always opens.

`LCapsuleConsoleSave` replaces the stored state whole.
A null or blank root has no file, so nothing is written.
It writes a pending file first and moves it over the old one.
A crash therefore never leaves half a file.
A folder that cannot be written throws, and the caller decides what a lost save means.

## `LCapsuleSetting`

The console-only state one workspace keeps between runs.
The stored keys are a data contract, so they stay lowercase and independent of member names.
Each member is nothing until first stored.
The theme, glyph and ambiguous members hold names, and Demeanor checks each name before it uses it.
The folder member holds the folder the path prompt last accepted.

## Stamp

Commit: `78fc0ebfff7c`
- `LCapsuleConsole.cs` `d876aac5d0f89340`
- `LCapsuleSetting.cs` `38a4c0859e1a71e9`
- `Llyn.UIDemeanor.Capsule.csproj` `c26a2a97a04873fe`
