# PThemePanel.xaml

## `<Style x:Key="Theme.Panel.Surface" TargetType="Border">`

The region a panel gives a whole screen of its own, drawn on nothing.
It carries no outline.
A region is not an object, and an outline at one pixel could not say that it was.
The outline it once carried was the only thing marking the region, which is why the region read as unfinished.
What separates one region from the next is a seam, not a box drawn round each of them.

## `<Style x:Key="Theme.Panel.Helper" TargetType="Border">`

The region a helper stands on, the same region shape carried on the helper ground instead of the panel's own.
A helper is an aid to the work rather than the work.
A reader must be able to see that before reading a word of it.
The ground is one step cooler and deeper than the canvas.
That is enough to read as another kind of region.
It stays inside the program's own blue-grey.
A helper is a quieter thing than a choice or a verdict and must not out-colour either.

## `<Style x:Key="Theme.Panel.SeamRow" TargetType="Rectangle">`

The hairline laid across a panel where its controls end and its contents begin.
It is drawn at the foot of the row it belongs to and bled past the panel's own margin.
It therefore runs the width of the window.
A seam marks a division between two regions, never an enclosure around one.

## `<Style x:Key="Theme.Panel.SeamColumn" TargetType="Rectangle">`

The hairline laid down the gutter between two columns of a panel.
It is set against the leading edge of the right column and pulled back into the middle of the gutter.
It is bled past the panel's foot so it reaches the bottom of the window.
The row seam reaches both sides the same way.
