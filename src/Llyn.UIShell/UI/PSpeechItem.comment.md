# PSpeechItem.cs

## `internal sealed class PSpeechItem`

Presentation item for one preset row in the `PSpeech` dropdown.
It is the display name the chosen language declares for one part of speech.
That is also what the row writes into the field when it is clicked.
The stable id behind the name is not carried here.
The field holds text, and turning that text back into an id is the engine's decision.
It is made at the write, the same way for a picked name and a typed one.
