# PClipItem.cs

## `internal sealed class PClipItem : INotifyPropertyChanged`

Presentation item for one downloadable recording shown in `PClip`.
Wraps the domain `LRecording` with the source label the row shows.
It also wraps that row's own download state.
So the menu can report a save on the row that was taken.
It need not report it on the one status line the search owns.

## `public string PClipItemAction`

The label the row's taking button shows: what it offers, or how its download went.

## `public bool PClipItemReady`

Whether the row's taking button still offers a download: false while one runs, and after one has been saved.
