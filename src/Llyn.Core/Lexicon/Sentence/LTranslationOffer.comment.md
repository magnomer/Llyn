# LTranslationOffer.cs

## `public sealed record LTranslationOffer(`

What a translation field keeps after a gate, and the Entries its word may link to.
The engine answers it whole, so the field shows it without deciding anything.

**Parameters**

- `LTranslationOfferText`: The text the entry keeps once the gate has linked what it could.
- `LTranslationOfferWord`: The trimmed word the offer was found for, or empty when nothing is offered.
- `LTranslationOfferRows`: The Entries the word matches, whole headwords first.
- `LTranslationOfferShown`: True when the word is not blank and the search answered, so the dropdown opens.
- `LTranslationOfferChosen`: True when one Entry answers the whole word, so its row stands chosen.
- `LTranslationOfferLanguages`: The languages a fresh Entry for the word is offered in, the draft's own last.
  A mention offers none, because it may only name an Entry that exists.
