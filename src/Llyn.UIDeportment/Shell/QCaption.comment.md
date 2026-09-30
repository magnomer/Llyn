# QCaption.cs

## `public static class QCaption`

The decisions behind the caption the program draws for itself.
The veneer's caption and roof handlers hand every press here.

## `public static void QCaptionDragRefine(Window window, FrameworkElement roof, MouseButtonEventArgs e)`

A double press on the roof toggles maximize instead of dragging.
A maximized window is restored under the pointer before the drag starts.

## `private static void QCaptionPointerRefine(Window window, FrameworkElement roof, MouseButtonEventArgs e)`

The pointer keeps its share of the restored width.
It stays at most half the roof's height below the top edge.
Device pixels become layout units first, so a scaled display still lands the window under the pointer.
