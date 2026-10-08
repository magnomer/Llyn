# QCaption.cs
Hash: `a94cfc236ad84fe3`

## `public sealed class QCaption`

The window frame the program draws for itself, since the system one is off.
That is the caption buttons and dragging the window by its roof.
Minimize and close go to the system commands.
The loaded window is what each press acts on.

## `public QCaption(Window window)`

Keeps the loaded window whose caption and roof it drives.

## `public void QCaptionIntroduce()`

Each caption glyph strokes with its button's foreground through a binding set here.
The roof and the caption buttons are subscribed here after the load.

## `private void QCaptionMaximizeRefine(object sender, RoutedEventArgs e)`

Toggles between maximized and restored rather than only maximizing.
The caption's maximize button and a double press on the roof share this one switch.

## `private void QRoofRefine(object sender, MouseButtonEventArgs e)`

A double press on the roof toggles maximize instead of dragging.
A maximized window is restored under the pointer before the drag starts.

## `private void QCaptionPointerRefine(MouseButtonEventArgs e)`

The pointer keeps its share of the restored width.
It stays at most half the roof's height below the top edge.
Device pixels become layout units first, so a scaled display still lands the window under the pointer.
