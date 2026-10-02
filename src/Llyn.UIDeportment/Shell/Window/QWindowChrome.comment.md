# QWindowChrome.cs
Hash: `669d8c8794036f73`

## `public partial class QWindow`

The window frame the program draws for itself, since the system one is off.
That is the caption buttons and dragging the window by its roof.
Minimize and close go to the system commands.
Maximize and the roof drag go to `QCaption`, which decides what the press does.
The loaded window is what each press acts on.
