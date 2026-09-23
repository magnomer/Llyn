# PNotationTemplate.xaml.cs

## `public partial class PNotationTemplate : ResourceDictionary`

This dictionary presents notation controls while the editor owns transcription and language state.

## `private readonly PEditor _pNotationHost`

The editor resolves notation selection against the transcription in the active card.

## `internal PNotationTemplate(PEditor host)`

The host routes shared notation controls into the owning editor context.

## `private void PNotationSelectorHandle(object sender, RoutedEventArgs e)`

The editor opens notation selection because it owns transcription updates.
