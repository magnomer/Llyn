# PMedia.cs

## `internal sealed class PMedia`

The row maker a reading view hands down to the picture and film rows built inside its templates.
A reading view binds a card's locations straight from the draft, so no code of its own builds the rows.
The rows still need the engine to turn a location into an address, and this carries it down for them.

## `public static readonly DependencyProperty PMediaProperty`

Attached and inherited, so one write on the view's root reaches every template under it.

## `internal static void PMediaAttach(DependencyObject root, LEngine engine)`

Puts one maker on the view's root, made over the engine the view was attached to.
