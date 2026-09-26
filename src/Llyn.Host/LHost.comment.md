# LHost.cs

## `int code = 1;`

The file uses top-level statements, since a method named `Main` fails the name audit.
The exit code stays 1 unless the application runs, so a failed start exits with 1.

## `Thread thread = new(() =>`

WPF needs a single-threaded apartment, and top-level statements cannot carry `[STAThread]`.
So the whole run happens on one STA thread the entry point starts and joins.

## `HttpClient client = LRigFactory.LRigClientCreate();`

This file is the composition root: the one place that builds a rig and names every project.
The client is built once and handed to every rig of the session, and disposed after the run.

## `LUsher usher = new LUsherShell(new LUsherFile());`

The file usher answers path facts and the shell usher adds the launch of a folder or a link.

## `LPress press = new LPressBrowser();`

The engine prints through the rig, so no press is applied after construction.

## `PBootstrap application = new();`

The veneer's application, which the host only creates and runs.
Creating it makes it `Application.Current`, which is how the deportment reaches it.

## `QBootstrap bootstrap = new();`

`QBootstrap` applies resources, raises every dialog of the start, and shows the window.
It pulls the application from `Application.Current`, so the host hands it nothing.

## `() => LLocalization.LLocalizationLoad(new LLocalizationLoader(), LLocalization.LLocalizationDefault));`

The default catalog is applied before any engine exists, so the host opens it through the port itself.
That load lists the embedded languages first, since it refuses a language the build does not embed.

## `workspace => new LEngine(LRigFactory.LRigFactoryBuild(workspace, client, usher, press, phonograph)),`

The engine opens the workspace: settings, database file, and schema.
The engine receives every adapter through the rig and references Infrastructure nowhere.

## `bootstrap.QBootstrapWindowShow(new PWindow(`

The host creates the deportment's window too, so `QBootstrap` builds nothing.

## `new LWindow(`

Composes the window from the posture and one outlet per port, all over the one engine.
The deportment receives the outlets and never sees the engine itself.

## `path =>`

The host's half of a workspace change, handed to the window as a delegate.
The engine takes the new rig whole and refuses it whole.
A folder that fails to open leaves the old rig standing.
The pointer is written last, so a folder that fails to open is never pointed at.

## `code = application.Run();`

The window disposes its own deportment and posture when it closes.
The host then disposes the engine the window ran over, and then the client.
