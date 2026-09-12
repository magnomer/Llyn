# LRequestExample.cs

The requests of the example panel.
There is one per field of the sentence a draft holds on its own, and one for the whole body.
They name only the draft, because a draft holds one such sentence.
A sentence inside a card is edited through the sentence requests instead.

## `public sealed record LRequestExampleText(long LRequestDraftId, LStateValue LRequestValue)`

Replaces the text.

## `public sealed record LRequestExampleLanguage(long LRequestDraftId, string LRequestLanguage)`

Replaces the language.

## `public sealed record LRequestExampleReference(long LRequestDraftId, long LRequestReferenceId)`

Names the source the sentence cites, or zero to cite nothing.

## `public sealed record LRequestExampleBody`

Carries every field of the sentence at once, as the form currently holds them.
Text travels as written, with its mark, and the source as the chosen id or zero.
The Glosses are not part of the body, because each is a row of its own and travels through [LRequestGloss](LRequestGloss.comment.md).
The engine resolves each and keeps the held id.
A form sends this instead of comparing its controls against the draft field by field.
So the decision of what changed lives in the engine, and an unchanged body saves and announces nothing.
