# PCompassItem.cs

## `internal sealed class PCompassItem`

One row of the floating contents: what it is called, what number it carries, and how deep it sits.

### `internal FrameworkElement PCompassItemTarget { get; }`

The row holds the element it names rather than a stored offset.
The element keeps moving as the view is resized, and it knows where it is at every moment.

### `public bool PCompassItemCurrent`

Only this changes after the row is made, so it is the only member that raises a change.

## `public string PCompassItemName`

The label as the row shows it, numbered `(1)`, `(2)` while another row carries the same label.
`LTwin` writes it once the list is filled, because a repeat is only visible across rows.
`PCompassItemLabel` keeps the plain label for everything that is not display.
