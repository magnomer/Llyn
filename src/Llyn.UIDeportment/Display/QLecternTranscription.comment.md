# QLecternTranscription.cs
Hash: `1270a1871234006d`

## `public sealed class QLecternTranscription`

The reading view's transcription section, drawing the transcription rows [CDisplaySound](../../Llyn.Conduct/Display/CDisplaySound.comment.md) answers.
[QLectern](QLectern.comment.md) builds it once in its constructor over the view's page.
It pulls its own list by contract ID, and the lectern subscribes its redraw to the display's open and close.

## `public QLecternTranscription(FrameworkElement surface, CDisplaySound area)`

Binds the transcription list pulled from `surface` to its rows.
`area` is the display's sound area, the only part it reads.
The list is attached to `QTranscriptionItem.QTranscriptionItemRefine`, which fills each row.

## `public void QLecternTranscriptionRefine()`

Rebuilds the transcription rows from the area's answer.
The lectern subscribes it to both open and close, since a closed display answers no rows.
