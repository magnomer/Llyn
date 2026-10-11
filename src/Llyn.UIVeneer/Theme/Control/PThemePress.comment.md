# PThemePress.xaml
Hash: `e55c56f07808710c`

## `<ResourceDictionary.MergedDictionaries>`

Merges the palette for its brushes and the scroll part for the canvas base style.

## `<Style x:Key="Theme.Press.Column" TargetType="Border">`

The settings column of the print window, at a fixed width beside the canvas.
Its 370 leaves the labels their width beside the window's 232 wide controls.
It has no ground of its own, so the settings sit on the window like the settings page.

## `<Style x:Key="Theme.Press.Canvas" TargetType="ScrollViewer" BasedOn="{StaticResource {x:Type ScrollViewer}}">`

The page canvas, on the raised surface so white paper stands out in any palette.
It builds on the app scroll viewer, so its bars match every other list.

## `<Style x:Key="Theme.Press.Paper" TargetType="Border">`

One printed sheet on the canvas.
Its ground is white in every palette, because the printed output is white.
The shadow is the popup shadow, so the sheet floats the way a popup does.

## `<Style x:Key="Theme.Press.Shade" TargetType="Border">`

Dims a sheet that the chosen range leaves out.
The page stays in place and in order, so page numbers never shift.

## `<Style x:Key="Theme.Press.Badge" TargetType="Border">`

The small tag on a dimmed sheet that says it is not printed.

## `<Style x:Key="Theme.Press.Veil" TargetType="Border">`

The translucent cover over the canvas while pages render or after rendering failed.
The last pages show through it, so the reader keeps their place.

## `<Style x:Key="Theme.Press.Bar" TargetType="Border">`

The strip under the canvas for page moves and zoom.
A hairline on top divides it from the pages.
