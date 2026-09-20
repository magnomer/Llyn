# PTranscriptionChoice.cs

## `internal sealed class PTranscriptionChoice : INotifyPropertyChanged`

One scheme a transcription row's dropdown offers.
It carries the scheme name, the label the scheme is shown under, and whether another row already holds it.
A row keeps one of these per declared scheme, built once, so the dropdown never rebinds while open.

## `public bool PTranscriptionChoiceTaken`

True while another row of the same draft carries this scheme.
The dropdown disables such a row, because the engine refuses two rows in one scheme.
It announces only a real change, so a render marking the same state stays silent.
