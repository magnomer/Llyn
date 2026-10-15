# Esteem

This folder holds the looks of the marks put on an entry.
They are the favourite heart, the grasp stars and the frequency chip.

## Favorite styles

`Theme.Favorite.Row` is the row of heart, stars and words that follows the language pill.
The editor and the reading view both draw it from this style.
It is as tall as the heart, so the stars and the words centre against one height in both.
Left to size itself, the row would be the heart's height in one view and the stars' in the other.
`Theme.Favorite.Icon` is the heart the toggle carries, hollow until the entry is kept.
It is a heart because the stars beside it belong to the grasp row.
Its icon is drawn in pink rather than the accent for the same reason.
Deportment fills the heart through `QLook` rows on the toggle, keyed to the icon named `PFavoriteMark`.
`Theme.Favorite.Mark` sets the heart's square, its gap after the pill and its centring once.
The heart carries the mark alone, because a framed button would read as a control among the headword's own text.
Hover and press dim it instead, so the button still answers the pointer.
Deportment sets that dimming and the padding through `QLook` rows.

## Frequency styles

`Theme.Frequency.Chip` shows the frequency of the headword, copying the part-of-speech chip with the neutral raised ground.
The two chip kinds stand one under the other, so they must read apart at a glance.
The chip hugs its text rather than stretching, because there is only ever one.
`Theme.Frequency.Name` is the rung name, inked at rest and recoloured by the shared label in the rung's brush.
`Theme.Frequency.Band` is the row of four stars after the name.
It is a point smaller so the glyphs sit on the text's x-height.
The rung brushes are gold for core and green for everyday.
Blue marks advanced, and violet marks rare.
`Theme.Frequency.Unknown` is muted ink for an entry whose sources answered but earned no rung.
`Theme.Frequency.Empty` is muted ink at low opacity, so unearned stars mark the row's width without reading as filled.

## Grasp styles

`Theme.Grasp.Fill` is the fill of a star the user has earned, in the accent.
The heart beside it fills in its own pink, so the two marks are told apart at a glance.
`Theme.Grasp.Empty` is the outline every star keeps once a step stands or a hover previews one.
`Theme.Grasp.Unrated` is the muted outline while the entry stands unrated and nothing hovers.
So an unjudged row does not call for attention.
`Theme.Grasp.Preview` is the accent at half strength, drawn while the pointer hovers before a click commits.
It is strong enough to read at a glance, yet plainly not the committed fill.
`Theme.Grasp.Mark` sets the star row's gap after the heart and its centring once for both views.
`Theme.Grasp.Label` is the muted words saying what the shown step means.
So the stars stay the mark and the words the gloss.

## Stamp

Commit: `2f9a06b8eb95`
- `PThemeFavorite.xaml` `09326dc14eb43c83`
- `PThemeFrequency.xaml` `8eb759c9eefdc88d`
- `PThemeGrasp.xaml` `e8206b66874f48ca`
