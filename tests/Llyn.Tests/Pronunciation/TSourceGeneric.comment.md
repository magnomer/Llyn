# TSourceGeneric.cs

## `public sealed class TSourceGeneric`

Covers the generic source reading several varieties from one fetched body.
Two readings on one attempt both come back tagged.
A later attempt fills only the varieties an earlier one missed, first wins per variety.
A skip count passes over earlier matches of the same pattern.
A flat attempt is one untagged reading, and the first phonetic is the answer's value.
A missed guard is a blank answer and a server failure is a lost one.

## Inline notes

### `private const string TSourceGenericBody =`

Two nested spans, one per variety, in the shape the span reader expects.
The outer span is the block a reading opens on and the inner one holds the transcription.
