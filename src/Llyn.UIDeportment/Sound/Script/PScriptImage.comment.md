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

## `private const string PScriptImageArea`

The localization area a stored age's code is read under.

## `public BitmapSource? PScriptImageSource`

The decoded picture, used as an opacity mask so the glyph takes the theme's ink.
It then reads on a dark ground too.
Null until the picture is seen, and the mask then draws nothing.

## `public string PScriptImageCaption`

The caption printed under the picture, without its age, or empty so the template collapses it.

## `public string PScriptImageEpoch`

The stored age in the interface language, printed above the caption, or empty so the template collapses it.
The stored code names a localization text, which is read on every request rather than kept.
So the line follows a language change with no fetch and no rebuilt row.
A code no shipped language names reads as empty, because a bare code says less than nothing to a reader.

## `public double PScriptImageWidth`

The width that keeps the picture's aspect at the drawn height.

## `public double PScriptImageHeight`

The drawn height, handed to the template so the measure lives in one place.

## `internal static void PScriptImageApply(FrameworkElement container, object item, string? _)`

Fills a picture of `Theme.Script.Picture`: the lazy loader's row, the shape, the age and the caption.
The shape takes the picture as its mask, so the glyph is drawn in the ink colour.
It runs again when the picture decodes, since the row raises its source then.

## `internal static PScriptImage? PScriptImageCreate(LScriptImage image)`

Reads the size from the stored bytes, or returns null when they are not a picture the framework can read.

## `internal void PScriptImageUpdate()`

Tells the row its age reads differently now, which the box calls for every picture after a language change.
The picture itself is untouched, so only the one line is redrawn.

## `public void PImageLoad()`

Decodes the held bytes on the first call and lets them go.
A later call finds nothing held and does nothing.

## `private static Size? PScriptShapeRead(byte[] data)`

The pixel size from the picture's header alone, or null when the bytes are not a picture.
The decoder is asked not to cache and to defer, so no pixel is decoded to answer.

## `private static BitmapSource? PScriptImageRead(byte[] data)`

Decodes the bytes to the drawn height before the stream closes and freezes the result for any thread.
