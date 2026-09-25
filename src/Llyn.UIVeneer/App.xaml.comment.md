# App.xaml.cs

## `public partial class PBootstrap : System.Windows.Application`

The veneer's half of the start: it applies resources and shows the window, and only makes calls.
The host builds the engine and the deportment's `LBootstrap` raises every dialog of the start.
`App.xaml` builds as a page, so this assembly carries no entry point of its own.

## `public void PBootstrapThemeApply(Func<string, string> colorRead, IReadOnlyDictionary<string, string> catalog)`

Applies the default catalog, the theme colors, and the shared control resources in one step.
The host wraps it in `LBootstrapThemeApply`, so a failure here reaches the user as a dialog.

## `public void PBootstrapCatalogApply(IReadOnlyDictionary<string, string> catalog)`

Applies the catalog of the language the workspace stores over the default one.

## `public string PBootstrapTextRead(string key)`

Reads one interface text, handed to `LBootstrap` as the seam its dialogs speak through.

## `public void PBootstrapWindowShow(PWindow window)`

Shows the main window the host built over the deportment, never over the engine itself.
The host creates the window, since a creation in the veneer is more than a call.
