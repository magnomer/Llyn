# LSituationOffer.cs
Hash: `d82e5cd8b08662eb`

## `public sealed record LSituationOffer(`

What a situation field keeps after a gate, and the stored Situations it offers for the kept text.

**Parameters**

- `LSituationOfferText`: the text the field keeps once the completed titles went onto the card.
- `LSituationOfferRows`: the stored Situations the trimmed text matches, at most eight, leaving out those the card links.
- `LSituationOfferShown`: true when the text is not blank and some stored Situation is offered.
