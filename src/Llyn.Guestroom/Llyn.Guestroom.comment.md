# Llyn.Guestroom

The launcher's choice of the UI a start opens.
It sits beside Host, so Host only builds and wires and holds no branch.
It names no Llyn project, so it compiles against nothing beyond .NET.
It builds for every platform, since the choice reads only the path and the flag Host hands it.

## `LGuestroom`

`LGuestroomRun` runs the console start or the window start and returns its exit code.
A marked start means the native `Llyn.com` started `Llyn.exe` and marked it, so the console start runs.
A path ending in `.com` means a plain build's apphost `Llyn.com` started it.
The console start runs then too.
The match ignores case, since Windows file names ignore case.
Any other path runs the window start.
A missing path runs the window start too, so a renamed launcher keeps the GUI.

Today WPF is the only GUI family, so the window start opens WPF at once.
There is nothing to choose, so no selector shows and no saved choice is read.
A later GUI family adds the selector and the saved choice here.
A console start never reaches either of them.

## Stamp

Commit: `ed7d18d90760`
- `LGuestroom.cs` `c7e21533102a3f4c`
- `Llyn.Guestroom.csproj` `c26a2a97a04873fe`
