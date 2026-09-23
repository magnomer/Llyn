# LRequestExample.cs

The requests of the example panel.
There is one per field of the sentence a draft holds on its own.
They name only the draft, because a draft holds one such sentence.
A sentence inside a card is edited through the sentence requests instead.

## `public sealed record LRequestExampleText(long LRequestDraftId, LStateValue LRequestValue)`

Replaces the text.

## `public sealed record LRequestExampleLanguage(long LRequestDraftId, string LRequestLanguage)`

Replaces the language.

## `public sealed record LRequestExampleReference(long LRequestDraftId, long LRequestReferenceId)`

Names the source the sentence cites, or zero to cite nothing.
