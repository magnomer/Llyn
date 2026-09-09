# PDisplayRegister.xaml

## `ResourceDictionary`

The register row of a read-only card, from its panel down to the single chip.

## Inline notes

### `<Style x:Key="Display.Card.RegisterLink" TargetType="Button">`

A register names the tone an entry is used in, so it is drawn as a chip and not as prose.
It takes the helper colour rather than the situation colour, so the two rows are never read as one.
