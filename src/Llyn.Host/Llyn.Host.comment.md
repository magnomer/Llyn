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
A `None` item copies it from there beside `Llyn.exe`, in the build output and in the publish folder.
So the build script needs no step, and a clean removes it with the rest.

## `LHost.cs`

The file uses top-level statements, since a method named `Main` fails the name audit.
The host builds the console start and the window start, and hands both to Guestroom with the process path.
Guestroom picks the one that runs, so the host holds no branch.
The choice reads no console and no operating system, since the launcher alone carries the user's intent.
Demeanor never sees the choice, so neither UI family knows the other exists.

The console path runs on the calling thread, with no STA and no WPF object.
`BBootstrap` takes the same rig seams `QBootstrap` takes and returns the exit code itself.
The client is disposed once the bootstrap returns, as on the GUI path.

The atelier is composed in one place for both paths, so the two UIs wire the same ports.
Each path hands in its own marshal, built on its own UI thread.
The factory also takes a detached flag, which the atelier keeps from construction.
The console path takes the flag from `BBootstrapAtelierRun`, which always asks for a detached atelier.
The window path passes false, so the GUI still reads and writes its stored layout.
The console path hands `BBootstrapAtelierRun` the atelier as a seam on that flag, with `File.Exists` as the presence seam.
The presence seam lets the strict export ask whether a file exists.
So the console driver names no file system call.
The engine factory also takes a leased flag.
The console path takes it from the parsed options.
It is true only for the writing options `-new`, `-delete` and `-import`.
The window path always asks for a lease, so a console that writes cannot open the workspace the GUI holds.
Each path builds its own rig on its own UI thread, so the GUI builds its adapters as before.

The GUI exit code stays 1 unless the application runs, so a failed start exits with 1.
WPF needs a single-threaded apartment, and top-level statements cannot carry `[STAThread]`.
So the whole GUI run happens on one STA thread the entry point starts and joins.

The client is built once and handed to every rig of the session.
It is disposed in one place at the end, whether or not the start went through.
The file usher answers path facts and the shell usher adds the launch of a folder or a link.
The rig carries the press, so the engine prints through it and the host applies none later.
The Windows spooler lists the printers, so no layer above sees System.Printing.
The Windows warrant protects the Joplin token, so the engine never sees the operating system.
The host builds one phonograph for the whole run, so a workspace change keeps the same player.
For each Windows twin the host is the one place that names it.

The veneer's application is only created and run by the host.
Creating it makes it `Application.Current`, which is how the deportment reaches it.
`QBootstrap` applies resources, raises every dialog of the start, and shows the window.
It pulls the application from `Application.Current`, so the host hands it no application.

The host hands over the theme, the engine factory and the rest of the run as seams.
`QBootstrap` decides whether the start continues, so the host holds no branch.
A theme or an engine that fails leaves the rest unrun, and the exit code stays 1.

The deportment may not name Infrastructure, so the host hands the doctor's busy check over as a seam.
The engine receives every adapter through the rig and references Infrastructure nowhere.
The engine reads the settings the rig holds and imports the languages when it opens.
The engine keeps a rig factory for a workspace change.
The engine takes the new rig whole and refuses it whole.
A folder that fails to open leaves the old rig standing.
The chosen folder is recorded only after the new rig stands, so a failing folder is never pointed at.

The default catalog is applied before any engine exists, so the host opens it through the port itself.
That load lists the embedded languages first, so a stored language the build lacks falls back to the default.

The rest of the run is called only with an engine that was built.
It wires the engine into the window, runs it, and disposes the engine at its end.
The fault record is the first step once the engine exists, so a fault in any later step is recorded.
The settings outlet is built before the window, because the catalog step reads the stored language through it.
The atelier then takes this same outlet.
The catalog step runs before the rescue consult, so the rescue notice speaks the stored language.

The host builds Conduct's root and hands it to the deportment's window or to `BBootstrap`.
So neither bootstrap builds it.
The root takes the posture and one outlet per port, all over the one engine.
The entry ports and the sound ports are the engine's own facades, gathered in two bundles with no outlet between.
The deportment receives the root and never sees the engine itself.
The root also takes the marshal, built here on the UI thread, which every editor takes at construction.
The window disposes its own deportment and posture when it closes.
The host then disposes the engine the window ran over.

## Stamp

Commit: `52c21d68b4c4`
- `LHost.cs` `4f583129b1c6aa82`
- `Llyn.Host.csproj` `227db85934ecef45`
