# CMentionOffer.cs

## `public sealed record CMentionOffer(int CMentionOfferOffset, IReadOnlyList<CTranslationTarget> CMentionOfferEntry)`

What a word click leaves for the menu, once the gate has opened what it opens at once.

**Parameters**

- `CMentionOfferOffset`: where the found word starts in the text, so the menu hangs under it.
- `CMentionOfferEntry`: the Entries the menu offers, empty when the click opened one or found none.
  An empty offer still closes an open menu.
