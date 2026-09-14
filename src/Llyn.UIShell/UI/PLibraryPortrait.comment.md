# PLibraryPortrait.cs

## `public partial class PLibrary`

The library panel's export action.
The panel picks a file and a format, and hands both to the engine.
It renders nothing, opens no writer, and knows no file format.

## `private static readonly IReadOnlyList<(string Key, string Suffix, LPortraitFormat Format)> PLibraryPortraitKinds`

One row per offered format, keeping the dialog filter and the chosen format in step.
The dialog returns a one-based index into this same list, so the two cannot drift apart.

## `internal async void PLibraryPortraitHandle(object sender, RoutedEventArgs e)`

Export acts on the entry being read, which is the one the panel already shows.
A cancelled dialog is not a failure and leaves nothing behind.
The filter index chooses the format, so the reader picks it where they pick the name.
The whole attempt stands together, because a half-written document is of no use.
The words the export needs come from the window, which reads them for every panel alike.

## `private string PLibraryNameRead(long id)`

The headword is offered as the file name, since that is what the reader would type.
Characters a file name cannot hold are replaced rather than dropped, so nothing silently merges.
An unknown entry still offers a name, because the dialog must open either way.
