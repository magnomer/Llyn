# PThemeFrequency.xaml

## `<Style x:Key="Theme.Frequency.Chip" TargetType="Border">`

The frequency of the headword as it reads.
It copies the part-of-speech chip with the accent-soft ground swapped for the neutral raised ground.
The two chip kinds stand one under the other, so they must read apart at a glance.
The chip hugs its text rather than stretching, because there is only ever one.

## `<Style x:Key="Theme.Frequency.Name" TargetType="TextBlock">`

The rung name.
The inked foreground is the resting value.
The shared label recolours it in the rung's brush.

## `<Style x:Key="Theme.Frequency.Band" TargetType="TextBlock">`

The row of four stars after the name, a point smaller so the glyphs sit on the text's x-height.

## `<SolidColorBrush x:Key="Theme.Frequency.Core" ...>`

Gold for the core thousand, the rung a learner meets first.

## `<SolidColorBrush x:Key="Theme.Frequency.Everyday" ...>`

Green for the everyday five thousand.

## `<SolidColorBrush x:Key="Theme.Frequency.Advanced" ...>`

Blue for the advanced thirty thousand.

## `<SolidColorBrush x:Key="Theme.Frequency.Rare" ...>`

Violet for the rest.

## `<SolidColorBrush x:Key="Theme.Frequency.Unknown" ...>`

Muted ink for an entry whose sources answered but earned no rung.

## `<SolidColorBrush x:Key="Theme.Frequency.Empty" ...>`

The unearned stars, muted ink at low opacity so they mark the row's width without reading as filled.
