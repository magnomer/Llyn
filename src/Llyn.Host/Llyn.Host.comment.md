# Llyn.Host

The composition root that builds the engine and hands it to a UI.
It builds the executable `Llyn.exe`, so the installed program keeps its name.
It stands apart because it is the only project that may name every project and every Windows twin.

## Two launchers

The user decided on one program with two launchers for the same `Llyn.dll`.
`Llyn.exe` has the window subsystem and opens the GUI with no console behind it.
`Llyn.com` has the console subsystem and opens the CUI in the terminal that typed it.
Windows tries `.COM` before `.EXE` in `PATHEXT`, so `llyn` typed in a terminal reaches `Llyn.com`.
A double click or a shortcut names `Llyn.exe`, so the desktop keeps the window.
One assembly means one build, one set of settings and one version for both surfaces.
The subsystem is fixed in the launcher's header, so the process cannot change it once started.
So the launcher is the choice, and Guestroom reads which one started it.

A publish is single-file, so `Llyn.exe` holds the runtime and every assembly and no `Llyn.dll` sits beside it.
So the published `Llyn.com` is the native launcher from `Llyn.Launcher`, not an apphost.
It starts `Llyn.exe` on its own console and marks the start with `LLYN_LAUNCHER`.
`Llyn.exe` keeps the window subsystem, so it attaches to that console before the CUI opens.
A plain build still places the apphost `Llyn.com` beside `Llyn.dll`, where it starts the CUI directly.

## `Llyn.Host.csproj`

It holds no behaviour, and each UI's entry point adds only what depends on its medium.
It takes the veneer's framework, runtime identifiers and icon, since it is now the executable.
It uses WPF because it starts the veneer's application on its own thread.
The Release publish is precompiled with ReadyToRun.
Precompiled code removes first-use JIT from launch and first clicks.
It references both UI families, since one assembly runs behind both launchers.
It references Guestroom, which picks the UI a start opens.

## `LHostLauncherCreate`

Makes `Llyn.com`, the console launcher, right after the SDK makes `Llyn.exe`.
It calls the SDK's own `CreateAppHost` task with the same inputs, so both launchers come from one template.
Only the subsystem differs, so `Llyn.com` carries the same icon, version and target `Llyn.dll`.
It writes into the intermediate folder, as the SDK does for `Llyn.exe`.
A `None` item copies it from there beside `Llyn.exe` in the build output only.
A publish leaves it out, since a single-file publish has no `Llyn.dll` for it to load.

## `LHostLauncherPublish`

Runs after every publish and publishes `Llyn.Launcher` ahead of time for the same configuration and runtime.
It runs a separate `dotnet publish`.
So this publish's single-file, ReadyToRun and self-contained properties never reach it.
It adds the Visual Studio installer folder to `PATH`, since the native linker's setup calls `vswhere.exe` by bare name.
It runs quiet, so only an error of the nested publish shows.
It reads that output as UTF-8, so a Korean message is not garbled by the console's code page.
It copies the result into the publish folder as `Llyn.com`.
So the build script needs no step for either launcher.

The project keeps only `.pdb` among a reference's related files.
So the WebView2 `.xml` documentation is not published.

## `LHost.cs`

The file uses top-level statements, since a method named `Main` fails the name audit.
The host builds the console start and the window start.
Each start creates one `LRigFactory` with the Windows adapters and builds its first rig and every later rig from it.
The factory is disposed when the start returns, so it releases the client it owns.
The console start hands the UI four seams.
They are the file presence check, the folder and file listings and the logical drives.
So the console's path prompt reads the disk only through the host.
It hands both to Guestroom with the process path and the answer of `LConsoleParent`.
Guestroom picks the one that runs, so the host holds no branch.
The choice reads no console and no operating system, since the launcher alone carries the user's intent.

## Stamp

Commit: `1f3467bbe5c6`
- `LHost.cs` `21dca8e35a4728e2`
- `Llyn.Host.csproj` `2e9c42cd1033e477`
