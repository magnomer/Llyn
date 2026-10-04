# QScriptImage.cs
Hash: `5f0922efc7df87bf`

## `internal sealed class QScriptImage : INotifyPropertyChanged, QImagePending`

One glyph picture as the script box draws it.
It holds the bitmap once seen, its caption, and its drawn width.
The stored picture is the large original, and the row draws it at a fixed small height.
The bytes are held undecoded until the element drawing them reports itself in view.
Only the size is read at once, from the picture's header.
So the row has its width before it has its pixels.
The decode then goes straight to the drawn height, and the large original is never held as pixels.
The bitmap is decoded once and frozen, so the row template only paints it.

## `private const double QScriptImageMeasure`

The height every picture is drawn at, in device-independent pixels.

## `private const int QScriptImageDecode`

How many pixels are decoded per drawn pixel of height, so a scaled display still reads crisp.

## `public BitmapSource? QScriptImageSource`

The decoded picture, used as an opacity mask so the glyph takes the theme's ink.
It then reads on a dark ground too.
Null until the picture is seen, and the mask then draws nothing.

## `public string QScriptImageCaption`

The caption printed under the picture, without its age, or empty so the template collapses it.

## `public string QScriptImageEpoch`

The stored age in the interface language, printed above the caption, or empty so the template collapses it.
Conduct hands the age as a wording key, or empty when it has no wording.
The key is read on every request rather than kept.
So the line follows a language change with no fetch and no rebuilt row.

## `public double QScriptImageWidth`

The width that keeps the picture's aspect at the drawn height.

## `public double QScriptImageHeight`

The drawn height, handed to the template so the measure lives in one place.

## `internal static void QScriptImageRefine(FrameworkElement container, object item, string? _)`

Fills a picture of `Theme.Script.Picture`, which holds the lazy loader's row, the shape, the age and the caption.
The shape takes the picture as its mask, so the glyph is drawn in the ink colour.
It runs again when the picture decodes, since the row raises its source then.

## `private readonly Action<Exception> _qScriptImageFailure`

The scan's failure report, which the deferred decode reaches long after the scan returned.

## `internal static QScriptImage? QScriptImageCreate(CScriptImage image, Action<Exception> failure)`

Reads the size from the stored bytes, or returns null when they are not a picture the framework can read.
An unreadable picture is reported to `failure`, which the scan shows once.

## `internal void QScriptImageRaise()`

Tells the row its age reads differently now, which the box calls for every picture after a language change.
The picture itself is untouched, so only the one line is redrawn.

## `public void QImagePendingLoad()`

Decodes the held bytes on the first call and lets them go.
A later call finds nothing held and does nothing.

## `private static Size? QScriptShapeRead(byte[] data, Action<Exception> failure)`

The pixel size from the picture's header alone, or null when the bytes are not a picture.
Only the WPF decoder knows the bytes do not decode, so the failure is reported here.
The decoder is asked not to cache and to defer, so no pixel is decoded to answer.

## `private static BitmapSource? QScriptImageRead(byte[] data, Action<Exception> failure)`

Decodes the bytes to the drawn height before the stream closes and freezes the result for any thread.
A failed decode leaves the picture blank and reports to `failure`.
