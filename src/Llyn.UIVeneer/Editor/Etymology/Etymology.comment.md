# Etymology

This folder holds the editor's etymology card.
The card is a header and a map of source boxes drawn from the card theme.

## `PEtymology`

The card has the same frame and head strip as the other editor cards.
The map scrolls sideways only, and sits at the foot of the card, so the headword stays in view.
Source boxes stand above the headword and grow upward.
The sources list is named, so the driver can fill it with the tree.
`PEtymologyTrunk` joins the sources to the headword, and the driver hides it while the map is empty.
The headword stands in the `Theme.Etymology.Root` box, centred under the map.
Its insert button stands to its right, so the headword keeps the centre.
The insert button puts a box above the headword, and it has no handler here.
The code behind only builds the parts, since the deportment drives every part.

## Stamp

Commit: `c6e60141a172`
- `PEtymology.xaml` `5174542c8d7f2f97`
- `PEtymology.xaml.cs` `9808f301e0392c34`
