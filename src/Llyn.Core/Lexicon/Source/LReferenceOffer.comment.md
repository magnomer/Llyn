# LReferenceOffer.cs

## `public sealed record LReferenceOffer(`

The stored Sources a sentence's citation field offers for the text typed into it.

**Parameters**

- `LReferenceOfferText`: the typed text, unchanged, since typing a citation writes nothing to the draft.
- `LReferenceOfferRows`: the stored Sources the trimmed text matches, most cited first, at most eight.
- `LReferenceOfferShown`: true when some Source is offered.
  The offer is empty for blank text and for the byline of the Source the sentence already cites.
