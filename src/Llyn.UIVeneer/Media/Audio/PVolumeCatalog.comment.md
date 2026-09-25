# PVolumeCatalog.cs

## `public sealed class PVolumeCatalog`

The one loudness the program plays at, held where anything that plays can reach it.

The slider that sets it stands in the playback tray, which belongs to the pronunciation.
A video is played from a card and knows nothing of that tray.
The two cannot be wired to each other directly.
They are wired to this instead, which is a value and not a control.

The level is held rather than stored here.
The tray is what the workspace settings answer to, so writing the value back stays where the slider is.

## `public void PVolumeCatalogSet(double level)`

Sets the level as a method, so a deportment can take it as a seam.

## Inline notes

### `public double PVolumeCatalogLevel { get; set; }`

Held between silence and full, because that is the range every player here takes.
A value outside it is pulled to the nearest end rather than refused.
A slider or the seam a lectern holds is the only thing that sets it.
