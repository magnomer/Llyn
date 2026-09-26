# PThemeFavorite.xaml

## `<sys:Double x:Key="Theme.Favorite.Size">`

The square the heart is given, and the height of the row it leads.

## `<Style x:Key="Theme.Favorite.Row" TargetType="StackPanel">`

The row of heart, stars and words that follows the language pill.
The editor and the reading view both draw it from this style.
It is as tall as the heart, so the stars and the words center against one height in both.
Left to size itself, the row would be the heart's height in one view and the stars' in the other.

## `<Style x:Key="Theme.Favorite.Icon" TargetType="Path">`

The heart the favourite toggle carries, hollow until the entry is kept.
A heart, not a star, because the stars beside it belong to the grasp row.
It is drawn in the theme's favorite rose rather than the accent, for the same reason.
Deportment fills the heart through `PLook` rows on the toggle, keyed to the icon named `PFavoriteMark`.

## `<Style x:Key="Theme.Favorite.Mark" TargetType="ToggleButton">`

The heart's square, its gap after the pill and its centering are set here, once.

The heart carries the mark alone, with no surface or border behind it.
A framed button would read as a control among the headword's own text.
Hover and press dim it instead, so the button still answers the pointer.
Deportment sets that dimming and the padding through `PLook` rows.
