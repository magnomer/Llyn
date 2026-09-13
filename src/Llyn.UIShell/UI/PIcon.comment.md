# PIcon.cs

## `public sealed class PIcon : MarkupExtension`

Turns one SVG asset under `assets/icons` into a frozen geometry a `Path` fills with its own brush.
The asset file is the only place a symbol's shape lives, so no XAML or C# holds path data.
Every symbol is scaled by the frame its `viewBox` declares, never by the bounds of its ink.
Two symbols asked for at the same size therefore share one stroke width and one optical box.
Material Symbols draw on a 24-unit grid, so frames of 12 and 24 land every edge on a device pixel.
A `Path` showing the result sets `Stretch="None"` and the same width and height as the frame.

## `public PIcon(string name, double size)`

`name` is the asset's file name without `.svg`, and `size` is the frame's edge in device-independent pixels.

## `public override object ProvideValue(IServiceProvider serviceProvider)`

Answers the cached geometry for the name and size, so XAML pays the file read once per pair.

## `internal static Geometry PIconResolve(string name, double size)`

Loads the symbol on first use and keeps the scaled, frozen copy under the name and size.
The transform maps the frame's origin to zero and its longer edge to `size`.

## `internal static (Geometry, Rect) PIconLoad(string name)`

Reads the asset as XML and joins every `path` into one nonzero-filled group.
The `F1` prefix keeps the nonzero rule the SVG default gives, so rings and cutouts wind as drawn.
The frame is the `viewBox`, or the width and height when a file declares none.
Callers that fit by ink rather than by frame, like the grasp stars, take the raw symbol from here.
