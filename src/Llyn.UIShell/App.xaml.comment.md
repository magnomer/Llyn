# App.xaml.cs

## Inline notes

### `MessageBox.Show(`

The catalog is what would have translated this message, so this one stays inline.
It is the documented emergency fallback, shown only when the interface text is unavailable.

### `string workspace = string.Empty;`

The engine opens the workspace: settings, database file, and schema.
So it is built here rather than in a field initializer of the window.
An unwritable folder, a corrupt database, or a file from a newer build must reach the user.
It must reach them as a message naming the folder, not as a crash inside a constructor.

### `new PWindow(engine).Show();`

The window owns the engine from here: it disposes it when it closes.
