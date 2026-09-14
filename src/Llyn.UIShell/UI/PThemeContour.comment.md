# PThemeContour.xaml

## `<SolidColorBrush x:Key="Theme.Contour.Top" ...>`

The colour of Chao level five, the highest pitch.
`High`, `Mid`, `Low` and `Bottom` follow it down to level one.
The five are distinct hues rather than shades, so a contour reads by colour as well as by shape.
They step from warm at the top to cool at the bottom.
So a fall and a rise are told apart at a glance.

## `<SolidColorBrush x:Key="Theme.Contour.Guide" ...>`

The five guide lines, drawn in the theme's line colour so they sit behind the contour.

## `<SolidColorBrush x:Key="Theme.Contour.Axis" ...>`

The level digits along the left edge, muted like a label.

## `<SolidColorBrush x:Key="Theme.Contour.Ink" ...>`

The syllable text under each cell, in the theme's ink.

## `<SolidColorBrush x:Key="Theme.Contour.Frame" ...>`

The box fill, the theme's surface, so the box reads as a card under the pronunciation chip.

## `<SolidColorBrush x:Key="Theme.Contour.Edge" ...>`

The box outline, the theme's line colour.

## `<Style x:Key="Theme.Contour.Box" TargetType="local:PContour">`

Places the box under the pronunciation row in both the editor and the reading view.
It hugs the left, like the rows above it, and takes its own width from its syllables.
