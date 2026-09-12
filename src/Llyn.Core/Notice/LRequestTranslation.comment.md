# LRequestTranslation.cs

The translation requests.
A translation links only to an existing entry or to a draft the court opened, so there is no addition.
The id is the entry's, or the target draft's until that draft commits and the court settles it.

## `public sealed record LRequestTranslationPick(`

Links the entry to the card at `LRequestPosition`.
An entry the card already links is left as it is.

## `public sealed record LRequestTranslationRemoval(long LRequestDraftId, long LRequestCardId, long LRequestEntryId)`

Unlinks one entry from the card.
A court link behind it is the form's to close.

## `public sealed record LRequestTranslationShift(`

Moves one translation chip to `LRequestPosition` inside its card.
