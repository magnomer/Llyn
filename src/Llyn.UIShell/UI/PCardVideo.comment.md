# PCardVideo.cs

## `internal sealed partial class PCard`

The Video rows a card carries while it is being written. They are not part of what the card says it is: a Video is never read from a draft and never written into one, so it is neither loaded with the card nor stored with it, and it lives only as long as the form. A card that is saved and opened again shows no videos.

## `internal void PCardVideoAdd()`

Opens an empty video row at the end, which is what the card's Extra row asks for.

## `internal void PCardVideoRemove(PVideo row)`

Drops the row outright.
