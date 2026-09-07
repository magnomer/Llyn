# PClipItem.cs

## `internal sealed class PClipItem : INotifyPropertyChanged`

One row of the audio menu, standing for one source.
The row is created the moment its source starts searching, exactly as `PNotationItem` is.
It carries the recording's declared position, which is what the menu keeps the row at.
It also wraps that row's own download state.
So the menu can report a save on the row that was taken.
It need not report it on the one status line the search owns.

## `public string PClipItemAction`

The label the row's taking button shows: what it offers, or how its download went.

## `public string PClipItemNotice`

The one line a row without a recording says: searching, no entry, or failed to retrieve.

## `public bool PClipItemFound`

Whether the source offered a recording at all.
It is what the template switches the preview and taking buttons on.
A pronunciation row needs no such flag, because there a reading is the whole of what can be taken.

## `public bool PClipItemReady`

Whether the row carries a recording the user can take now, which is what `PNotationItemReady` means on a pronunciation row.
Here that is the narrower state: false while a download runs, and false once one has been saved.

## `internal LRecording PClipItemModel`

The recording the row stands for.
Reading it from a row that has none is a caller's mistake, not a state to render, so it throws.

## `internal void PClipItemShow(LRecording recording, string missing, string broken)`

Resolves the row from what its source finally said.
An address makes the row previewable and takeable.
No address reads as `missing` when the source answered and `broken` when it never did.
