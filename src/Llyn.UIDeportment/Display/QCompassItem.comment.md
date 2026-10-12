# QCompassItem.cs
Hash: `45e54ad36956b2ae`

## `public sealed class QCompassItem : INotifyPropertyChanged`

One row of the floating contents: what it is called, what number it carries, and how deep it sits.
Its constructor is public because [QCompass](QCompass.comment.md) builds the rows from Conduct's answer.

### `public FrameworkElement QCompassItemTarget { get; }`

The row holds the element it names rather than a stored offset.
The element keeps moving as the view is resized, and it knows where it is at every moment.

### `public string QCompassItemName { get; }`

The name as Conduct answered it, numbered `(1)`, `(2)` while another row carries the same one.

### `public bool QCompassItemMuted { get; }`

Conduct's verdict that the row is a count line, not a name, which the roster paints muted.

### `public bool QCompassItemCurrent`

It changes as the view scrolls, so it is the only member that raises a change.
