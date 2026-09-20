# App.xaml.cs

## `private readonly LUsher _lBootstrapUsher = new LUsherShell(new LUsherFile());`

The usher the rig is built over, composed here because the media ring is the root's to name.
The file usher answers path facts and the shell usher adds the launch of a folder or a link.

## `protected override void OnExit(ExitEventArgs e)`

Disposes the engine the window ran over, then the one client every rig of the session was built over.
The window disposes its own deportment and posture when it closes, so nothing waits on the exit for them.

## `private void LBootstrapWorkspaceChange(string path)`

The composition root's half of a workspace change, handed to the window as a delegate.
The rig for the new folder is built here, since only this file may name the factory.
The engine takes it whole and refuses it whole.
A folder that fails to open leaves the old rig standing.
The pointer is written last, so a folder that fails to open is never pointed at.
The pointer resolves the path to its full form itself, so it reads as the rig's root.
The rig passes straight from the factory into the engine.
The shell may hold no engine answer of its own.

## `private void LBootstrapLocalizationApply(LEngine engine)`

Applies the language the workspace stores before any window or notice is shown.
Every panel therefore attaches in that language, and the rescue notice speaks it too.
A stored language the interface does not carry leaves English, which is already applied, in place.
A catalog that fails to load is reported and English stays, since the program can still run in it.

## `private void LBootstrapFaultHandle(object sender, DispatcherUnhandledExceptionEventArgs e)`

Catches every exception that reaches the dispatcher unhandled.
That is where an `async void` handler's fault lands.
The fault is written to the audit log, shown to the user, and marked handled so the program stays up.
Without this any such fault ends the process with the user's unsaved drafts still open.
The dialog names the log, so the user can hand over what happened.
When no window is left showing, the program ends after the dialog instead of lingering unseen.
A fault while the main window is first shown leaves it unopened, and an unopened window never closes.
Such a process held the installed executable and blocked the next build.

## `private bool LBootstrapWindowCheck()`

Whether any window of the program is showing, so a fault with none is the end of the run.

## `private void LBootstrapStrayHandle(object? sender, UnobservedTaskExceptionEventArgs e)`

Records a task fault nobody awaited and marks it observed.
Such a fault reaches no dispatcher, so this is the only place it is seen at all.

## `private void LBootstrapRescueShow(LDoctorRescue rescue)`

Tells the user their database was set aside and a clean one started, and stays silent when it was not.
The program launches either way.
Without this the workspace would appear empty and the old data would look lost.
The message names the file that was kept, so the user can see nothing was thrown away.

## Inline notes

### `LLocalization.LLocalizationLoad(new LLocalizationLoader(), LLocalization.LLocalizationDefault));`

The default catalog is applied before any engine exists, so the bootstrap opens it through the port itself.
That load lists the embedded languages first, since it refuses a language the build does not embed.

### `MessageBox.Show(`

The catalog is what would have translated this message, so this one stays inline.
It is the documented emergency fallback, shown only when the interface text is unavailable.

### `string workspace = string.Empty;`

The engine opens the workspace: settings, database file, and schema.
So it is built here rather than in a field initializer of the window.
An unwritable folder must reach the user as a message naming the folder, not as a crash inside a constructor.
A corrupt database or a file from a newer build no longer stops the launch.
`LDoctor` sets it aside inside the engine and `LBootstrapRescueShow` reports it afterwards.
A database another program holds is not set aside, and the message says so.
Closing that program is the cure.

### `engine = new LEngine(LRigFactory.LRigFactoryBuild(workspace, _lBootstrapClient));`

This file is the composition root: the one place above Infrastructure that builds a rig.
The engine receives every adapter through the rig and references Infrastructure nowhere.
The client is the root's field, built once and handed to every rig, and disposed in `OnExit`.

### `new PWindow(engine, LBootstrapWorkspaceChange).Show();`

The window owns the engine from here: it disposes it when it closes.
