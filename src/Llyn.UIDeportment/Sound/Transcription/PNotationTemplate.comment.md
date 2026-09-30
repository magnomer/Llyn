# PNotationTemplate.cs

## `public class PNotationTemplate : ResourceDictionary`

This dictionary presents notation controls while the editor owns transcription and language state.

## `internal PNotationTemplate()`

The dictionary merges the markup its Veneer URI loads.
The editor subscribes `PEditor.PNotationSelectorObserve` on each realized reading button.
