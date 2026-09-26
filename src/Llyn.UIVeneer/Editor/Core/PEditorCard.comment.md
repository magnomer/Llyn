# PEditorCard.xaml

## `ResourceDictionary`

The button that adds a card, apart from the two lists it stands under.
A meaning list and a collocation list end the same way, so the ending is written once.

## Inline notes

### `<Style x:Key="Editor.Card.Add" TargetType="Button">`

A dashed outline says the card is not there yet and the plus says it can be.
Only the wording and the list it adds to differ, and both stay in the markup.
The plus is the named part `PSurfaceIcon`, which a look row fills with its icon.
Each add button is named in the markup, so the editor subscribes its click.
