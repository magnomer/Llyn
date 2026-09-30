# PNotationTemplate.cs

## `public class PNotationTemplate : ResourceDictionary`

This dictionary presents notation controls while the editor owns transcription and language state.

## `private readonly PEditor _pNotationHost`

The editor resolves notation selection against the transcription in the active card.

## `internal PNotationTemplate(PEditor host)`

The host routes shared notation controls into the owning editor context.
The dictionary merges the markup its Veneer URI loads.

## `internal void PNotationSelectorHandle(object sender, RoutedEventArgs e)`

Forwards a pressed reading to `PEditor.PNotationSelectorObserve`.
The reading fill subscribes it on each realized reading button.
