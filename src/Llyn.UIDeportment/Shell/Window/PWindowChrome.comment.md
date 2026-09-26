# PWindowChrome.cs

## `public partial class PWindow`

The window frame the program draws for itself, since the system one is off.
That is the caption buttons and dragging the window by its roof.
Each handler hands its press to `LCaption`, which decides what the press does.
The loaded window is what each press acts on.
