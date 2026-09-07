# PNotationItem.cs

## `internal sealed class PNotationItem : INotifyPropertyChanged`

One row of the pronunciation menu, standing for one source.
The row is created the moment its source starts searching, so every declared source is visible from the first frame.
It carries the source's declared position, which is where the menu keeps it whatever order the network answers in.
It is resolved in place rather than replaced, so a row never jumps as the search fills in.

## `public string PNotationItemReading`

The transcription this source gave, empty while none has arrived.

## `public string PNotationItemNotice`

The one line a row without a transcription says: searching, no entry, or failed to retrieve.

## `public bool PNotationItemReady`

Whether the row carries a transcription the user can take now, which is what `PClipItemReady` means on an audio row.
It is what the template switches the reading and the taking button on.

## `internal void PNotationItemShow(LCandidate candidate, string missing, string broken)`

Resolves the row from what its source finally said.
A transcription makes the row takeable.
No transcription reads as `missing` when the source answered and `broken` when it never did.
