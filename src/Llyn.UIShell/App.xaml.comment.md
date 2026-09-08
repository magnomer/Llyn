# App.xaml.cs

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

### `new PWindow(engine).Show();`

The window owns the engine from here: it disposes it when it closes.
