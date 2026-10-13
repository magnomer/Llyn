# Llyn.UIDeportment.Capsule

Deportment's own storage for the GUI-only state that outlives a run.
It keeps one JSON file under the workspace root, so the state moves with the workspace.
It holds no rule about what the state means, and Deportment clamps, propagates and decides.

The storage reads and writes files alone, so it builds for every platform.
A Windows-only call turns the build red through CA1416, so the platform stays out of the storage.
It stands above the cut and names no Llyn project, so it compiles against nothing beyond .NET.

## `LCapsule`

The file is `capsule.json` under the workspace root, written indented so a hand-read file stays legible.

`LCapsuleRead` answers the state kept under the root, or the default state when no file is there.
A null or blank root means no file, so it answers the default too.
A file that cannot be read or parsed also answers the default, so the window always opens.

`LCapsuleSave` replaces the stored state whole.
A null or blank root has no file, so nothing is written.
It writes a pending file first and moves it over the old one.
A crash therefore never leaves half a file.
A folder that cannot be written throws, and the caller decides what a lost save means.

## `LCapsuleContent`

The GUI-only state one workspace keeps between runs.
The stored keys are a data contract, so they stay lowercase and independent of member names.
The linked flag says whether dragging a panel in one tab sets the same width in every tab.
It is on by default.
The window, column and press members are nothing until first stored.

## `LCapsuleColumn`

The widths of one tab's fixed panels, keyed by the tab's lowercase name.
The last column takes whatever room the window leaves, so it is never stored.
The left and middle widths are nothing before a panel was ever dragged.
The middle width is also nothing where a tab has fewer than three panels.

## `LCapsulePress`

The print window's size between runs, which only the GUI needs.
The print setting is kept by the engine's store, so Capsule holds no print data.
A `setting` key an older file carries is ignored on read.
The stored keys are a data contract, so they stay lowercase and independent of member names.

## `LCapsuleWindow`

The main window's restored rectangle and whether it stood maximized when it closed.

## Stamp

Commit: `c2f335c331fe`
- `LCapsule.cs` `4ee2424a2987f7df`
- `LCapsuleColumn.cs` `44bd0b0b1df9a41e`
- `LCapsuleContent.cs` `f8a00a21a8ac6543`
- `LCapsulePress.cs` `635404bd8698189a`
- `LCapsuleWindow.cs` `1821cf8a791ff226`
- `Llyn.UIDeportment.Capsule.csproj` `c26a2a97a04873fe`
