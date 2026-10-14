# Llyn.Host

The composition root that builds the engine and hands it to a UI.
It builds the executable `Llyn.exe`, so the installed program keeps its name.
It stands apart because it is the only project that may name every project and every Windows twin.

## `Llyn.Host.csproj`

It holds no behaviour, and each UI's entry point adds only what depends on its medium.
It takes the veneer's framework, runtime identifiers and icon, since it is now the executable.
It uses WPF because it starts the veneer's application on its own thread.
The Release publish is precompiled with ReadyToRun.
Precompiled code removes first-use JIT from launch and first clicks.

## `LHost.cs`

The file uses top-level statements, since a method named `Main` fails the name audit.
The exit code stays 1 unless the application runs, so a failed start exits with 1.
WPF needs a single-threaded apartment, and top-level statements cannot carry `[STAThread]`.
So the whole run happens on one STA thread the entry point starts and joins.

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

The host builds Conduct's root and hands it to the deportment's window, so `QBootstrap` builds nothing.
The root takes the posture and one outlet per port, all over the one engine.
The entry ports and the sound ports are the engine's own facades, gathered in two bundles with no outlet between.
The deportment receives the root and never sees the engine itself.
The root also takes the marshal, built here on the UI thread, which every editor takes at construction.
The window disposes its own deportment and posture when it closes.
The host then disposes the engine the window ran over.

## Stamp

Commit: `c6e60141a172`
- `LHost.cs` `9b8f34eb7a69c9e5`
- `Llyn.Host.csproj` `e789063a5fe25eef`
