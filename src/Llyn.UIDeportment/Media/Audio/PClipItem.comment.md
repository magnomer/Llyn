# PClipItem.cs

## `internal sealed class PClipItem : INotifyPropertyChanged`

One row of the audio menu, standing for one source.
The row is created the moment its source starts searching, exactly as `PNotationItem` is.
It carries the source's declared position, which is what the menu keeps the row at.
It is resolved in place rather than replaced, so a row never jumps as the search fills in.
A source may answer several times, once per variety, and each answer becomes one more recording on the row.
Download state lives on each recording, so the row itself only says whether it has any.

## `public ObservableCollection<PClipReading> PClipItemReading`

The recordings this source gave so far, in the order they arrived, empty while none has.

## `public string PClipItemNotice`

The one line a row without a recording says: searching, no entry, or failed to retrieve.

## `public bool PClipItemReady`

Whether the row carries a recording the user can take now, which is what `PNotationItemReady` means on a pronunciation row.
It is what the template switches the recordings and the notice on.

## `internal void PClipItemShow(LRecording recording, PClipReading? reading, string missing, string broken)`

Resolves the row from one thing its source said.
A recording is appended and makes the row takeable.
The editor builds the recording's entry, because its label and flag come from the window and the language pack.
No address reads as `missing` when the source answered and `broken` when it never did.
An empty answer after a recording has landed changes nothing, so a found row never falls back to a notice.
