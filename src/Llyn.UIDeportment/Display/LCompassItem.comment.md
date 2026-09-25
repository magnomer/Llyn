# LCompassItem.cs

## `public sealed class LCompassItem`

One row of the floating contents: what it is called, what number it carries, and how deep it sits.
Its constructor and setters are public because [LCompass](LCompass.comment.md) builds the rows.

### `public FrameworkElement LCompassItemTarget { get; }`

The row holds the element it names rather than a stored offset.
The element keeps moving as the view is resized, and it knows where it is at every moment.

### `public bool LCompassItemCurrent`

It changes as the view scrolls, so it is the only member that raises a change.

## `public string LCompassItemName`

The label as the row shows it, numbered `(1)`, `(2)` while another row carries the same label.
`LCompass` writes it through `LDisplayNameResolve` once the list is filled, because a repeat is only visible across rows.
`LCompassItemLabel` keeps the plain label for everything that is not display.
