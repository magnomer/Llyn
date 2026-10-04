# LReferenceOffer.cs
Hash: `732025f4c3e88fe3`

## `public sealed record LReferenceOffer(string LReferenceOfferText, IReadOnlyList<LReferenceRow> LReferenceOfferRows, bool LReferenceOfferShown)`

The stored Sources a sentence's citation field offers for the text typed into it.

**Parameters**

- `LReferenceOfferText` — The typed text, unchanged, since typing a citation writes nothing to the draft.
- `LReferenceOfferRows` — The stored Sources the trimmed text matches, most cited first, at most eight.
- `LReferenceOfferShown` — True when some Source is offered.
  The offer is empty for blank text and for the byline of the Source the sentence already cites.
