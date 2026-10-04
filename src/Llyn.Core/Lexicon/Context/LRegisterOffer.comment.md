# LRegisterOffer.cs
Hash: `02633798ed7727e1`

## `public sealed record LRegisterOffer(string LRegisterOfferText, IReadOnlyList<LRegisterRow> LRegisterOfferRows, bool LRegisterOfferShown)`

What a register field keeps after a gate, and the stored Registers it offers for the kept text.

**Parameters**

- `LRegisterOfferText`: the text the field keeps once the completed names went onto the card.
- `LRegisterOfferRows`: the stored Registers the trimmed text matches, at most eight, leaving out those the card links.
- `LRegisterOfferShown`: true when the text is not blank and some stored Register is offered.
