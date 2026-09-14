# TSourceGeneric.cs

## `public sealed class TSourceGeneric`

Covers the generic source reading several varieties from one fetched body.
Two readings on one attempt both come back tagged.
A later attempt fills only the varieties an earlier one missed, first wins per variety.
A skip count passes over earlier matches of the same pattern.
A pattern matching several times yields every match once, so a page with two etymologies shows both readings.
A comma or slash inside one matched text separates alternatives, and each is a reading of its own.
A flat attempt is one untagged reading that yields every match, and the first phonetic is the answer's value.
A `{word}` token in a pattern is the headword, escaped so a regex character in it matches literally.
A missed guard is a blank answer and a server failure is a lost one.
A page that only points at another headword is followed there, and the reading comes from the page pointed at.
A page with its own readings that also points elsewhere keeps its own and adds the pointed page's.
Two pages pointing at each other stop at the hop limit with a blank reached answer.
The headword an attempt followed to is the one a later attempt asks for.

## Inline notes

### `private const string TSourceGenericBody =`

Two nested spans, one per variety, in the shape the span reader expects.
The outer span is the block a reading opens on and the inner one holds the transcription.
