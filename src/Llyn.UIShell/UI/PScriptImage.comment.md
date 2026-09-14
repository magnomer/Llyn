# PScriptImage.cs

## `public sealed class PScriptImage`

One glyph picture as the script box draws it: the decoded bitmap, its caption, and its drawn width.
The stored picture is the large original, and the row draws it at a fixed small height.
The bitmap is decoded once from the stored bytes and frozen, so the row template only paints it.

## `private const double PScriptImageMeasure`

The height every picture is drawn at, in device-independent pixels.

## `public BitmapSource PScriptImageSource`

The decoded picture, used as an opacity mask so the glyph takes the theme's ink.
It then reads on a dark ground too.

## `public string PScriptImageCaption`

The caption printed under the picture, or empty so the template collapses it.

## `public double PScriptImageWidth`

The width that keeps the picture's aspect at the drawn height.

## `public double PScriptImageHeight`

The drawn height, handed to the template so the measure lives in one place.

## `internal static PScriptImage? PScriptImageCreate(LScriptImage image)`

Decodes the stored bytes, or returns null when they are not a picture the framework can read.

## `private static BitmapSource? PScriptImageRead(byte[] data)`

Loads the bytes fully before the stream closes and freezes the result for any thread.
