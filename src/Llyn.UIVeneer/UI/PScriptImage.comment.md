# PScriptImage.cs

## `internal sealed class PScriptImage : INotifyPropertyChanged, PImagePending`

One glyph picture as the script box draws it: the bitmap once seen, its caption, and its drawn width.
The stored picture is the large original, and the row draws it at a fixed small height.
The bytes are held undecoded until the element drawing them reports itself in view.
Only the size is read at once, from the picture's header.
So the row has its width before it has its pixels.
The decode then goes straight to the drawn height, and the large original is never held as pixels.
The bitmap is decoded once and frozen, so the row template only paints it.

## `private const double PScriptImageMeasure`

The height every picture is drawn at, in device-independent pixels.

## `private const int PScriptImageDecode`

How many pixels are decoded per drawn pixel of height, so a scaled display still reads crisp.

## `public BitmapSource? PScriptImageSource`

The decoded picture, used as an opacity mask so the glyph takes the theme's ink.
It then reads on a dark ground too.
Null until the picture is seen, and the mask then draws nothing.

## `public string PScriptImageCaption`

The caption printed under the picture, or empty so the template collapses it.

## `public double PScriptImageWidth`

The width that keeps the picture's aspect at the drawn height.

## `public double PScriptImageHeight`

The drawn height, handed to the template so the measure lives in one place.

## `internal static PScriptImage? PScriptImageCreate(LScriptImage image)`

Reads the size from the stored bytes, or returns null when they are not a picture the framework can read.

## `public void PImageLoad()`

Decodes the held bytes on the first call and lets them go.
A later call finds nothing held and does nothing.

## `private static Size? PScriptShapeRead(byte[] data)`

The pixel size from the picture's header alone, or null when the bytes are not a picture.
The decoder is asked not to cache and to defer, so no pixel is decoded to answer.

## `private static BitmapSource? PScriptImageRead(byte[] data)`

Decodes the bytes to the drawn height before the stream closes and freezes the result for any thread.
