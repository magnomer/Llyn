# QIcon.cs
Hash: `c0795d7d784c7473`

## `public static class QIcon`

Turns one icon asset into a frozen image an `Image` can display.
SharpVectors preserves the asset's separate fills, strokes, gradients, and opacity instead of flattening every path into one monochrome geometry.
The asset file alone defines each symbol, so C# holds no path or palette data.
The veneer embeds the assets, and the deportment never names where.

## `private static readonly HashSet<string> QIconVector`

The icons that always load from their SVG source, never from a PNG.

## `internal static ImageSource? QIconResolve(string? name, double size, ImageSource? source = null, bool active = true)`

Loads the symbol on first use and keeps the frozen image under its name.
`name` is the asset's file name without its extension.
`size` validates the intended square image size at the call site.
The receiving `Image` supplies the requested width and height while WPF scales the drawing uniformly.
It also builds cached grayscale images when an icon image reports an inherited disabled state.

## `private static ImageSource QIconAssetLoad(string name)`

Reads the asset under the folder the contract resource `PIconRoot` names.
The veneer supplies that resource, so no pack URI of the veneer appears here.
A PNG wins when one exists, and the SVG is the fallback.
