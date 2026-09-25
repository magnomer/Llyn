# PIcon.cs

## `public sealed class PIcon : MarkupExtension`

Turns one SVG asset under `assets/icons` into a frozen drawing an `Image` can display.
SharpVectors preserves the asset's separate fills, strokes, gradients, and opacity instead of flattening every path into one monochrome geometry.
The SVG file alone defines each symbol, so XAML and C# hold no path or palette data.

## `private static readonly HashSet<string> PIconVector`

The icons that always load from their SVG source, never from a PNG.

## `public PIcon(string name, double size)`

`name` is the asset's file name without `.svg`, and `size` validates the intended square image size at the call site.

## `public override object ProvideValue(IServiceProvider serviceProvider)`

Answers the cached drawing image, so XAML pays the SVG conversion once per asset.

## `internal static ImageSource? PIconResolve(string? name, double size, ImageSource? source = null, bool active = true)`

Loads the symbol on first use and keeps the frozen drawing under its name.
The receiving `Image` supplies the requested width and height while WPF scales the drawing uniformly.
It also builds cached grayscale drawings when an icon image reports an inherited disabled state.
