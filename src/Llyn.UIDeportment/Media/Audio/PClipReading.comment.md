# PClipReading.cs

## `internal sealed class PClipReading`

One recording a source returned, as one previewable and takeable entry on its source's row.
It paints a ready `CClipReading` and raises no change.
The menu builds its entries afresh from each clip state the errand raises.
So a preview or a download keeps its look when another search step lands.

## `internal PClipReading(CClipReading reading, string label, ImageSource? flag, string action)`

Takes the text and the flag the row already looked up, and copies the ready states as they are.

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

## `public bool PClipReadingFetching`

Whether the preview of this recording is still being fetched.
The play button fills orange while it is, so a slow source is seen to be working.

## `public bool PClipReadingPlaying`

Whether the preview of this recording is the one now sounding.
The play button fills blue while it is, and clears when the sound ends or another preview starts.

## `public bool PClipReadingRefused`

Whether the last preview fetch of this recording failed.
The play button fills with the warning colour while it is.
So a host that refused is told apart from one that answered nothing.

## `internal CRecording PClipReadingModel`

The recording this entry stands for, which the preview and the taking hand to their gates.
