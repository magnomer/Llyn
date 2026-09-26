# PIconImage.cs

## `public sealed class PIconImage : Image`

Displays an icon in its original colors while enabled and in grayscale while disabled.
The inherited enabled state lets a disabled parent button update its icon automatically.

## `public ImageSource? PIconSource`

Holds the original icon separately from the displayed source so repeated state changes preserve its colors.

## `public PIconImage()`

Listens for effective enabled-state changes inherited from this image or any ancestor.
