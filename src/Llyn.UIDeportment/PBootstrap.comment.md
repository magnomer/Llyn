# PBootstrap.cs

## `public sealed class PBootstrap : System.Windows.Application`

The deportment's application: it applies resources and shows the window.
The host builds the engine and `LBootstrap` raises every dialog of the start.
It is a plain subclass with no markup, so no assembly carries a generated entry point.

## `public PBootstrap()`

Merges the veneer's `App.xaml` by pack URI, before any resource is applied over it.
The deportment cannot reference the veneer, so the URI is the only link between them.
The deportment holds no markup of its own, so nothing else is merged.

## `public void PBootstrapThemeApply(Func<string, string> colorRead, IReadOnlyDictionary<string, string> catalog)`

Applies the default catalog, the theme colors, and the shared control resources in one step.
The host wraps it in `LBootstrapThemeApply`, so a failure here reaches the user as a dialog.

## `public void PBootstrapCatalogApply(IReadOnlyDictionary<string, string> catalog)`

Applies the catalog of the language the workspace stores over the default one.

## `public string PBootstrapTextRead(string key)`

Reads one interface text, handed to `LBootstrap` as the seam its dialogs speak through.

## `public void PBootstrapWindowShow(PWindow window)`

Shows the main window the host built over the deportment, never over the engine itself.
What is shown is the loaded window the class holds.
The host creates the window, so the bootstrap holds nothing but calls.
