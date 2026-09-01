# PCard.cs

## Inline notes

### `public string PCardId { get; set; } = string.Empty;`

Id of the stored row this card was loaded from, empty for a card typed into an empty form. No control shows it and nothing on screen changes with it: it travels with the card so a saved form can say which stored Meaning or Collocation each card is, which is what lets an update change the row a card came from instead of writing it again.
