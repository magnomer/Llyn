# LTagOffer.cs
Hash: `d72f09d5ee506774`

## `public sealed record LTagOffer(`

What a tag field keeps after a gate, and the stored Tags it offers for the kept text.

**Parameters**

- `LTagOfferText`: the text the field keeps once the completed tags went onto the card.
- `LTagOfferRows`: the stored Tags the trimmed text matches, at most eight, leaving out those the card holds.
- `LTagOfferShown`: true when the text is not blank and some stored Tag is offered.
