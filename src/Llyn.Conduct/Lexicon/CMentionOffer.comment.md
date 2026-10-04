# CMentionOffer.cs
Hash: `1cc7318417f1a177`

## `public sealed record CMentionOffer(int? CMentionOfferUnit, IReadOnlyList<CTranslationTarget> CMentionOfferEntry, string CMentionOfferKey)`

What a word click leaves for the menu, once the gate has opened what it opens at once.
It carries the menu's place as a ready surface value, so the control converts no offset.

**Parameters**

- `CMentionOfferUnit`: the UTF-16 unit in the whole shown text where the found word starts.
  The menu hangs under it, whichever run of the control holds that unit.
  Null when the found start lies outside the text, so the menu hangs at the control's bottom left.
- `CMentionOfferEntry`: the Entries the menu offers, empty when the click opened one or found none.
  An empty offer still closes an open menu.
- `CMentionOfferKey`: the localization key of the menu's title, which the driver looks up.
