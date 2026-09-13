# PTranscriptionItem.cs

## `internal sealed class PTranscriptionItem : INotifyPropertyChanged`

One transcription row, as the editor and the reading view show it.
It carries the draft row's id, its scheme, the label the scheme is shown under, and its text.
The field is bound to the text and the dropdown to the scheme.
It also carries the schemes its dropdown offers, one choice per scheme the pack declares.

## `public string PTranscriptionItemScheme`

The scheme the row spells the reading in, as picked or as the draft holds it.
An empty pick is ignored, because a dropdown rebinding hands one over on its way.
It announces only a real change, and announces the label with it.

## `public string PTranscriptionItemLabel`

The scheme's localized name, shown as the chip before the field the way a variety name is.

## `internal void PTranscriptionItemUpdate(IReadOnlyList<string> schemes, IReadOnlyList<PTranscriptionItem> rows)`

Gives the dropdown its choices and marks the ones other rows hold as taken.
The choices are built once and kept, so an open dropdown is not rebound under the pointer.
They are rebuilt only when the pack's scheme count differs, which a language switch brings.

## `public string PTranscriptionItemText`

The transcription as typed or as the draft holds it.
It announces only a real change, so a render writing the same text stays silent.

## `internal static PTranscriptionItem PTranscriptionItemCreate(PWindow host, LTranscriptionDraft spelled)`

Builds the row for one draft transcription.

## `internal static string PTranscriptionLabelFormat(PWindow host, string scheme)`

A scheme's label is its localized `Scheme.*` text when one exists and its raw name otherwise.
An unnamed scheme has no label.
