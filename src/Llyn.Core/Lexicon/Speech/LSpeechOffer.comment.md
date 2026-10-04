# LSpeechOffer.cs
Hash: `b9e2434a27f873f0`

## `public sealed record LSpeechOffer(IReadOnlyList<LSpeechRow> LSpeechOfferRows, bool LSpeechOfferDeclared, bool LSpeechOfferMatched, bool LSpeechOfferShown)`

The parts of speech the language offers for the typed text, with the verdicts the offer needs.

**Parameters**

- `LSpeechOfferRows` — The catalog's parts whose name holds the trimmed typed text, in catalog order.
- `LSpeechOfferDeclared` — True when the language's catalog names any part at all.
- `LSpeechOfferMatched` — True when at least one part matches the typed text.
- `LSpeechOfferShown` — True when the typed text is not blank and some part matches it.
