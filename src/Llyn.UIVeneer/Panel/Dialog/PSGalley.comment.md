# PSGalley.xaml
Hash: `a982ebf5b1fed6ae`

## `<UserControl>`

The print preview, the page canvas with its veils above and its bar below.
`PSPress` hosts it in the bordered cell beside the settings.
It is a control of its own because the window's markup outgrew one file.
Its class is a bare shell, and the drivers find every part by its contract ID.
The lookup walks the logical tree, so the IDs inside this control stay reachable from the window.

## `<ScrollViewer x:Name="PSGalleyCanvas" ...>`

The canvas stacks one white paper per page on the raised surface.
It is named `PSGalleyCanvas`, since the control itself carries the `PSGalley` class name.
White stays fixed whatever the palette, because the printed sheet is white.
Each paper holds the drawn page, a shade and a badge for a page left out of the range.

## `<Grid x:Name="PSPressBusy" ...>`

The busy veil keeps the last pages visible beneath it, so a change never blanks the canvas.
It covers only the canvas row, so the bar stays usable while a page renders.
It keeps its `PSPress` ID, since `QSPress` drives it as window state.

## `<Grid x:Name="PSPressFailure" ...>`

The failure veil says what went wrong and offers one retry.
It covers only the canvas row and keeps its `PSPress` IDs, as the busy veil does.

## `<Border Grid.Row="1" Style="{StaticResource Theme.Press.Bar}">`

The canvas bar moves between pages on the left and sets the zoom on the right.
`QSGalley` drives the page half and `QSGalleyBar` drives the zoom half.
The sheet count fills the space between them and trims first when the window narrows.
The zoom percent is a text field, since the input choice template has no editable face.
Every icon button carries a tooltip and every control an accessible name from the print keys.
