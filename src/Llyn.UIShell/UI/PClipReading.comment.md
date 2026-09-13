# PClipReading.cs

## `internal sealed class PClipReading : INotifyPropertyChanged`

One recording a source returned, as one previewable and takeable entry on its source's row.
It is built once from the recording, but its taking button reports its own download.
So it notifies, where `PNotationReading` need not.

## `public string PClipReadingVariety`

The raw variety name the source tagged the recording with, empty when the source did not say.
It is what taking the recording sends to the row, never the label the user sees.

## `public string PClipReadingLabel`

The name shown for the variety, localized when a `Variety.*` key exists and raw otherwise.
In flag mode it is the tooltip of the flag rather than visible text.

## `public ImageSource? PClipReadingFlag`

The variety's flag, resolved when the language pack shows varieties as flags and the pack declares one.
Null means the label stands in for it.

## `public string PClipReadingAction`

The label the taking button shows: what it offers, or how its download went.

## `public bool PClipReadingReady`

Whether the user can take this recording now.
False while a download runs, and false once one has been saved.

## `internal LRecording PClipReadingModel`

The recording this entry stands for, which is what preview fetches and taking saves.
