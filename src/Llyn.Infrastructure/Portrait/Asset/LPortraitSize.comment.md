# LPortraitSize.cs

## `public static class LPortraitSize`

Reads a picture's pixel size from its own header.
A Word picture must be sized when it is placed, and no decoder is available here.

## `public static (int Width, int Height) LPortraitSizeRead(byte[] data)`

Four container shapes are recognized from their opening bytes.
Anything else falls back to a plain widescreen size rather than failing the export.

## `private static (int Width, int Height) LPortraitSizeScan(byte[] data)`

A JPEG keeps its size in a frame marker somewhere after the start.
Markers are walked until a frame marker is met, and the scan stops at a malformed length.

## `private static int LPortraitSizeRead(byte[] data, int place, bool big)`

Reads four bytes in whichever order the container uses.
