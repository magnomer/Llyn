# PSpeechItem.cs

## `internal sealed class PSpeechItem`

Presentation item for one preset row in the `PSpeech` dropdown: the display name the chosen language declares for one part of speech, which is also what the row writes into the field when it is clicked. The stable id behind the name is not carried here — the field holds text, and turning that text back into an id is the engine's decision at the write, made the same way for a name that was picked and a name that was typed.
