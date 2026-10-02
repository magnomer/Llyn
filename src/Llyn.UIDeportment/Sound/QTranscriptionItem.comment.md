# QTranscriptionItem.cs
Hash: `2df6c3ee6ca2c1c9`

## `public sealed class QTranscriptionItem : INotifyPropertyChanged`

One transcription row, as the editor and the reading view show it.
It carries the draft row's id, its scheme, the label the scheme is shown under, and its text.
The field is bound to the text and the dropdown to the scheme.
It also carries the schemes its dropdown offers, one choice per scheme the pack declares.

## `public QTranscriptionItem(long id, string scheme, string text)`

Builds a row from the plain values of one draft transcription.

## `public string QTranscriptionItemScheme`

The scheme the row spells the reading in, as picked or as the draft holds it.
An empty pick is ignored, because a dropdown rebinding hands one over on its way.
It announces only a real change, and announces the label with it.

## `public string QTranscriptionItemKey`

The localization key of the row's scheme, as Conduct chose it.
The owner writes it before the scheme, so the scheme's change announces the label with the new key.

## `public string QTranscriptionItemLabel`

The scheme's localized name, shown as the chip before the field the way a variety name is.

## `public string QTranscriptionItemText`

The transcription as typed or as the draft holds it.
It announces only a real change, so a render writing the same text stays silent.

## `internal void QTranscriptionSchemeRefine(IReadOnlyList<CScheme> schemes)`

Gives the dropdown the choices Conduct answered and marks the ones it answered taken.
The choices are built once and kept, so an open dropdown is not rebound under the pointer.
They are rebuilt only when the pack's scheme count differs, which a language switch brings.
Each kept choice takes the taken mark of the scheme at its place, since both follow the pack's order.

## `internal static void QTranscriptionItemRefine(FrameworkElement container, object item, string? _)`

Fills the parts both views share, which are the scheme name, the text and the unseen measure twin.
The editor's own refine in `QTranscription` calls this first, then fills its field and scheme dropdown.

## `private static void QTranscriptionMeasureRefine(TextBlock measure, string text)`

Sizes the measure twin by the text, or by the placeholder while the row is blank.

## `internal static QTranscriptionItem QTranscriptionRowRefine(CTranscriptionDraft spelled)`

Builds the row for one draft transcription, its scheme key included.

## `internal static QTranscriptionItem QTranscriptionStateRefine(QTranscriptionItem row, CTranscriptionDraft spelled)`

Brings a shown row up to the draft row with the same id.
A changed scheme is written onto the row, which relabels itself.
A row with a text request waiting keeps its text, since the draft is about to become what it holds.

## `internal static string QTranscriptionLabelRefine(string key, string scheme)`

A scheme's label, looked up under the key Conduct chose.
A missing translation prints the raw name, and an unnamed scheme prints nothing.
