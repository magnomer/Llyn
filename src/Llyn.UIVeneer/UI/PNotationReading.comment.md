# PNotationReading.cs

## `internal sealed class PNotationReading`

One transcription a source returned, as one takeable button on its source's row.
It is built once from the candidate and never changes, so it carries no change notification.

## `public string PNotationReadingVariety`

The raw variety name the source tagged the reading with, empty when the source did not say.
It is what the draft stores, never the label the user sees.

## `public string PNotationReadingLabel`

The name shown for the variety, localized when a `Variety.*` key exists and raw otherwise.
In flag mode it is the tooltip of the flag rather than visible text.

## `public ImageSource? PNotationReadingFlag`

The variety's flag, resolved when the language pack shows varieties as flags and the pack declares one.
Null means the label stands in for it.

## `public string PNotationReadingPhonetic`

The transcription as the source gave it, which is what taking the reading stores as the row's reading.
The engine derives the respelling from it again, so the pick fills both forms.

## `public string PNotationReadingText`

The form the button prints, the candidate's respelling while the switch shows respellings and the phonetic otherwise.
A transcription lookup carries no respelling and prints its text as it came.

## `public string PNotationReadingOpener`

The bracket drawn before the text, blank for a transcription, a slash for a phonemic respelling and square otherwise.

## `public string PNotationReadingCloser`

The bracket drawn after the text, chosen the same way.
