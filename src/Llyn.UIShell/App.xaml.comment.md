# App.xaml.cs

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

## `private void LBootstrapStrayHandle(object? sender, UnobservedTaskExceptionEventArgs e)`

Records a task fault nobody awaited and marks it observed.
Such a fault reaches no dispatcher, so this is the only place it is seen at all.

## `private void LBootstrapRescueShow(LDoctorRescue rescue)`

Tells the user their database was set aside and a clean one started, and stays silent when it was not.
The program launches either way.
Without this the workspace would appear empty and the old data would look lost.
The message names the file that was kept, so the user can see nothing was thrown away.

## Inline notes

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

### `new PWindow(engine).Show();`

The window owns the engine from here: it disposes it when it closes.
