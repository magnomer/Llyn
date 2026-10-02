# QIconImage.cs
Hash: `e66b7e69f1938b90`

## `public sealed class QIconImage : Image`

Displays an icon in its original colors while enabled and in grayscale while disabled.
The inherited enabled state lets a disabled parent button update its icon automatically.

## `public ImageSource? QIconSource`

Holds the original icon separately from the displayed source so repeated state changes preserve its colors.

## `public QIconImage()`

Listens for effective enabled-state changes inherited from this image or any ancestor.
