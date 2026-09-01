# PDownloaderRecording.cs

## `internal sealed class PDownloaderRecording : INotifyPropertyChanged`

Presentation item for one downloadable recording shown in the downloader menu. Wraps the domain `LRecording` with the source label the row shows, and with that row's own download state, so the menu can report a save on the row that was taken rather than on the one status line the search owns.

## `public string PDownloaderRecordingAction`

The label the row's taking button shows: what it offers, or how its download went.

## `public bool PDownloaderRecordingReady`

Whether the row's taking button still offers a download: false while one runs, and after one has been saved.
